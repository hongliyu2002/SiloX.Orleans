using System.Collections.Concurrent;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Machines;
using Vending.Domain.Abstractions.Snacks;
using Vending.Projection.Abstractions.Machines;
using Vending.Projection.EntityFrameworkCore;

namespace Vending.Projection.Machines;

[ImplicitStreamSubscription(Constants.MachinesNamespace)]
public sealed class MachineProjectionGrain : SubscriberGrainWithGuidKey<MachineEvent, MachineErrorEvent>, IMachineProjectionGrain
{
    private readonly ProjectionDbContext _dbContext;
    private readonly ILogger<MachineProjectionGrain> _logger;

    /// <inheritdoc />
    public MachineProjectionGrain(ProjectionDbContext dbContext, ILogger<MachineProjectionGrain> logger)
        : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetStreamNamespace()
    {
        return Constants.MachinesNamespace;
    }

    /// <inheritdoc />
    protected override Task HandLeEventAsync(MachineEvent domainEvent)
    {
        switch (domainEvent)
        {
            case MachineInitializedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineRemovedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineMoneyLoadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineMoneyUnloadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineMoneyInsertedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineMoneyReturnedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineSnacksLoadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineSnacksUnloadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineSnackBoughtEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            default:
                return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    protected override Task HandLeErrorEventAsync(MachineErrorEvent errorEvent)
    {
        _logger.LogWarning("MachineErrorEvent received: {Reasons}", string.Join(';', errorEvent.Reasons));
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task HandleExceptionAsync(Exception exception)
    {
        _logger.LogError(exception, "Exception is {Message}", exception.Message);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task HandleCompleteAsync()
    {
        _logger.LogInformation($"Stream {Constants.MachinesNamespace} is completed.");
        return Task.CompletedTask;
    }

    private async Task ApplyEventAsync(MachineInitializedEvent machineEvent)
    {
        try
        {
            var machine = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machine == null)
            {
                machine = new MachineInfo
                          {
                              Id = machineEvent.MachineId,
                              MoneyInfoInside = machineEvent.MoneyInside.ToProjection(),
                              Slots = await Task.WhenAll(machineEvent.Slots.Select(slot => slot.ToProjection(GetSnackNameAndPictureUrlAsync))),
                              SlotsCount = machineEvent.SlotsCount,
                              SnackCount = machineEvent.SnackCount,
                              SnackQuantity = machineEvent.SnackQuantity,
                              SnackAmount = machineEvent.SnackAmount,
                              CreatedAt = machineEvent.OperatedAt,
                              CreatedBy = machineEvent.OperatedBy,
                              Version = machineEvent.Version
                          };
                _dbContext.Machines.Add(machine);
            }
            if (_dbContext.Entry(machine).State != EntityState.Added)
            {
                _logger.LogWarning($"Apply MachineInitializedEvent: SnackInfo machine {machineEvent.MachineId} is already in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineInitializedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineRemovedEvent machineEvent)
    {
        try
        {
            var machine = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply MachineRemovedEvent: SnackInfo machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply MachineRemovedEvent: SnackInfo machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machine.MoneyInfoInside = machineEvent.MoneyInside.ToProjection(machine.MoneyInfoInside);
            machine.AmountInTransaction = machineEvent.AmountInTransaction;
            machine.Slots = await Task.WhenAll(machineEvent.Slots.Select(slot => slot.ToProjection(GetSnackNameAndPictureUrlAsync, machine.Slots.FirstOrDefault(ms => ms.MachineId == slot.MachineId && ms.Position == slot.Position))));
            machine.SlotsCount = machineEvent.SlotsCount;
            machine.SnackCount = machineEvent.SnackCount;
            machine.SnackQuantity = machineEvent.SnackQuantity;
            machine.SnackAmount = machineEvent.SnackAmount;
            machine.DeletedAt = machineEvent.OperatedAt;
            machine.DeletedBy = machineEvent.OperatedBy;
            machine.IsDeleted = true;
            machine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineRemovedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineMoneyLoadedEvent machineEvent)
    {
        try
        {
            var machine = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply MachineMoneyLoadedEvent: SnackInfo machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply MachineMoneyLoadedEvent: SnackInfo machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machine.MoneyInfoInside = machineEvent.MoneyInside.ToProjection(machine.MoneyInfoInside);
            machine.LastModifiedAt = machineEvent.OperatedAt;
            machine.LastModifiedBy = machineEvent.OperatedBy;
            machine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyLoadedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineMoneyUnloadedEvent machineEvent)
    {
        try
        {
            var machine = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply MachineMoneyUnloadedEvent: SnackInfo machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply MachineMoneyUnloadedEvent: SnackInfo machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machine.MoneyInfoInside = machineEvent.MoneyInside.ToProjection(machine.MoneyInfoInside);
            machine.LastModifiedAt = machineEvent.OperatedAt;
            machine.LastModifiedBy = machineEvent.OperatedBy;
            machine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyUnloadedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineMoneyInsertedEvent machineEvent)
    {
        try
        {
            var machine = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply MachineMoneyInsertedEvent: SnackInfo machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply MachineMoneyInsertedEvent: SnackInfo machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machine.MoneyInfoInside = machineEvent.MoneyInside.ToProjection(machine.MoneyInfoInside);
            machine.AmountInTransaction = machineEvent.AmountInTransaction;
            machine.LastModifiedAt = machineEvent.OperatedAt;
            machine.LastModifiedBy = machineEvent.OperatedBy;
            machine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyInsertedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineMoneyReturnedEvent machineEvent)
    {
        try
        {
            var machine = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply MachineMoneyReturnedEvent: SnackInfo machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply MachineMoneyReturnedEvent: SnackInfo machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machine.MoneyInfoInside = machineEvent.MoneyInside.ToProjection(machine.MoneyInfoInside);
            machine.AmountInTransaction = machineEvent.AmountInTransaction;
            machine.LastModifiedAt = machineEvent.OperatedAt;
            machine.LastModifiedBy = machineEvent.OperatedBy;
            machine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyReturnedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineSnacksLoadedEvent machineSnacksEvent)
    {
        try
        {
            var machine = await _dbContext.Machines.Include(sm => sm.Slots).FirstOrDefaultAsync(sm => sm.Id == machineSnacksEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply MachineSnacksLoadedEvent: SnackInfo machine {machineSnacksEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineSnacksEvent);
                return;
            }
            if (machine.Version != machineSnacksEvent.Version - 1)
            {
                _logger.LogWarning($"Apply MachineSnacksLoadedEvent: SnackInfo machine {machineSnacksEvent.MachineId} version {machine.Version}) in the database should be {machineSnacksEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineSnacksEvent);
                return;
            }
            var slot = machine.Slots.FirstOrDefault(ms => ms.MachineId == machineSnacksEvent.Slot.MachineId && ms.Position == machineSnacksEvent.Slot.Position);
            if (slot == null)
            {
                slot = new MachineSlotInfo();
                machine.Slots.Add(slot);
            }
            await machineSnacksEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            machine.SlotsCount = machineSnacksEvent.SlotsCount;
            machine.SnackCount = machineSnacksEvent.SnackCount;
            machine.SnackQuantity = machineSnacksEvent.SnackQuantity;
            machine.SnackAmount = machineSnacksEvent.SnackAmount;
            machine.LastModifiedAt = machineSnacksEvent.OperatedAt;
            machine.LastModifiedBy = machineSnacksEvent.OperatedBy;
            machine.Version = machineSnacksEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyReturnedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineSnacksEvent);
        }
    }

    private async Task ApplyEventAsync(MachineSnacksUnloadedEvent machineSnacksEvent)
    {
        try
        {
            var machine = await _dbContext.Machines.Include(sm => sm.Slots).FirstOrDefaultAsync(sm => sm.Id == machineSnacksEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply MachineSnacksUnloadedEvent: SnackInfo machine {machineSnacksEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineSnacksEvent);
                return;
            }
            if (machine.Version != machineSnacksEvent.Version - 1)
            {
                _logger.LogWarning($"Apply MachineSnacksUnloadedEvent: SnackInfo machine {machineSnacksEvent.MachineId} version {machine.Version}) in the database should be {machineSnacksEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineSnacksEvent);
                return;
            }
            var slot = machine.Slots.FirstOrDefault(ms => ms.MachineId == machineSnacksEvent.Slot.MachineId && ms.Position == machineSnacksEvent.Slot.Position);
            if (slot == null)
            {
                slot = new MachineSlotInfo();
                machine.Slots.Add(slot);
            }
            await machineSnacksEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            machine.SlotsCount = machineSnacksEvent.SlotsCount;
            machine.SnackCount = machineSnacksEvent.SnackCount;
            machine.SnackQuantity = machineSnacksEvent.SnackQuantity;
            machine.SnackAmount = machineSnacksEvent.SnackAmount;
            machine.LastModifiedAt = machineSnacksEvent.OperatedAt;
            machine.LastModifiedBy = machineSnacksEvent.OperatedBy;
            machine.Version = machineSnacksEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineSnacksUnloadedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineSnacksEvent);
        }
    }

    private async Task ApplyEventAsync(MachineSnackBoughtEvent machineSnackEvent)
    {
        try
        {
            var machine = await _dbContext.Machines.Include(sm => sm.Slots).FirstOrDefaultAsync(sm => sm.Id == machineSnackEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply MachineSnackBoughtEvent: SnackInfo machine {machineSnackEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineSnackEvent);
                return;
            }
            if (machine.Version != machineSnackEvent.Version - 1)
            {
                _logger.LogWarning($"Apply MachineSnackBoughtEvent: SnackInfo machine {machineSnackEvent.MachineId} version {machine.Version}) in the database should be {machineSnackEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineSnackEvent);
                return;
            }
            machine.AmountInTransaction = machineSnackEvent.AmountInTransaction;
            var slot = machine.Slots.FirstOrDefault(ms => ms.MachineId == machineSnackEvent.Slot.MachineId && ms.Position == machineSnackEvent.Slot.Position);
            if (slot == null)
            {
                slot = new MachineSlotInfo();
                machine.Slots.Add(slot);
            }
            await machineSnackEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            machine.SnackQuantity = machineSnackEvent.SnackQuantity;
            machine.SnackAmount = machineSnackEvent.SnackAmount;
            machine.LastModifiedAt = machineSnackEvent.OperatedAt;
            machine.LastModifiedBy = machineSnackEvent.OperatedBy;
            machine.Version = machineSnackEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyReturnedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineSnackEvent);
        }
    }

    private async Task ApplyFullUpdateAsync(MachineEvent machineEvent)
    {
        var attempts = 0;
        bool retryNeeded;
        do
        {
            try
            {
                var machineId = machineEvent.MachineId;
                var machineGrain = GrainFactory.GetGrain<IMachineGrain>(machineId);
                var machineInGrain = await machineGrain.GetMachineAsync();
                var machine = await _dbContext.Machines.FindAsync(machineId);
                if (machineInGrain == null)
                {
                    if (machine == null)
                    {
                        return;
                    }
                    _dbContext.Remove(machine);
                    await _dbContext.SaveChangesAsync();
                    return;
                }
                if (machine == null)
                {
                    machine = new MachineInfo();
                    _dbContext.Machines.Add(machine);
                }
                machine = await machineInGrain.ToProjection(GetSnackNameAndPictureUrlAsync, machine);
                machine.Version = await machineGrain.GetVersionAsync();
                var purchaseStatsGrain = GrainFactory.GetGrain<IMachineStatsOfPurchasesGrain>(machineId);
                machine.BoughtCount = await purchaseStatsGrain.GetBoughtCountAsync();
                machine.BoughtAmount = await purchaseStatsGrain.GetBoughtAmountAsync();
                await _dbContext.SaveChangesAsync();
                return;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                retryNeeded = ++attempts <= 3;
                if (retryNeeded)
                {
                    _logger.LogWarning(ex, $"ApplyFullUpdateAsync: DbUpdateConcurrencyException is occurred when try to write data to the database. Retrying {attempts}...");
                    await Task.Delay(TimeSpan.FromSeconds(attempts));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApplyFullUpdateAsync: Exception is occurred when try to write data to the database.");
                retryNeeded = false;
            }
        }
        while (retryNeeded);
    }

    #region Get SnackInfo Name And Picture Url

    private readonly ConcurrentDictionary<Guid, (string SnackName, string? SnackPictureUrl)> _snackNamePictureCache = new();

    private async Task<(string SnackName, string? SnackPictureUrl)> GetSnackNameAndPictureUrlAsync(Guid snackId)
    {
        if (!_snackNamePictureCache.TryGetValue(snackId, out var snackNamePicture))
        {
            var snackGrain = GrainFactory.GetGrain<ISnackGrain>(snackId);
            var snackInGrain = await snackGrain.GetSnackAsync();
            snackNamePicture = (snackInGrain.Name, snackInGrain.PictureUrl);
            _snackNamePictureCache.TryAdd(snackId, snackNamePicture);
        }
        return snackNamePicture;
    }

    #endregion

}

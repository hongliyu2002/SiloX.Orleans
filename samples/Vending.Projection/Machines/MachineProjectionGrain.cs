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
            case MachineLoadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineUnloadedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineBoughtEvent machineEvent:
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
            machine.Slots = await Task.WhenAll(machineEvent.Slots.Select(slot => slot.ToProjection(GetSnackNameAndPictureUrlAsync, machine.Slots.FirstOrDefault(sl => sl.MachineId == slot.MachineId && sl.Position == slot.Position))));
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

    private async Task ApplyEventAsync(MachineLoadedEvent machineEvent)
    {
        try
        {
            var machine = await _dbContext.Machines.Include(sm => sm.Slots).FirstOrDefaultAsync(sm => sm.Id == machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply MachineLoadedEvent: SnackInfo machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply MachineLoadedEvent: SnackInfo machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            var slot = machine.Slots.FirstOrDefault(sl => sl.MachineId == machineEvent.Slot.MachineId && sl.Position == machineEvent.Slot.Position);
            if (slot == null)
            {
                slot = new SlotInfo();
                machine.Slots.Add(slot);
            }
            await machineEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            machine.SlotsCount = machineEvent.SlotsCount;
            machine.SnackCount = machineEvent.SnackCount;
            machine.SnackQuantity = machineEvent.SnackQuantity;
            machine.SnackAmount = machineEvent.SnackAmount;
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

    private async Task ApplyEventAsync(MachineUnloadedEvent machineEvent)
    {
        try
        {
            var machine = await _dbContext.Machines.Include(sm => sm.Slots).FirstOrDefaultAsync(sm => sm.Id == machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply MachineUnloadedEvent: SnackInfo machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply MachineUnloadedEvent: SnackInfo machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            var slot = machine.Slots.FirstOrDefault(sl => sl.MachineId == machineEvent.Slot.MachineId && sl.Position == machineEvent.Slot.Position);
            if (slot == null)
            {
                slot = new SlotInfo();
                machine.Slots.Add(slot);
            }
            await machineEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            machine.SlotsCount = machineEvent.SlotsCount;
            machine.SnackCount = machineEvent.SnackCount;
            machine.SnackQuantity = machineEvent.SnackQuantity;
            machine.SnackAmount = machineEvent.SnackAmount;
            machine.LastModifiedAt = machineEvent.OperatedAt;
            machine.LastModifiedBy = machineEvent.OperatedBy;
            machine.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineUnloadedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineBoughtEvent machineEvent)
    {
        try
        {
            var machine = await _dbContext.Machines.Include(sm => sm.Slots).FirstOrDefaultAsync(sm => sm.Id == machineEvent.MachineId);
            if (machine == null)
            {
                _logger.LogWarning($"Apply MachineBoughtEvent: SnackInfo machine {machineEvent.MachineId} does not exist in the database. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machine.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning($"Apply MachineBoughtEvent: SnackInfo machine {machineEvent.MachineId} version {machine.Version}) in the database should be {machineEvent.Version - 1}. Try to execute full update...");
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machine.AmountInTransaction = machineEvent.AmountInTransaction;
            var slot = machine.Slots.FirstOrDefault(sl => sl.MachineId == machineEvent.Slot.MachineId && sl.Position == machineEvent.Slot.Position);
            if (slot == null)
            {
                slot = new SlotInfo();
                machine.Slots.Add(slot);
            }
            await machineEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            machine.SnackQuantity = machineEvent.SnackQuantity;
            machine.SnackAmount = machineEvent.SnackAmount;
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
                var machineInGrain = await machineGrain.GetStateAsync();
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
                machine.BoughtCount = await purchaseStatsGrain.GetCountAsync();
                machine.BoughtAmount = await purchaseStatsGrain.GetAmountAsync();
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
            var snackInGrain = await snackGrain.GetStateAsync();
            snackNamePicture = (snackInGrain.Name, snackInGrain.PictureUrl);
            _snackNamePictureCache.TryAdd(snackId, snackNamePicture);
        }
        return snackNamePicture;
    }

    #endregion

}

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
            case MachineBoughtCountUpdatedEvent machineEvent:
                return ApplyEventAsync(machineEvent);
            case MachineBoughtAmountUpdatedEvent machineEvent:
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
            var machineInfo = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machineInfo == null)
            {
                machineInfo = new MachineInfo
                              {
                                  Id = machineEvent.MachineId,
                                  MoneyInside = machineEvent.MoneyInside.ToProjection(),
                                  Slots = await Task.WhenAll(machineEvent.Slots.Select(slot => slot.ToProjection(GetSnackNameAndPictureUrlAsync))),
                                  SlotsCount = machineEvent.SlotsCount,
                                  SnackCount = machineEvent.SnackCount,
                                  SnackQuantity = machineEvent.SnackQuantity,
                                  SnackAmount = machineEvent.SnackAmount,
                                  CreatedAt = machineEvent.OperatedAt,
                                  CreatedBy = machineEvent.OperatedBy,
                                  Version = machineEvent.Version
                              };
                _dbContext.Machines.Add(machineInfo);
            }
            if (_dbContext.Entry(machineInfo).State != EntityState.Added)
            {
                _logger.LogWarning("Apply MachineInitializedEvent: Machine {MachineId} is already in the database. Try to execute full update...", machineEvent.MachineId);
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
            var machineInfo = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineRemovedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machineInfo.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineRemovedEvent: Machine {MachineId} version {Version}) in the database should be {MachineVersion}. Try to execute full update...", machineEvent.MachineId, machineInfo.Version, machineEvent.Version - 1);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machineInfo.MoneyInside = machineEvent.MoneyInside.ToProjection(machineInfo.MoneyInside);
            machineInfo.AmountInTransaction = machineEvent.AmountInTransaction;
            machineInfo.Slots = await Task.WhenAll(machineEvent.Slots.Select(slot => slot.ToProjection(GetSnackNameAndPictureUrlAsync, machineInfo.Slots.FirstOrDefault(ms => ms.MachineId == slot.MachineId && ms.Position == slot.Position))));
            machineInfo.SlotsCount = machineEvent.SlotsCount;
            machineInfo.SnackCount = machineEvent.SnackCount;
            machineInfo.SnackQuantity = machineEvent.SnackQuantity;
            machineInfo.SnackAmount = machineEvent.SnackAmount;
            machineInfo.DeletedAt = machineEvent.OperatedAt;
            machineInfo.DeletedBy = machineEvent.OperatedBy;
            machineInfo.IsDeleted = true;
            machineInfo.Version = machineEvent.Version;
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
            var machineInfo = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineMoneyLoadedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machineInfo.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineMoneyLoadedEvent: Machine {MachineId} version {Version}) in the database should be {MachineVersion}. Try to execute full update...", machineEvent.MachineId, machineInfo.Version, machineEvent.Version - 1);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machineInfo.MoneyInside = machineEvent.MoneyInside.ToProjection(machineInfo.MoneyInside);
            machineInfo.LastModifiedAt = machineEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineEvent.OperatedBy;
            machineInfo.Version = machineEvent.Version;
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
            var machineInfo = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineMoneyUnloadedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machineInfo.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineMoneyUnloadedEvent: Machine {MachineId} version {Version}) in the database should be {MachineVersion}. Try to execute full update...", machineEvent.MachineId, machineInfo.Version, machineEvent.Version - 1);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machineInfo.MoneyInside = machineEvent.MoneyInside.ToProjection(machineInfo.MoneyInside);
            machineInfo.LastModifiedAt = machineEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineEvent.OperatedBy;
            machineInfo.Version = machineEvent.Version;
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
            var machineInfo = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineMoneyInsertedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machineInfo.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineMoneyInsertedEvent: Machine {MachineId} version {Version}) in the database should be {MachineVersion}. Try to execute full update...", machineEvent.MachineId, machineInfo.Version, machineEvent.Version - 1);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machineInfo.MoneyInside = machineEvent.MoneyInside.ToProjection(machineInfo.MoneyInside);
            machineInfo.AmountInTransaction = machineEvent.AmountInTransaction;
            machineInfo.LastModifiedAt = machineEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineEvent.OperatedBy;
            machineInfo.Version = machineEvent.Version;
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
            var machineInfo = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineMoneyReturnedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machineInfo.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineMoneyReturnedEvent: Machine {MachineId} version {Version}) in the database should be {MachineVersion}. Try to execute full update...", machineEvent.MachineId, machineInfo.Version, machineEvent.Version - 1);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machineInfo.MoneyInside = machineEvent.MoneyInside.ToProjection(machineInfo.MoneyInside);
            machineInfo.AmountInTransaction = machineEvent.AmountInTransaction;
            machineInfo.LastModifiedAt = machineEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineEvent.OperatedBy;
            machineInfo.Version = machineEvent.Version;
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
            var machineInfo = await _dbContext.Machines.Include(m => m.Slots).FirstOrDefaultAsync(m => m.Id == machineSnacksEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineSnacksLoadedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineSnacksEvent.MachineId);
                await ApplyFullUpdateAsync(machineSnacksEvent);
                return;
            }
            if (machineInfo.Version != machineSnacksEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineSnacksLoadedEvent: Machine {MachineId} version {MachineVersion}) in the database should be {Version}. Try to execute full update...", machineSnacksEvent.MachineId, machineInfo.Version,
                                   machineSnacksEvent.Version - 1);
                await ApplyFullUpdateAsync(machineSnacksEvent);
                return;
            }
            var slotInfo = machineInfo.Slots.FirstOrDefault(ms => ms.MachineId == machineSnacksEvent.Slot.MachineId && ms.Position == machineSnacksEvent.Slot.Position);
            if (slotInfo == null)
            {
                slotInfo = new MachineSlotInfo();
                machineInfo.Slots.Add(slotInfo);
            }
            await machineSnacksEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slotInfo);
            machineInfo.SlotsCount = machineSnacksEvent.SlotsCount;
            machineInfo.SnackCount = machineSnacksEvent.SnackCount;
            machineInfo.SnackQuantity = machineSnacksEvent.SnackQuantity;
            machineInfo.SnackAmount = machineSnacksEvent.SnackAmount;
            machineInfo.LastModifiedAt = machineSnacksEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineSnacksEvent.OperatedBy;
            machineInfo.Version = machineSnacksEvent.Version;
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
            var machineInfo = await _dbContext.Machines.Include(m => m.Slots).FirstOrDefaultAsync(m => m.Id == machineSnacksEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineSnacksUnloadedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineSnacksEvent.MachineId);
                await ApplyFullUpdateAsync(machineSnacksEvent);
                return;
            }
            if (machineInfo.Version != machineSnacksEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineSnacksUnloadedEvent: Machine {MachineId} version {Version}) in the database should be {MachineVersion}. Try to execute full update...", machineSnacksEvent.MachineId, machineInfo.Version,
                                   machineSnacksEvent.Version - 1);
                await ApplyFullUpdateAsync(machineSnacksEvent);
                return;
            }
            var slot = machineInfo.Slots.FirstOrDefault(ms => ms.MachineId == machineSnacksEvent.Slot.MachineId && ms.Position == machineSnacksEvent.Slot.Position);
            if (slot == null)
            {
                slot = new MachineSlotInfo();
                machineInfo.Slots.Add(slot);
            }
            await machineSnacksEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            machineInfo.SlotsCount = machineSnacksEvent.SlotsCount;
            machineInfo.SnackCount = machineSnacksEvent.SnackCount;
            machineInfo.SnackQuantity = machineSnacksEvent.SnackQuantity;
            machineInfo.SnackAmount = machineSnacksEvent.SnackAmount;
            machineInfo.LastModifiedAt = machineSnacksEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineSnacksEvent.OperatedBy;
            machineInfo.Version = machineSnacksEvent.Version;
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
            var machineInfo = await _dbContext.Machines.Include(m => m.Slots).FirstOrDefaultAsync(m => m.Id == machineSnackEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineSnackBoughtEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineSnackEvent.MachineId);
                await ApplyFullUpdateAsync(machineSnackEvent);
                return;
            }
            if (machineInfo.Version != machineSnackEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineSnackBoughtEvent: Machine {MachineId} version {Version}) in the database should be {MachineVersion}. Try to execute full update...", machineSnackEvent.MachineId, machineInfo.Version,
                                   machineSnackEvent.Version - 1);
                await ApplyFullUpdateAsync(machineSnackEvent);
                return;
            }
            machineInfo.AmountInTransaction = machineSnackEvent.AmountInTransaction;
            var slot = machineInfo.Slots.FirstOrDefault(ms => ms.MachineId == machineSnackEvent.Slot.MachineId && ms.Position == machineSnackEvent.Slot.Position);
            if (slot == null)
            {
                slot = new MachineSlotInfo();
                machineInfo.Slots.Add(slot);
            }
            await machineSnackEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slot);
            machineInfo.SnackQuantity = machineSnackEvent.SnackQuantity;
            machineInfo.SnackAmount = machineSnackEvent.SnackAmount;
            machineInfo.LastModifiedAt = machineSnackEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineSnackEvent.OperatedBy;
            machineInfo.Version = machineSnackEvent.Version;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyReturnedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineSnackEvent);
        }
    }

    private async Task ApplyEventAsync(MachineBoughtCountUpdatedEvent machineEvent)
    {
        try
        {
            var machineInfo = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineBoughtCountUpdatedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machineInfo.BoughtCount = machineEvent.BoughtCount;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineBoughtCountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineBoughtAmountUpdatedEvent machineEvent)
    {
        try
        {
            var machineInfo = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineBoughtAmountUpdatedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machineInfo.BoughtAmount = machineEvent.BoughtAmount;
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineBoughtAmountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
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
                var machine = await machineGrain.GetMachineAsync();
                var machineInfo = await _dbContext.Machines.Include(m => m.Slots).FirstOrDefaultAsync(m => m.Id == machineId);
                if (machine == null)
                {
                    if (machineInfo == null)
                    {
                        return;
                    }
                    _dbContext.Remove(machineInfo);
                    await _dbContext.SaveChangesAsync();
                    return;
                }
                if (machineInfo == null)
                {
                    machineInfo = new MachineInfo();
                    _dbContext.Machines.Add(machineInfo);
                }
                machineInfo = await machine.ToProjection(GetSnackNameAndPictureUrlAsync, machineInfo);
                machineInfo.Version = await machineGrain.GetVersionAsync();
                var statsOfPurchasesGrain = GrainFactory.GetGrain<IMachineStatsOfPurchasesGrain>(machineId);
                machineInfo.BoughtCount = await statsOfPurchasesGrain.GetBoughtCountAsync();
                machineInfo.BoughtAmount = await statsOfPurchasesGrain.GetBoughtAmountAsync();
                await _dbContext.SaveChangesAsync();
                return;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                retryNeeded = ++attempts <= 3;
                if (retryNeeded)
                {
                    _logger.LogWarning(ex, "ApplyFullUpdateAsync: DbUpdateConcurrencyException is occurred when try to write data to the database. Retrying {Attempts}...", attempts);
                    await Task.Delay(TimeSpan.FromSeconds(attempts));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApplyFullUpdateAsync: Exception is occurred when try to write data to the database");
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

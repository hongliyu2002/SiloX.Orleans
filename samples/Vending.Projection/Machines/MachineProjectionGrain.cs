﻿using System.Collections.Concurrent;
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
public sealed class MachineProjectionGrain : SubscriberPublisherGrainWithGuidKey<MachineEvent, MachineErrorEvent, MachineInfoEvent, MachineInfoErrorEvent>, IMachineProjectionGrain
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
    protected override string GetSubStreamNamespace()
    {
        return Constants.MachinesNamespace;
    }

    /// <inheritdoc />
    protected override string GetPubStreamNamespace()
    {
        return Constants.MachineInfosNamespace;
    }

    /// <inheritdoc />
    protected override string GetPubBroadcastStreamNamespace()
    {
        return Constants.MachineInfosBroadcastNamespace;
    }

    /// <inheritdoc />
    protected override Task HandLeEventAsync(MachineEvent domainEvent)
    {
        return domainEvent switch
               {
                   MachineInitializedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineDeletedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineUpdatedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineSlotAddedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineSlotRemovedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineMoneyLoadedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineMoneyUnloadedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineMoneyInsertedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineMoneyReturnedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineSnacksLoadedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineSnacksUnloadedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineSnackBoughtEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineBoughtCountUpdatedEvent machineEvent => ApplyEventAsync(machineEvent),
                   MachineBoughtAmountUpdatedEvent machineEvent => ApplyEventAsync(machineEvent),
                   _ => Task.CompletedTask
               };
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
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
        try
        {
            var machineInfo = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machineInfo == null)
            {
                machineInfo = new MachineInfo
                              {
                                  Id = machineEvent.MachineId,
                                  MoneyInside = machineEvent.MoneyInside.ToProjection(),
                                  Slots = (await Task.WhenAll(machineEvent.Slots.Select(slot => slot.ToProjection(GetSnackNameAndPictureUrlAsync)))).ToList(),
                                  SlotCount = machineEvent.SlotCount,
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
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineInitializedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 201, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineDeletedEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
        try
        {
            var machineInfo = await _dbContext.Machines.FindAsync(machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineDeletedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machineInfo.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineDeletedEvent: Machine {MachineId} version {Version}) in the database should be {MachineVersion}. Try to execute full update...", machineEvent.MachineId, machineInfo.Version, machineEvent.Version - 1);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machineInfo.DeletedAt = machineEvent.OperatedAt;
            machineInfo.DeletedBy = machineEvent.OperatedBy;
            machineInfo.IsDeleted = true;
            machineInfo.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineDeletedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 202, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineUpdatedEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
        try
        {
            var machineInfo = await _dbContext.Machines.Include(m => m.Slots).FirstOrDefaultAsync(m => m.Id == machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineUpdatedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machineInfo.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineUpdatedEvent: Machine {MachineId} version {MachineVersion}) in the database should be {Version}. Try to execute full update...", machineEvent.MachineId, machineInfo.Version, machineEvent.Version - 1);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machineInfo.MoneyInside = machineEvent.MoneyInside.ToProjection(machineInfo.MoneyInside);
            // Remove slots that are not in the new machine.
            foreach (var existingSlotInfo in machineInfo.Slots.Where(slot => machineEvent.Slots.All(ms => ms.Position != slot.Position)))
            {
                _dbContext.Remove(existingSlotInfo);
            }
            // Update or add slots.
            foreach (var newSlot in machineEvent.Slots)
            {
                var existingSlotInfo = machineInfo.Slots.SingleOrDefault(ms => ms.Position == newSlot.Position);
                if (existingSlotInfo == null)
                {
                    var newSlotInfo = await newSlot.ToProjection(GetSnackNameAndPictureUrlAsync);
                    machineInfo.Slots.Add(newSlotInfo);
                }
                else
                {
                    var newSlotInfo = await newSlot.ToProjection(GetSnackNameAndPictureUrlAsync);
                    _dbContext.Entry(existingSlotInfo).CurrentValues.SetValues(newSlotInfo);
                    existingSlotInfo.SnackPile = newSlotInfo.SnackPile;
                }
            }
            machineInfo.SlotCount = machineEvent.SlotCount;
            machineInfo.SnackCount = machineEvent.SnackCount;
            machineInfo.SnackQuantity = machineEvent.SnackQuantity;
            machineInfo.SnackAmount = machineEvent.SnackAmount;
            machineInfo.LastModifiedAt = machineEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineEvent.OperatedBy;
            machineInfo.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 201, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineSlotAddedEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
        try
        {
            var machineInfo = await _dbContext.Machines.Include(m => m.Slots).FirstOrDefaultAsync(m => m.Id == machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineSlotAddedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machineInfo.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineSlotAddedEvent: Machine {MachineId} version {MachineVersion}) in the database should be {Version}. Try to execute full update...", machineEvent.MachineId, machineInfo.Version, machineEvent.Version - 1);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            var slotInfo = machineInfo.Slots.FirstOrDefault(ms => ms.Position == machineEvent.Slot.Position);
            if (slotInfo != null)
            {
                _logger.LogWarning("Apply MachineSlotAddedEvent: Slot at position {Position} in machine {MachineId} is already in the database. Try to execute full update...", machineEvent.Slot.Position, machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            slotInfo = await machineEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync);
            machineInfo.Slots.Add(slotInfo);
            machineInfo.SlotCount = machineEvent.SlotCount;
            machineInfo.SnackCount = machineEvent.SnackCount;
            machineInfo.SnackQuantity = machineEvent.SnackQuantity;
            machineInfo.SnackAmount = machineEvent.SnackAmount;
            machineInfo.LastModifiedAt = machineEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineEvent.OperatedBy;
            machineInfo.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyReturnedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 203, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineSlotRemovedEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
        try
        {
            var machineInfo = await _dbContext.Machines.Include(m => m.Slots).FirstOrDefaultAsync(m => m.Id == machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineSlotRemovedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machineInfo.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineSlotRemovedEvent: Machine {MachineId} version {Version}) in the database should be {MachineVersion}. Try to execute full update...", machineEvent.MachineId, machineInfo.Version, machineEvent.Version - 1);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            var slotInfo = machineInfo.Slots.FirstOrDefault(ms => ms.Position == machineEvent.Slot.Position);
            if (slotInfo == null)
            {
                _logger.LogWarning("Apply MachineSlotRemovedEvent: Slot at position {Position} in machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.Slot.Position, machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machineInfo.Slots.Remove(slotInfo);
            machineInfo.SlotCount = machineEvent.SlotCount;
            machineInfo.SnackCount = machineEvent.SnackCount;
            machineInfo.SnackQuantity = machineEvent.SnackQuantity;
            machineInfo.SnackAmount = machineEvent.SnackAmount;
            machineInfo.LastModifiedAt = machineEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineEvent.OperatedBy;
            machineInfo.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineSlotRemovedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 204, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineMoneyLoadedEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
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
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyLoadedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 205, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineMoneyUnloadedEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
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
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyUnloadedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 206, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineMoneyInsertedEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
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
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyInsertedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 207, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineMoneyReturnedEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
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
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyReturnedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 208, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineSnacksLoadedEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
        try
        {
            var machineInfo = await _dbContext.Machines.Include(m => m.Slots).FirstOrDefaultAsync(m => m.Id == machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineSnacksLoadedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machineInfo.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineSnacksLoadedEvent: Machine {MachineId} version {MachineVersion}) in the database should be {Version}. Try to execute full update...", machineEvent.MachineId, machineInfo.Version, machineEvent.Version - 1);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            var slotInfo = machineInfo.Slots.FirstOrDefault(ms => ms.Position == machineEvent.Slot.Position);
            if (slotInfo == null)
            {
                _logger.LogWarning("Apply MachineSnacksLoadedEvent: Slot at position {Position} in machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.Slot.Position, machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            await machineEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slotInfo);
            machineInfo.SlotCount = machineEvent.SlotCount;
            machineInfo.SnackCount = machineEvent.SnackCount;
            machineInfo.SnackQuantity = machineEvent.SnackQuantity;
            machineInfo.SnackAmount = machineEvent.SnackAmount;
            machineInfo.LastModifiedAt = machineEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineEvent.OperatedBy;
            machineInfo.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyReturnedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 209, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineSnacksUnloadedEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
        try
        {
            var machineInfo = await _dbContext.Machines.Include(m => m.Slots).FirstOrDefaultAsync(m => m.Id == machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineSnacksUnloadedEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machineInfo.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineSnacksUnloadedEvent: Machine {MachineId} version {Version}) in the database should be {MachineVersion}. Try to execute full update...", machineEvent.MachineId, machineInfo.Version, machineEvent.Version - 1);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            var slotInfo = machineInfo.Slots.FirstOrDefault(ms => ms.Position == machineEvent.Slot.Position);
            if (slotInfo == null)
            {
                _logger.LogWarning("Apply MachineSnacksUnloadedEvent: Slot at position {Position} in machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.Slot.Position, machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            await machineEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slotInfo);
            machineInfo.SlotCount = machineEvent.SlotCount;
            machineInfo.SnackCount = machineEvent.SnackCount;
            machineInfo.SnackQuantity = machineEvent.SnackQuantity;
            machineInfo.SnackAmount = machineEvent.SnackAmount;
            machineInfo.LastModifiedAt = machineEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineEvent.OperatedBy;
            machineInfo.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineSnacksUnloadedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 210, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineSnackBoughtEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
        try
        {
            var machineInfo = await _dbContext.Machines.Include(m => m.Slots).FirstOrDefaultAsync(m => m.Id == machineEvent.MachineId);
            if (machineInfo == null)
            {
                _logger.LogWarning("Apply MachineSnackBoughtEvent: Machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            if (machineInfo.Version != machineEvent.Version - 1)
            {
                _logger.LogWarning("Apply MachineSnackBoughtEvent: Machine {MachineId} version {Version}) in the database should be {MachineVersion}. Try to execute full update...", machineEvent.MachineId, machineInfo.Version, machineEvent.Version - 1);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            machineInfo.AmountInTransaction = machineEvent.AmountInTransaction;
            var slotInfo = machineInfo.Slots.FirstOrDefault(ms => ms.Position == machineEvent.Slot.Position);
            if (slotInfo == null)
            {
                _logger.LogWarning("Apply MachineSnackBoughtEvent: Slot at position {Position} in machine {MachineId} does not exist in the database. Try to execute full update...", machineEvent.Slot.Position, machineEvent.MachineId);
                await ApplyFullUpdateAsync(machineEvent);
                return;
            }
            await machineEvent.Slot.ToProjection(GetSnackNameAndPictureUrlAsync, slotInfo);
            machineInfo.SnackQuantity = machineEvent.SnackQuantity;
            machineInfo.SnackAmount = machineEvent.SnackAmount;
            machineInfo.LastModifiedAt = machineEvent.OperatedAt;
            machineInfo.LastModifiedBy = machineEvent.OperatedBy;
            machineInfo.Version = machineEvent.Version;
            await _dbContext.SaveChangesAsync();
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineMoneyReturnedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 211, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineBoughtCountUpdatedEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
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
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineBoughtCountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 221, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyEventAsync(MachineBoughtAmountUpdatedEvent machineEvent)
    {
        var operatedAt = DateTimeOffset.UtcNow;
        var operatedBy = $"System/{GetType().Name}";
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
            await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply MachineBoughtAmountUpdatedEvent: Exception is occurred when try to write data to the database. Try to execute full update...");
            await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 222, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
            await ApplyFullUpdateAsync(machineEvent);
        }
    }

    private async Task ApplyFullUpdateAsync(MachineEvent machineEvent)
    {
        var attempts = 0;
        bool retryNeeded;
        do
        {
            var operatedAt = DateTimeOffset.UtcNow;
            var operatedBy = $"System/{GetType().Name}";
            try
            {
                var machineId = machineEvent.MachineId;
                var machineGrain = GrainFactory.GetGrain<IMachineGrain>(machineId);
                var machine = await machineGrain.GetMachineAsync();
                var machineInfo = await _dbContext.Machines.Include(m => m.MoneyInside).Include(m => m.Slots).ThenInclude(ms => ms.SnackPile).FirstOrDefaultAsync(m => m.Id == machineId);
                if (machine == null || machine.Id == Guid.Empty)
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
                machineInfo.Id = machine.Id;
                machineInfo.MoneyInside = machine.MoneyInside.ToProjection(machineInfo.MoneyInside);
                machineInfo.AmountInTransaction = machine.AmountInTransaction;
                // Remove slots that are not in the new machine.
                foreach (var existingSlotInfo in machineInfo.Slots.Where(slot => machine.Slots.All(ms => ms.Position != slot.Position)))
                {
                    _dbContext.Remove(existingSlotInfo);
                }
                // Update or add slots.
                foreach (var newSlot in machine.Slots)
                {
                    var existingSlotInfo = machineInfo.Slots.SingleOrDefault(ms => ms.Position == newSlot.Position);
                    if (existingSlotInfo == null)
                    {
                        var newSlotInfo = await newSlot.ToProjection(GetSnackNameAndPictureUrlAsync);
                        machineInfo.Slots.Add(newSlotInfo);
                    }
                    else
                    {
                        var newSlotInfo = await newSlot.ToProjection(GetSnackNameAndPictureUrlAsync);
                        _dbContext.Entry(existingSlotInfo).CurrentValues.SetValues(newSlotInfo);
                        existingSlotInfo.SnackPile = newSlotInfo.SnackPile;
                    }
                }
                machineInfo.CreatedAt = machine.CreatedAt;
                machineInfo.LastModifiedAt = machine.LastModifiedAt;
                machineInfo.DeletedAt = machine.DeletedAt;
                machineInfo.CreatedBy = machine.CreatedBy;
                machineInfo.LastModifiedBy = machine.LastModifiedBy;
                machineInfo.DeletedBy = machine.DeletedBy;
                machineInfo.IsDeleted = machine.IsDeleted;
                machineInfo.SlotCount = machine.SlotCount;
                machineInfo.SnackCount = machine.SnackCount;
                machineInfo.SnackQuantity = machine.SnackQuantity;
                machineInfo.SnackAmount = machine.SnackAmount;
                machineInfo.Version = await machineGrain.GetVersionAsync();
                var statsOfPurchasesGrain = GrainFactory.GetGrain<IMachineStatsOfPurchasesGrain>(machineId);
                machineInfo.BoughtCount = await statsOfPurchasesGrain.GetBoughtCountAsync();
                machineInfo.BoughtAmount = await statsOfPurchasesGrain.GetBoughtAmountAsync();
                await _dbContext.SaveChangesAsync();
                await PublishAsync(new MachineInfoSavedEvent(machineInfo.Id, machineInfo.Version, machineInfo, machineEvent.TraceId, operatedAt, operatedBy));
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
                await PublishErrorAsync(new MachineInfoErrorEvent(machineEvent.MachineId, machineEvent.Version, 200, new[] { ex.Message }, machineEvent.TraceId, operatedAt, operatedBy));
                retryNeeded = false;
            }
        }
        while (retryNeeded);
    }

    #region Get SnackInfo Name And Picture Url

    private readonly ConcurrentDictionary<Guid, (string SnackName, string? SnackPictureUrl)> _snackNamePictureCache = new();

    private async Task<(string SnackName, string? SnackPictureUrl)> GetSnackNameAndPictureUrlAsync(Guid snackId)
    {
        if (_snackNamePictureCache.TryGetValue(snackId, out var snackNamePicture))
        {
            return snackNamePicture;
        }
        var snackGrain = GrainFactory.GetGrain<ISnackGrain>(snackId);
        var snackInGrain = await snackGrain.GetSnackAsync();
        snackNamePicture = (snackInGrain.Name, snackInGrain.PictureUrl);
        _snackNamePictureCache.TryAdd(snackId, snackNamePicture);
        return snackNamePicture;
    }

    #endregion

}
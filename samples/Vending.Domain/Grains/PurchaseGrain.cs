using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Microsoft.Extensions.Logging;
using Orleans.FluentResults;
using Orleans.Providers;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Commands;
using Vending.Domain.Abstractions.Events;
using Vending.Domain.Abstractions.Grains;
using Vending.Domain.Abstractions.States;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Grains;

/// <summary>
///     Represents a grain that manages the state of a snack purchase of a snack machine.
/// </summary>
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public sealed class PurchaseGrain : StatefulGrainWithStringKey<Purchase, PurchaseEvent, PurchaseErrorEvent>, IPurchaseGrain
{
    private readonly DomainDbContext _dbContext;
    private readonly ILogger<PurchaseGrain> _logger;

    /// <inheritdoc />
    public PurchaseGrain(DomainDbContext dbContext, ILogger<PurchaseGrain> logger) : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    /// <inheritdoc />
    protected override string GetStreamNamespace()
    {
        return Constants.PurchasesNamespace;
    }

    /// <inheritdoc />
    protected override string GetBroadcastStreamNamespace()
    {
        return Constants.PurchasesBroadcastNamespace;
    }

    /// <inheritdoc />
    public Task<Purchase> GetStateAsync()
    {
        return Task.FromResult(State);
    }

    private Result ValidateInitialize(PurchaseInitializeCommand command)
    {
        var purchaseId = this.GetPrimaryKeyString();
        return Result.Ok()
                     .Verify(State.IsInitialized == false, $"Purchase {purchaseId} is already initialized.")
                     .Verify($"{command.MachineId}/{command.Position}/{command.SnackId}" == purchaseId, $"Purchase {purchaseId} is not owned by the same snack machine {command.MachineId} and slot {command.Position} and snack {command.SnackId}.")
                     .Verify(command.OperatedBy.IsNotNullOrWhiteSpace(), "Operator should not be empty.");
    }

    /// <inheritdoc />
    public Task<bool> CanInitializeAsync(PurchaseInitializeCommand command)
    {
        return Task.FromResult(ValidateInitialize(command).IsSuccess);
    }

    /// <inheritdoc />
    public Task<Result> InitializeAsync(PurchaseInitializeCommand command)
    {
        return ValidateInitialize(command)
              .TapErrorTryAsync(errors => PublishErrorAsync(new PurchaseErrorEvent(command.MachineId, command.Position, command.SnackId, 1001, errors.ToReasons(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)))
              .MapTryAsync(() => ApplyAsync(command))
              .MapTryAsync(PersistAsync)
              .MapTryIfAsync(persisted => persisted,
                             () => PublishAsync(new PurchaseInitializedEvent(State.MachineId, State.Position, State.SnackId, State.BoughtPrice, command.TraceId, State.BoughtAt ?? DateTimeOffset.UtcNow, State.BoughtBy ?? command.OperatedBy)));
    }

    #region Persistence

    private Task ApplyAsync(PurchaseInitializeCommand command)
    {
        State.MachineId = command.MachineId;
        State.Position = command.Position;
        State.SnackId = command.SnackId;
        State.BoughtPrice = command.BoughtPrice;
        State.BoughtAt = command.OperatedAt;
        State.BoughtBy = command.OperatedBy;
        return WriteStateAsync();
    }

    private async Task<bool> PersistAsync()
    {
        try
        {
            var purchaseInGrain = State;
            var purchase = await _dbContext.Purchases.FindAsync(purchaseInGrain.MachineId, purchaseInGrain.Position, purchaseInGrain.SnackId);
            if (purchase == null)
            {
                purchase = new Purchase();
                _dbContext.Purchases.Add(purchase);
            }
            purchase.MachineId = purchaseInGrain.MachineId;
            purchase.Position = purchaseInGrain.Position;
            purchase.SnackId = purchaseInGrain.SnackId;
            purchase.BoughtPrice = purchaseInGrain.BoughtPrice;
            purchase.BoughtAt = purchaseInGrain.BoughtAt;
            purchase.BoughtBy = purchaseInGrain.BoughtBy;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PersistAsync: Exception is occurred when try to write data to the database.");
            return false;
        }
    }

    #endregion

}

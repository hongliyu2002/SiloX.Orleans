using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Orleans.FluentResults;
using Orleans.Providers;
using SiloX.Domain.Abstractions;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions;
using Vending.Domain.Abstractions.Purchases;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Purchases;

/// <summary>
///     Represents a grain that manages the state of a snack purchase of a machine.
/// </summary>
[StorageProvider(ProviderName = Constants.GrainStorageName)]
public sealed class PurchaseGrain : StatefulGrainWithStringKey<Purchase, PurchaseEvent, PurchaseErrorEvent>, IPurchaseGrain
{
    private readonly DomainDbContext _dbContext;

    /// <inheritdoc />
    public PurchaseGrain(DomainDbContext dbContext)
        : base(Constants.StreamProviderName)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
    }

    /// <inheritdoc />
    protected override string GetPubStreamNamespace()
    {
        return Constants.PurchasesNamespace;
    }

    /// <inheritdoc />
    protected override string GetPubBroadcastStreamNamespace()
    {
        return Constants.PurchasesBroadcastNamespace;
    }

    /// <inheritdoc />
    public Task<Purchase> GetPurchaseAsync()
    {
        return Task.FromResult(State);
    }

    private Result ValidateInitialize(PurchaseInitializeCommand command)
    {
        var purchaseId = this.GetPrimaryKey();
        return Result.Ok()
                     .Verify(State.IsInitialized == false, $"Purchase {purchaseId} is already initialized.")
                     .Verify(command.BoughtPrice >= 0, "Bought price should be greater than or equals to zero.")
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
              .MapTryAsync(() => ApplyAsync(command))
              .MapTryAsync(PersistAsync)
              .TapTryAsync(() => PublishAsync(new PurchaseInitializedEvent(State.Id, State.MachineId, State.Position, State.SnackId, State.BoughtPrice, command.TraceId, State.BoughtAt ?? DateTimeOffset.UtcNow, State.BoughtBy ?? command.OperatedBy)))
              .TapErrorTryAsync(errors => PublishErrorAsync(new PurchaseErrorEvent(this.GetPrimaryKey(), command.MachineId, command.Position, command.SnackId, 301, errors.ToListMessages(), command.TraceId, DateTimeOffset.UtcNow, command.OperatedBy)));
    }

    #region Persistence

    private Task ApplyAsync(PurchaseInitializeCommand command)
    {
        State.Id = command.PurchaseId;
        State.MachineId = command.MachineId;
        State.Position = command.Position;
        State.SnackId = command.SnackId;
        State.BoughtPrice = command.BoughtPrice;
        State.BoughtAt = command.OperatedAt;
        State.BoughtBy = command.OperatedBy;
        return WriteStateAsync();
    }

    private async Task PersistAsync()
    {
        var purchase = await _dbContext.Purchases.FirstOrDefaultAsync(p => p.Id == State.Id);
        if (purchase != null)
        {
            _dbContext.Entry(purchase).CurrentValues.SetValues(State);
        }
        else
        {
            _dbContext.Purchases.Add(State);
        }
        await _dbContext.SaveChangesAsync();
    }

    #endregion

}

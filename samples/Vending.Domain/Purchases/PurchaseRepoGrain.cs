﻿using Fluxera.Extensions.Common;
using Fluxera.Guards;
using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Purchases;

namespace Vending.Domain.Purchases;

[StatelessWorker]
[Reentrant]
public sealed class PurchaseRepoGrain : Grain, IPurchaseRepoGrain
{
    private readonly IGuidGenerator _guidGenerator;

    /// <inheritdoc />
    public PurchaseRepoGrain(IGuidGenerator guidGenerator)
    {
        _guidGenerator = Guard.Against.Null(guidGenerator, nameof(guidGenerator));
    }

    /// <inheritdoc />
    public Task<Result<Purchase>> CreateAsync(PurchaseRepoCreateCommand command)
    {
        var purchaseId = _guidGenerator.Create();
        IPurchaseGrain grain = null!;
        return Result.Ok()
                     .MapTry(() => grain = GrainFactory.GetGrain<IPurchaseGrain>(purchaseId))
                     .BindTryAsync(() => grain.InitializeAsync(new PurchaseInitializeCommand(purchaseId, command.MachineId, command.Position, command.SnackId, command.BoughtPrice, command.TraceId, command.OperatedAt, command.OperatedBy)))
                     .MapAsync(() => grain.GetPurchaseAsync());
    }
}

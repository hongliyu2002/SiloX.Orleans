using Fluxera.Extensions.Common;
using Fluxera.Guards;
using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Purchases;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Purchases;

[StatelessWorker]
public sealed class PurchaseRepoGrain : Grain, IPurchaseRepoGrain
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly DomainDbContext _dbContext;

    /// <inheritdoc />
    public PurchaseRepoGrain(DomainDbContext dbContext, IGuidGenerator guidGenerator)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _guidGenerator = Guard.Against.Null(guidGenerator, nameof(guidGenerator));
    }

    /// <inheritdoc />
    public Task<Result<Guid>> CreateAsync(PurchaseRepoCreateCommand command)
    {
        var purchaseId = _guidGenerator.Create();
        return Result.Ok()
                     .MapTry(() => GrainFactory.GetGrain<IPurchaseGrain>(purchaseId))
                     .MapTryAsync(grain => grain.InitializeAsync(new PurchaseInitializeCommand(purchaseId, command.MachineId, command.Position, command.SnackId, command.BoughtPrice, command.TraceId, command.OperatedAt, command.OperatedBy)))
                     .MapAsync(() => purchaseId);
    }
}

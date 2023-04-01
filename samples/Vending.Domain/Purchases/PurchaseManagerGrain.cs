using System.Collections.Immutable;
using System.Linq.Dynamic.Core;
using Fluxera.Extensions.Common;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Orleans.Concurrency;
using Orleans.FluentResults;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions.Purchases;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Purchases;

[StatelessWorker]
public sealed class PurchaseManagerGrain : Grain, IPurchaseManagerGrain
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly DomainDbContext _dbContext;

    /// <inheritdoc />
    public PurchaseManagerGrain(DomainDbContext dbContext, IGuidGenerator guidGenerator)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _guidGenerator = Guard.Against.Null(guidGenerator, nameof(guidGenerator));
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<Purchase>>> ListAsync(PurchaseManagerListQuery query)
    {
        var machineId = query.MachineId;
        var snackId = query.SnackId;
        var boughtPriceRangeStart = query.BoughtPriceRange?.Start;
        var boughtPriceRangeEnd = query.BoughtPriceRange?.End;
        var boughtAtRangeStart = query.BoughtAtRange?.Start;
        var boughtAtRangeEnd = query.BoughtAtRange?.End;
        var boughtBy = query.BoughtBy;
        var sortings = query.Sortings?.ToSortStrinng();
        return Result.Ok(_dbContext.Purchases.AsNoTracking())
                     .MapIf(machineId != null, purchases => purchases.Where(p => p.MachineId == machineId))
                     .MapIf(snackId != null, purchases => purchases.Where(p => p.SnackId == snackId))
                     .MapIf(boughtPriceRangeStart != null, purchases => purchases.Where(p => p.BoughtPrice >= boughtPriceRangeStart))
                     .MapIf(boughtPriceRangeEnd != null, purchases => purchases.Where(p => p.BoughtPrice < boughtPriceRangeEnd))
                     .MapIf(boughtAtRangeStart != null, purchases => purchases.Where(p => p.BoughtAt >= boughtAtRangeStart))
                     .MapIf(boughtAtRangeEnd != null, purchases => purchases.Where(p => p.BoughtAt < boughtAtRangeEnd))
                     .MapIf(boughtBy != null, purchases => purchases.Where(p => p.BoughtBy == boughtBy))
                     .MapIf(sortings != null, purchases => purchases.OrderBy(sortings!))
                     .MapTryAsync(purchases => purchases.ToImmutableListAsync());
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<Purchase>>> PagedListAsync(PurchaseManagerPagedListQuery query)
    {
        var machineId = query.MachineId;
        var snackId = query.SnackId;
        var boughtPriceRangeStart = query.BoughtPriceRange?.Start;
        var boughtPriceRangeEnd = query.BoughtPriceRange?.End;
        var boughtAtRangeStart = query.BoughtAtRange?.Start;
        var boughtAtRangeEnd = query.BoughtAtRange?.End;
        var boughtBy = query.BoughtBy;
        var sortings = query.Sortings?.ToSortStrinng();
        var skipCount = query.SkipCount;
        var maxResultCount = query.MaxResultCount;
        return Result.Ok(_dbContext.Purchases.AsNoTracking())
                     .MapIf(machineId != null, purchases => purchases.Where(p => p.MachineId == machineId))
                     .MapIf(snackId != null, purchases => purchases.Where(p => p.SnackId == snackId))
                     .MapIf(boughtPriceRangeStart != null, purchases => purchases.Where(p => p.BoughtPrice >= boughtPriceRangeStart))
                     .MapIf(boughtPriceRangeEnd != null, purchases => purchases.Where(p => p.BoughtPrice < boughtPriceRangeEnd))
                     .MapIf(boughtAtRangeStart != null, purchases => purchases.Where(p => p.BoughtAt >= boughtAtRangeStart))
                     .MapIf(boughtAtRangeEnd != null, purchases => purchases.Where(p => p.BoughtAt < boughtAtRangeEnd))
                     .MapIf(boughtBy != null, purchases => purchases.Where(p => p.BoughtBy == boughtBy))
                     .MapIf(sortings != null, purchases => purchases.OrderBy(sortings!))
                     .MapIf(skipCount is >= 0, purchases => purchases.Skip(skipCount!.Value))
                     .MapIf(maxResultCount is >= 1, purchases => purchases.Take(maxResultCount!.Value))
                     .MapTryAsync(purchases => purchases.ToImmutableListAsync());
    }

    /// <inheritdoc />
    public Task<Result<Guid>> CreateAsync(PurchaseManagerCreateCommand command)
    {
        var purchaseId = _guidGenerator.Create();
        return Result.Ok()
                     .MapTry(() => GrainFactory.GetGrain<IPurchaseGrain>(purchaseId))
                     .MapTryAsync(grain => grain.InitializeAsync(new PurchaseInitializeCommand(purchaseId, command.MachineId, command.Position, command.SnackId, command.BoughtPrice, command.TraceId, command.OperatedAt, command.OperatedBy)))
                     .MapAsync(() => purchaseId);
    }
}

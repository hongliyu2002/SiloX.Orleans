using System.Collections.Immutable;
using System.Linq.Dynamic.Core;
using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Orleans.Concurrency;
using Orleans.FluentResults;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Projection.Abstractions.Purchases;
using Vending.Projection.EntityFrameworkCore;

namespace Vending.Projection.Purchases;

[StatelessWorker]
[Reentrant]
public class PurchaseRetrieverGrain : Grain, IPurchaseRetrieverGrain
{
    private readonly ProjectionDbContext _dbContext;

    /// <inheritdoc />
    public PurchaseRetrieverGrain(ProjectionDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<PurchaseInfo>>> ListAsync(PurchaseRetrieverListQuery query)
    {
        var machineId = query.MachineId;
        var snackId = query.SnackId;
        var boughtPriceRangeStart = query.BoughtPriceRange?.Start;
        var boughtPriceRangeEnd = query.BoughtPriceRange?.End;
        var boughtAtRangeStart = query.BoughtAtRange?.Start;
        var boughtAtRangeEnd = query.BoughtAtRange?.End;
        var boughtBy = query.BoughtBy;
        var sortings = query.Sortings?.ToOrderByString();
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
    public Task<Result<ImmutableList<PurchaseInfo>>> PagedListAsync(PurchaseRetrieverPagedListQuery query)
    {
        var machineId = query.MachineId;
        var snackId = query.SnackId;
        var boughtPriceRangeStart = query.BoughtPriceRange?.Start;
        var boughtPriceRangeEnd = query.BoughtPriceRange?.End;
        var boughtAtRangeStart = query.BoughtAtRange?.Start;
        var boughtAtRangeEnd = query.BoughtAtRange?.End;
        var boughtBy = query.BoughtBy;
        var sortings = query.Sortings?.ToOrderByString();
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
    public Task<Result<ImmutableList<PurchaseInfo>>> SearchingListAsync(PurchaseRetrieverSearchingListQuery query)
    {
        var searchTerm = query.SearchTerm;
        var machineId = query.MachineId;
        var snackId = query.SnackId;
        var boughtPriceRangeStart = query.BoughtPriceRange?.Start;
        var boughtPriceRangeEnd = query.BoughtPriceRange?.End;
        var boughtAtRangeStart = query.BoughtAtRange?.Start;
        var boughtAtRangeEnd = query.BoughtAtRange?.End;
        var boughtBy = query.BoughtBy;
        var sortings = query.Sortings?.ToOrderByString();
        return Result.Ok(_dbContext.Purchases.AsNoTracking())
                     .MapIf(searchTerm.IsNotNullOrEmpty(), purchases => purchases.Where(p => EF.Functions.Like(p.SnackName, $"%{searchTerm}%")))
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
    public Task<Result<ImmutableList<PurchaseInfo>>> SearchingPagedListAsync(PurchaseRetrieverSearchingPagedListQuery query)
    {
        var searchTerm = query.SearchTerm;
        var machineId = query.MachineId;
        var snackId = query.SnackId;
        var boughtPriceRangeStart = query.BoughtPriceRange?.Start;
        var boughtPriceRangeEnd = query.BoughtPriceRange?.End;
        var boughtAtRangeStart = query.BoughtAtRange?.Start;
        var boughtAtRangeEnd = query.BoughtAtRange?.End;
        var boughtBy = query.BoughtBy;
        var sortings = query.Sortings?.ToOrderByString();
        var skipCount = query.SkipCount;
        var maxResultCount = query.MaxResultCount;
        return Result.Ok(_dbContext.Purchases.AsNoTracking())
                     .MapIf(searchTerm.IsNotNullOrEmpty(), purchases => purchases.Where(p => EF.Functions.Like(p.SnackName, $"%{searchTerm}%")))
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
}

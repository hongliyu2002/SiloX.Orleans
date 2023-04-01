using System.Collections.Immutable;
using System.Linq.Dynamic.Core;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Orleans.Concurrency;
using Orleans.FluentResults;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Projection.Abstractions.Purchases;
using Vending.Projection.EntityFrameworkCore;

namespace Vending.Projection.Purchases;

[StatelessWorker]
public class PurchaseProjectionManagerGrain : Grain, IPurchaseProjectionManagerGrain
{
    private readonly ProjectionDbContext _dbContext;

    /// <inheritdoc />
    public PurchaseProjectionManagerGrain(ProjectionDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<Purchase>>> ListAsync(PurchaseProjectionManagerListQuery query)
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
    public Task<Result<ImmutableList<Purchase>>> PagedListAsync(PurchaseProjectionManagerPagedListQuery query)
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
    public Task<Result<ImmutableList<Purchase>>> SearchingListAsync(PurchaseProjectionManagerSearchingListQuery query)
    {
        var searchCriteria = query.SearchCriteria;
        var machineId = query.MachineId;
        var snackId = query.SnackId;
        var boughtPriceRangeStart = query.BoughtPriceRange?.Start;
        var boughtPriceRangeEnd = query.BoughtPriceRange?.End;
        var boughtAtRangeStart = query.BoughtAtRange?.Start;
        var boughtAtRangeEnd = query.BoughtAtRange?.End;
        var boughtBy = query.BoughtBy;
        var sortings = query.Sortings?.ToSortStrinng();
        return Result.Ok(_dbContext.Purchases.AsNoTracking())
                     .MapIf(searchCriteria != null, purchases => purchases.Where(p => p.SnackName.Contains(searchCriteria!)))
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
    public Task<Result<ImmutableList<Purchase>>> SearchingPagedListAsync(PurchaseProjectionManagerSearchingPagedListQuery query)
    {
        var searchCriteria = query.SearchCriteria;
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
                     .MapIf(searchCriteria != null, purchases => purchases.Where(p => p.SnackName.Contains(searchCriteria!)))
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

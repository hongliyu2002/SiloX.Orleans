using System.Collections.Immutable;
using System.Linq.Dynamic.Core;
using Fluxera.Guards;
using Fluxera.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Orleans.Concurrency;
using Orleans.FluentResults;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Projection.Abstractions.Snacks;
using Vending.Projection.EntityFrameworkCore;

namespace Vending.Projection.Snacks;

[StatelessWorker]
[Reentrant]
public class SnackRetrieverGrain : Grain, ISnackRetrieverGrain
{
    private readonly ProjectionDbContext _dbContext;

    /// <inheritdoc />
    public SnackRetrieverGrain(ProjectionDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<Snack>>> ListAsync(SnackRetrieverListQuery retrieverListQuery)
    {
        var machineCountRangeStart = retrieverListQuery.MachineCountRange?.Start;
        var machineCountRangeEnd = retrieverListQuery.MachineCountRange?.End;
        var boughtCountRangeStart = retrieverListQuery.BoughtCountRange?.Start;
        var boughtCountRangeEnd = retrieverListQuery.BoughtCountRange?.End;
        var boughtAmountRangeStart = retrieverListQuery.BoughtAmountRange?.Start;
        var boughtAmountRangeEnd = retrieverListQuery.BoughtAmountRange?.End;
        var createdAtRangeStart = retrieverListQuery.CreatedAtRange?.Start;
        var createdAtRangeEnd = retrieverListQuery.CreatedAtRange?.End;
        var createdBy = retrieverListQuery.CreatedBy;
        var lastModifiedAtRangeStart = retrieverListQuery.LastModifiedAtRange?.Start;
        var lastModifiedAtRangeEnd = retrieverListQuery.LastModifiedAtRange?.End;
        var lastModifiedBy = retrieverListQuery.LastModifiedBy;
        var deletedAtRangeStart = retrieverListQuery.DeletedAtRange?.Start;
        var deletedAtRangeEnd = retrieverListQuery.DeletedAtRange?.End;
        var deletedBy = retrieverListQuery.DeletedBy;
        var isDeleted = retrieverListQuery.IsDeleted;
        var sortings = retrieverListQuery.Sortings?.ToSortStrinng();
        return Result.Ok(_dbContext.Snacks.AsNoTracking())
                     .MapIf(machineCountRangeStart != null, snacks => snacks.Where(s => s.MachineCount >= machineCountRangeStart))
                     .MapIf(machineCountRangeEnd != null, snacks => snacks.Where(s => s.MachineCount < machineCountRangeEnd))
                     .MapIf(boughtCountRangeStart != null, snacks => snacks.Where(s => s.BoughtCount >= boughtCountRangeStart))
                     .MapIf(boughtCountRangeEnd != null, snacks => snacks.Where(s => s.BoughtCount < boughtCountRangeEnd))
                     .MapIf(boughtAmountRangeStart != null, snacks => snacks.Where(s => s.BoughtAmount >= boughtAmountRangeStart))
                     .MapIf(boughtAmountRangeEnd != null, snacks => snacks.Where(s => s.BoughtAmount < boughtAmountRangeEnd))
                     .MapIf(createdAtRangeStart != null, snacks => snacks.Where(s => s.CreatedAt >= createdAtRangeStart))
                     .MapIf(createdAtRangeEnd != null, snacks => snacks.Where(s => s.CreatedAt < createdAtRangeEnd))
                     .MapIf(createdBy != null, snacks => snacks.Where(s => s.CreatedBy == createdBy))
                     .MapIf(lastModifiedAtRangeStart != null, snacks => snacks.Where(s => s.LastModifiedAt >= lastModifiedAtRangeStart))
                     .MapIf(lastModifiedAtRangeEnd != null, snacks => snacks.Where(s => s.LastModifiedAt < lastModifiedAtRangeEnd))
                     .MapIf(lastModifiedBy != null, snacks => snacks.Where(s => s.LastModifiedBy == lastModifiedBy))
                     .MapIf(deletedAtRangeStart != null, snacks => snacks.Where(s => s.DeletedAt >= deletedAtRangeStart))
                     .MapIf(deletedAtRangeEnd != null, snacks => snacks.Where(s => s.DeletedAt < deletedAtRangeEnd))
                     .MapIf(deletedBy != null, snacks => snacks.Where(s => s.DeletedBy == deletedBy))
                     .MapIf(isDeleted != null, snacks => snacks.Where(s => s.IsDeleted == isDeleted))
                     .MapIf(sortings != null, snacks => snacks.OrderBy(sortings!))
                     .MapTryAsync(snacks => snacks.ToImmutableListAsync());
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<Snack>>> PagedListAsync(SnackRetrieverPagedListQuery retrieverPagedListQuery)
    {
        var machineCountRangeStart = retrieverPagedListQuery.MachineCountRange?.Start;
        var machineCountRangeEnd = retrieverPagedListQuery.MachineCountRange?.End;
        var boughtCountRangeStart = retrieverPagedListQuery.BoughtCountRange?.Start;
        var boughtCountRangeEnd = retrieverPagedListQuery.BoughtCountRange?.End;
        var boughtAmountRangeStart = retrieverPagedListQuery.BoughtAmountRange?.Start;
        var boughtAmountRangeEnd = retrieverPagedListQuery.BoughtAmountRange?.End;
        var createdAtRangeStart = retrieverPagedListQuery.CreatedAtRange?.Start;
        var createdAtRangeEnd = retrieverPagedListQuery.CreatedAtRange?.End;
        var createdBy = retrieverPagedListQuery.CreatedBy;
        var lastModifiedAtRangeStart = retrieverPagedListQuery.LastModifiedAtRange?.Start;
        var lastModifiedAtRangeEnd = retrieverPagedListQuery.LastModifiedAtRange?.End;
        var lastModifiedBy = retrieverPagedListQuery.LastModifiedBy;
        var deletedAtRangeStart = retrieverPagedListQuery.DeletedAtRange?.Start;
        var deletedAtRangeEnd = retrieverPagedListQuery.DeletedAtRange?.End;
        var deletedBy = retrieverPagedListQuery.DeletedBy;
        var isDeleted = retrieverPagedListQuery.IsDeleted;
        var sortings = retrieverPagedListQuery.Sortings?.ToSortStrinng();
        var skipCount = retrieverPagedListQuery.SkipCount;
        var maxResultCount = retrieverPagedListQuery.MaxResultCount;
        return Result.Ok(_dbContext.Snacks.AsNoTracking())
                     .MapIf(machineCountRangeStart != null, snacks => snacks.Where(s => s.MachineCount >= machineCountRangeStart))
                     .MapIf(machineCountRangeEnd != null, snacks => snacks.Where(s => s.MachineCount < machineCountRangeEnd))
                     .MapIf(boughtCountRangeStart != null, snacks => snacks.Where(s => s.BoughtCount >= boughtCountRangeStart))
                     .MapIf(boughtCountRangeEnd != null, snacks => snacks.Where(s => s.BoughtCount < boughtCountRangeEnd))
                     .MapIf(boughtAmountRangeStart != null, snacks => snacks.Where(s => s.BoughtAmount >= boughtAmountRangeStart))
                     .MapIf(boughtAmountRangeEnd != null, snacks => snacks.Where(s => s.BoughtAmount < boughtAmountRangeEnd))
                     .MapIf(createdAtRangeStart != null, snacks => snacks.Where(s => s.CreatedAt >= createdAtRangeStart))
                     .MapIf(createdAtRangeEnd != null, snacks => snacks.Where(s => s.CreatedAt < createdAtRangeEnd))
                     .MapIf(createdBy != null, snacks => snacks.Where(s => s.CreatedBy == createdBy))
                     .MapIf(lastModifiedAtRangeStart != null, snacks => snacks.Where(s => s.LastModifiedAt >= lastModifiedAtRangeStart))
                     .MapIf(lastModifiedAtRangeEnd != null, snacks => snacks.Where(s => s.LastModifiedAt < lastModifiedAtRangeEnd))
                     .MapIf(lastModifiedBy != null, snacks => snacks.Where(s => s.LastModifiedBy == lastModifiedBy))
                     .MapIf(deletedAtRangeStart != null, snacks => snacks.Where(s => s.DeletedAt >= deletedAtRangeStart))
                     .MapIf(deletedAtRangeEnd != null, snacks => snacks.Where(s => s.DeletedAt < deletedAtRangeEnd))
                     .MapIf(deletedBy != null, snacks => snacks.Where(s => s.DeletedBy == deletedBy))
                     .MapIf(isDeleted != null, snacks => snacks.Where(s => s.IsDeleted == isDeleted))
                     .MapIf(sortings != null, snacks => snacks.OrderBy(sortings!))
                     .MapIf(skipCount is >= 0, snacks => snacks.Skip(skipCount!.Value))
                     .MapIf(maxResultCount is >= 1, snacks => snacks.Take(maxResultCount!.Value))
                     .MapTryAsync(snacks => snacks.ToImmutableListAsync());
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<Snack>>> SearchingListAsync(SnackRetrieverSearchingListQuery query)
    {
        var searchTerm = query.SearchTerm;
        var machineCountRangeStart = query.MachineCountRange?.Start;
        var machineCountRangeEnd = query.MachineCountRange?.End;
        var boughtCountRangeStart = query.BoughtCountRange?.Start;
        var boughtCountRangeEnd = query.BoughtCountRange?.End;
        var boughtAmountRangeStart = query.BoughtAmountRange?.Start;
        var boughtAmountRangeEnd = query.BoughtAmountRange?.End;
        var createdAtRangeStart = query.CreatedAtRange?.Start;
        var createdAtRangeEnd = query.CreatedAtRange?.End;
        var createdBy = query.CreatedBy;
        var lastModifiedAtRangeStart = query.LastModifiedAtRange?.Start;
        var lastModifiedAtRangeEnd = query.LastModifiedAtRange?.End;
        var lastModifiedBy = query.LastModifiedBy;
        var deletedAtRangeStart = query.DeletedAtRange?.Start;
        var deletedAtRangeEnd = query.DeletedAtRange?.End;
        var deletedBy = query.DeletedBy;
        var isDeleted = query.IsDeleted;
        var sortings = query.Sortings?.ToSortStrinng();
        return Result.Ok(_dbContext.Snacks.AsNoTracking())
                     .MapIf(searchTerm.IsNotNullOrEmpty(), snacks => snacks.Where(s => EF.Functions.Like(s.Name, $"%{searchTerm}%")))
                     .MapIf(machineCountRangeStart != null, snacks => snacks.Where(s => s.MachineCount >= machineCountRangeStart))
                     .MapIf(machineCountRangeEnd != null, snacks => snacks.Where(s => s.MachineCount < machineCountRangeEnd))
                     .MapIf(boughtCountRangeStart != null, snacks => snacks.Where(s => s.BoughtCount >= boughtCountRangeStart))
                     .MapIf(boughtCountRangeEnd != null, snacks => snacks.Where(s => s.BoughtCount < boughtCountRangeEnd))
                     .MapIf(boughtAmountRangeStart != null, snacks => snacks.Where(s => s.BoughtAmount >= boughtAmountRangeStart))
                     .MapIf(boughtAmountRangeEnd != null, snacks => snacks.Where(s => s.BoughtAmount < boughtAmountRangeEnd))
                     .MapIf(createdAtRangeStart != null, snacks => snacks.Where(s => s.CreatedAt >= createdAtRangeStart))
                     .MapIf(createdAtRangeEnd != null, snacks => snacks.Where(s => s.CreatedAt < createdAtRangeEnd))
                     .MapIf(createdBy != null, snacks => snacks.Where(s => s.CreatedBy == createdBy))
                     .MapIf(lastModifiedAtRangeStart != null, snacks => snacks.Where(s => s.LastModifiedAt >= lastModifiedAtRangeStart))
                     .MapIf(lastModifiedAtRangeEnd != null, snacks => snacks.Where(s => s.LastModifiedAt < lastModifiedAtRangeEnd))
                     .MapIf(lastModifiedBy != null, snacks => snacks.Where(s => s.LastModifiedBy == lastModifiedBy))
                     .MapIf(deletedAtRangeStart != null, snacks => snacks.Where(s => s.DeletedAt >= deletedAtRangeStart))
                     .MapIf(deletedAtRangeEnd != null, snacks => snacks.Where(s => s.DeletedAt < deletedAtRangeEnd))
                     .MapIf(deletedBy != null, snacks => snacks.Where(s => s.DeletedBy == deletedBy))
                     .MapIf(isDeleted != null, snacks => snacks.Where(s => s.IsDeleted == isDeleted))
                     .MapIf(sortings != null, snacks => snacks.OrderBy(sortings!))
                     .MapTryAsync(snacks => snacks.ToImmutableListAsync());
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<Snack>>> SearchingPagedListAsync(SnackRetrieverSearchingPagedListQuery query)
    {
        var searchTerm = query.SearchTerm;
        var machineCountRangeStart = query.MachineCountRange?.Start;
        var machineCountRangeEnd = query.MachineCountRange?.End;
        var boughtCountRangeStart = query.BoughtCountRange?.Start;
        var boughtCountRangeEnd = query.BoughtCountRange?.End;
        var boughtAmountRangeStart = query.BoughtAmountRange?.Start;
        var boughtAmountRangeEnd = query.BoughtAmountRange?.End;
        var createdAtRangeStart = query.CreatedAtRange?.Start;
        var createdAtRangeEnd = query.CreatedAtRange?.End;
        var createdBy = query.CreatedBy;
        var lastModifiedAtRangeStart = query.LastModifiedAtRange?.Start;
        var lastModifiedAtRangeEnd = query.LastModifiedAtRange?.End;
        var lastModifiedBy = query.LastModifiedBy;
        var deletedAtRangeStart = query.DeletedAtRange?.Start;
        var deletedAtRangeEnd = query.DeletedAtRange?.End;
        var deletedBy = query.DeletedBy;
        var isDeleted = query.IsDeleted;
        var sortings = query.Sortings?.ToSortStrinng();
        var skipCount = query.SkipCount;
        var maxResultCount = query.MaxResultCount;
        return Result.Ok(_dbContext.Snacks.AsNoTracking())
                     .MapIf(searchTerm.IsNotNullOrEmpty(), snacks => snacks.Where(s => EF.Functions.Like(s.Name, $"%{searchTerm}%")))
                     .MapIf(machineCountRangeStart != null, snacks => snacks.Where(s => s.MachineCount >= machineCountRangeStart))
                     .MapIf(machineCountRangeEnd != null, snacks => snacks.Where(s => s.MachineCount < machineCountRangeEnd))
                     .MapIf(boughtCountRangeStart != null, snacks => snacks.Where(s => s.BoughtCount >= boughtCountRangeStart))
                     .MapIf(boughtCountRangeEnd != null, snacks => snacks.Where(s => s.BoughtCount < boughtCountRangeEnd))
                     .MapIf(boughtAmountRangeStart != null, snacks => snacks.Where(s => s.BoughtAmount >= boughtAmountRangeStart))
                     .MapIf(boughtAmountRangeEnd != null, snacks => snacks.Where(s => s.BoughtAmount < boughtAmountRangeEnd))
                     .MapIf(createdAtRangeStart != null, snacks => snacks.Where(s => s.CreatedAt >= createdAtRangeStart))
                     .MapIf(createdAtRangeEnd != null, snacks => snacks.Where(s => s.CreatedAt < createdAtRangeEnd))
                     .MapIf(createdBy != null, snacks => snacks.Where(s => s.CreatedBy == createdBy))
                     .MapIf(lastModifiedAtRangeStart != null, snacks => snacks.Where(s => s.LastModifiedAt >= lastModifiedAtRangeStart))
                     .MapIf(lastModifiedAtRangeEnd != null, snacks => snacks.Where(s => s.LastModifiedAt < lastModifiedAtRangeEnd))
                     .MapIf(lastModifiedBy != null, snacks => snacks.Where(s => s.LastModifiedBy == lastModifiedBy))
                     .MapIf(deletedAtRangeStart != null, snacks => snacks.Where(s => s.DeletedAt >= deletedAtRangeStart))
                     .MapIf(deletedAtRangeEnd != null, snacks => snacks.Where(s => s.DeletedAt < deletedAtRangeEnd))
                     .MapIf(deletedBy != null, snacks => snacks.Where(s => s.DeletedBy == deletedBy))
                     .MapIf(isDeleted != null, snacks => snacks.Where(s => s.IsDeleted == isDeleted))
                     .MapIf(sortings != null, snacks => snacks.OrderBy(sortings!))
                     .MapIf(skipCount is >= 0, snacks => snacks.Skip(skipCount!.Value))
                     .MapIf(maxResultCount is >= 1, snacks => snacks.Take(maxResultCount!.Value))
                     .MapTryAsync(snacks => snacks.ToImmutableListAsync());
    }
}

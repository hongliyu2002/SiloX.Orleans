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
    public Task<Result<ImmutableList<SnackInfo>>> ListAsync(SnackRetrieverListQuery query)
    {
        var machineCountRangeStart = query.MachineCountRange?.Start;
        var machineCountRangeEnd = query.MachineCountRange?.End;
        var totalQuantityRangeStart = query.TotalQuantityRange?.Start;
        var totalQuantityRangeEnd = query.TotalQuantityRange?.End;
        var totalAmountRangeStart = query.TotalAmountRange?.Start;
        var totalAmountRangeEnd = query.TotalAmountRange?.End;
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
        var sortings = query.Sortings?.ToOrderByString();
        return Result.Ok(_dbContext.Snacks.AsNoTracking())
                     .MapIf(machineCountRangeStart != null, snacks => snacks.Where(s => s.MachineCount >= machineCountRangeStart))
                     .MapIf(machineCountRangeEnd != null, snacks => snacks.Where(s => s.MachineCount < machineCountRangeEnd))
                     .MapIf(totalQuantityRangeStart != null, snacks => snacks.Where(s => s.TotalQuantity >= totalQuantityRangeStart))
                     .MapIf(totalQuantityRangeEnd != null, snacks => snacks.Where(s => s.TotalQuantity < totalQuantityRangeEnd))
                     .MapIf(totalAmountRangeStart != null, snacks => snacks.Where(s => s.TotalAmount >= totalAmountRangeStart))
                     .MapIf(totalAmountRangeEnd != null, snacks => snacks.Where(s => s.TotalAmount < totalAmountRangeEnd))
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
    public Task<Result<ImmutableList<SnackInfo>>> PagedListAsync(SnackRetrieverPagedListQuery query)
    {
        var machineCountRangeStart = query.MachineCountRange?.Start;
        var machineCountRangeEnd = query.MachineCountRange?.End;
        var totalQuantityRangeStart = query.TotalQuantityRange?.Start;
        var totalQuantityRangeEnd = query.TotalQuantityRange?.End;
        var totalAmountRangeStart = query.TotalAmountRange?.Start;
        var totalAmountRangeEnd = query.TotalAmountRange?.End;
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
        var sortings = query.Sortings?.ToOrderByString();
        var skipCount = query.SkipCount;
        var maxResultCount = query.MaxResultCount;
        return Result.Ok(_dbContext.Snacks.AsNoTracking())
                     .MapIf(machineCountRangeStart != null, snacks => snacks.Where(s => s.MachineCount >= machineCountRangeStart))
                     .MapIf(machineCountRangeEnd != null, snacks => snacks.Where(s => s.MachineCount < machineCountRangeEnd))
                     .MapIf(totalQuantityRangeStart != null, snacks => snacks.Where(s => s.TotalQuantity >= totalQuantityRangeStart))
                     .MapIf(totalQuantityRangeEnd != null, snacks => snacks.Where(s => s.TotalQuantity < totalQuantityRangeEnd))
                     .MapIf(totalAmountRangeStart != null, snacks => snacks.Where(s => s.TotalAmount >= totalAmountRangeStart))
                     .MapIf(totalAmountRangeEnd != null, snacks => snacks.Where(s => s.TotalAmount < totalAmountRangeEnd))
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
    public Task<Result<ImmutableList<SnackInfo>>> SearchingListAsync(SnackRetrieverSearchingListQuery query)
    {
        var searchTerm = query.SearchTerm;
        var machineCountRangeStart = query.MachineCountRange?.Start;
        var machineCountRangeEnd = query.MachineCountRange?.End;
        var totalQuantityRangeStart = query.TotalQuantityRange?.Start;
        var totalQuantityRangeEnd = query.TotalQuantityRange?.End;
        var totalAmountRangeStart = query.TotalAmountRange?.Start;
        var totalAmountRangeEnd = query.TotalAmountRange?.End;
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
        var sortings = query.Sortings?.ToOrderByString();
        return Result.Ok(_dbContext.Snacks.AsNoTracking())
                     .MapIf(searchTerm.IsNotNullOrEmpty(), snacks => snacks.Where(s => EF.Functions.Like(s.Name, $"%{searchTerm}%")))
                     .MapIf(machineCountRangeStart != null, snacks => snacks.Where(s => s.MachineCount >= machineCountRangeStart))
                     .MapIf(machineCountRangeEnd != null, snacks => snacks.Where(s => s.MachineCount < machineCountRangeEnd))
                     .MapIf(totalQuantityRangeStart != null, snacks => snacks.Where(s => s.TotalQuantity >= totalQuantityRangeStart))
                     .MapIf(totalQuantityRangeEnd != null, snacks => snacks.Where(s => s.TotalQuantity < totalQuantityRangeEnd))
                     .MapIf(totalAmountRangeStart != null, snacks => snacks.Where(s => s.TotalAmount >= totalAmountRangeStart))
                     .MapIf(totalAmountRangeEnd != null, snacks => snacks.Where(s => s.TotalAmount < totalAmountRangeEnd))
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
    public Task<Result<ImmutableList<SnackInfo>>> SearchingPagedListAsync(SnackRetrieverSearchingPagedListQuery query)
    {
        var searchTerm = query.SearchTerm;
        var machineCountRangeStart = query.MachineCountRange?.Start;
        var machineCountRangeEnd = query.MachineCountRange?.End;
        var totalQuantityRangeStart = query.TotalQuantityRange?.Start;
        var totalQuantityRangeEnd = query.TotalQuantityRange?.End;
        var totalAmountRangeStart = query.TotalAmountRange?.Start;
        var totalAmountRangeEnd = query.TotalAmountRange?.End;
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
        var sortings = query.Sortings?.ToOrderByString();
        var skipCount = query.SkipCount;
        var maxResultCount = query.MaxResultCount;
        return Result.Ok(_dbContext.Snacks.AsNoTracking())
                     .MapIf(searchTerm.IsNotNullOrEmpty(), snacks => snacks.Where(s => EF.Functions.Like(s.Name, $"%{searchTerm}%")))
                     .MapIf(machineCountRangeStart != null, snacks => snacks.Where(s => s.MachineCount >= machineCountRangeStart))
                     .MapIf(machineCountRangeEnd != null, snacks => snacks.Where(s => s.MachineCount < machineCountRangeEnd))
                     .MapIf(totalQuantityRangeStart != null, snacks => snacks.Where(s => s.TotalQuantity >= totalQuantityRangeStart))
                     .MapIf(totalQuantityRangeEnd != null, snacks => snacks.Where(s => s.TotalQuantity < totalQuantityRangeEnd))
                     .MapIf(totalAmountRangeStart != null, snacks => snacks.Where(s => s.TotalAmount >= totalAmountRangeStart))
                     .MapIf(totalAmountRangeEnd != null, snacks => snacks.Where(s => s.TotalAmount < totalAmountRangeEnd))
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

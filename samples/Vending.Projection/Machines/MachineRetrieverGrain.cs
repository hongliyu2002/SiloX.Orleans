using System.Collections.Immutable;
using System.Linq.Dynamic.Core;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Orleans.Concurrency;
using Orleans.FluentResults;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Projection.Abstractions.Machines;
using Vending.Projection.EntityFrameworkCore;

namespace Vending.Projection.Machines;

[StatelessWorker]
[Reentrant]
public class MachineRetrieverGrain : Grain, IMachineRetrieverGrain
{
    private readonly ProjectionDbContext _dbContext;

    /// <inheritdoc />
    public MachineRetrieverGrain(ProjectionDbContext dbContext)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<MachineInfo>>> ListAsync(MachineRetrieverListQuery query)
    {
        var moneyInsideAmountRangeStart = query.MoneyInsideAmountRange?.Start;
        var moneyInsideAmountRangeEnd = query.MoneyInsideAmountRange?.End;
        var amountInTransactionRangeStart = query.AmountInTransactionRange?.Start;
        var amountInTransactionRangeEnd = query.AmountInTransactionRange?.End;
        var slotsCountRangeStart = query.SlotsCountRange?.Start;
        var slotsCountRangeEnd = query.SlotsCountRange?.End;
        var snackCountRangeStart = query.SnackCountRange?.Start;
        var snackCountRangeEnd = query.SnackCountRange?.End;
        var snackQuantityRangeStart = query.SnackQuantityRange?.Start;
        var snackQuantityRangeEnd = query.SnackQuantityRange?.End;
        var snackAmountRangeStart = query.SnackAmountRange?.Start;
        var snackAmountRangeEnd = query.SnackAmountRange?.End;
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
        return Result.Ok(_dbContext.Machines.AsNoTracking())
                     .MapIf(moneyInsideAmountRangeStart != null, snacks => snacks.Where(sm => sm.MoneyInside.Amount >= moneyInsideAmountRangeStart))
                     .MapIf(moneyInsideAmountRangeEnd != null, snacks => snacks.Where(sm => sm.MoneyInside.Amount < moneyInsideAmountRangeEnd))
                     .MapIf(amountInTransactionRangeStart != null, snacks => snacks.Where(sm => sm.AmountInTransaction >= amountInTransactionRangeStart))
                     .MapIf(amountInTransactionRangeEnd != null, snacks => snacks.Where(sm => sm.AmountInTransaction < amountInTransactionRangeEnd))
                     .MapIf(slotsCountRangeStart != null, snacks => snacks.Where(sm => sm.SlotsCount >= slotsCountRangeStart))
                     .MapIf(slotsCountRangeEnd != null, snacks => snacks.Where(sm => sm.SlotsCount < slotsCountRangeEnd))
                     .MapIf(snackCountRangeStart != null, snacks => snacks.Where(sm => sm.SnackCount >= snackCountRangeStart))
                     .MapIf(snackCountRangeEnd != null, snacks => snacks.Where(sm => sm.SnackCount < snackCountRangeEnd))
                     .MapIf(snackQuantityRangeStart != null, snacks => snacks.Where(sm => sm.SnackQuantity >= snackQuantityRangeStart))
                     .MapIf(snackQuantityRangeEnd != null, snacks => snacks.Where(sm => sm.SnackQuantity < snackQuantityRangeEnd))
                     .MapIf(snackAmountRangeStart != null, snacks => snacks.Where(sm => sm.SnackAmount >= snackAmountRangeStart))
                     .MapIf(snackAmountRangeEnd != null, snacks => snacks.Where(sm => sm.SnackAmount < snackAmountRangeEnd))
                     .MapIf(boughtCountRangeStart != null, snacks => snacks.Where(sm => sm.BoughtCount >= boughtCountRangeStart))
                     .MapIf(boughtCountRangeEnd != null, snacks => snacks.Where(sm => sm.BoughtCount < boughtCountRangeEnd))
                     .MapIf(boughtAmountRangeStart != null, snacks => snacks.Where(sm => sm.BoughtAmount >= boughtAmountRangeStart))
                     .MapIf(boughtAmountRangeEnd != null, snacks => snacks.Where(sm => sm.BoughtAmount < boughtAmountRangeEnd))
                     .MapIf(createdAtRangeStart != null, snacks => snacks.Where(sm => sm.CreatedAt >= createdAtRangeStart))
                     .MapIf(createdAtRangeEnd != null, snacks => snacks.Where(sm => sm.CreatedAt < createdAtRangeEnd))
                     .MapIf(createdBy != null, snacks => snacks.Where(sm => sm.CreatedBy == createdBy))
                     .MapIf(lastModifiedAtRangeStart != null, snacks => snacks.Where(sm => sm.LastModifiedAt >= lastModifiedAtRangeStart))
                     .MapIf(lastModifiedAtRangeEnd != null, snacks => snacks.Where(sm => sm.LastModifiedAt < lastModifiedAtRangeEnd))
                     .MapIf(lastModifiedBy != null, snacks => snacks.Where(sm => sm.LastModifiedBy == lastModifiedBy))
                     .MapIf(deletedAtRangeStart != null, snacks => snacks.Where(sm => sm.DeletedAt >= deletedAtRangeStart))
                     .MapIf(deletedAtRangeEnd != null, snacks => snacks.Where(sm => sm.DeletedAt < deletedAtRangeEnd))
                     .MapIf(deletedBy != null, snacks => snacks.Where(sm => sm.DeletedBy == deletedBy))
                     .MapIf(isDeleted != null, snacks => snacks.Where(sm => sm.IsDeleted == isDeleted))
                     .MapIf(sortings != null, snacks => snacks.OrderBy(sortings!))
                     .MapTryAsync(snacks => snacks.ToImmutableListAsync());
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<MachineInfo>>> PagedListAsync(MachineRetrieverPagedListQuery query)
    {
        var moneyInsideAmountRangeStart = query.MoneyInsideAmountRange?.Start;
        var moneyInsideAmountRangeEnd = query.MoneyInsideAmountRange?.End;
        var amountInTransactionRangeStart = query.AmountInTransactionRange?.Start;
        var amountInTransactionRangeEnd = query.AmountInTransactionRange?.End;
        var slotsCountRangeStart = query.SlotsCountRange?.Start;
        var slotsCountRangeEnd = query.SlotsCountRange?.End;
        var snackCountRangeStart = query.SnackCountRange?.Start;
        var snackCountRangeEnd = query.SnackCountRange?.End;
        var snackQuantityRangeStart = query.SnackQuantityRange?.Start;
        var snackQuantityRangeEnd = query.SnackQuantityRange?.End;
        var snackAmountRangeStart = query.SnackAmountRange?.Start;
        var snackAmountRangeEnd = query.SnackAmountRange?.End;
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
        return Result.Ok(_dbContext.Machines.AsNoTracking())
                     .MapIf(moneyInsideAmountRangeStart != null, snacks => snacks.Where(sm => sm.MoneyInside.Amount >= moneyInsideAmountRangeStart))
                     .MapIf(moneyInsideAmountRangeEnd != null, snacks => snacks.Where(sm => sm.MoneyInside.Amount < moneyInsideAmountRangeEnd))
                     .MapIf(amountInTransactionRangeStart != null, snacks => snacks.Where(sm => sm.AmountInTransaction >= amountInTransactionRangeStart))
                     .MapIf(amountInTransactionRangeEnd != null, snacks => snacks.Where(sm => sm.AmountInTransaction < amountInTransactionRangeEnd))
                     .MapIf(slotsCountRangeStart != null, snacks => snacks.Where(sm => sm.SlotsCount >= slotsCountRangeStart))
                     .MapIf(slotsCountRangeEnd != null, snacks => snacks.Where(sm => sm.SlotsCount < slotsCountRangeEnd))
                     .MapIf(snackCountRangeStart != null, snacks => snacks.Where(sm => sm.SnackCount >= snackCountRangeStart))
                     .MapIf(snackCountRangeEnd != null, snacks => snacks.Where(sm => sm.SnackCount < snackCountRangeEnd))
                     .MapIf(snackQuantityRangeStart != null, snacks => snacks.Where(sm => sm.SnackQuantity >= snackQuantityRangeStart))
                     .MapIf(snackQuantityRangeEnd != null, snacks => snacks.Where(sm => sm.SnackQuantity < snackQuantityRangeEnd))
                     .MapIf(snackAmountRangeStart != null, snacks => snacks.Where(sm => sm.SnackAmount >= snackAmountRangeStart))
                     .MapIf(snackAmountRangeEnd != null, snacks => snacks.Where(sm => sm.SnackAmount < snackAmountRangeEnd))
                     .MapIf(boughtCountRangeStart != null, snacks => snacks.Where(sm => sm.BoughtCount >= boughtCountRangeStart))
                     .MapIf(boughtCountRangeEnd != null, snacks => snacks.Where(sm => sm.BoughtCount < boughtCountRangeEnd))
                     .MapIf(boughtAmountRangeStart != null, snacks => snacks.Where(sm => sm.BoughtAmount >= boughtAmountRangeStart))
                     .MapIf(boughtAmountRangeEnd != null, snacks => snacks.Where(sm => sm.BoughtAmount < boughtAmountRangeEnd))
                     .MapIf(createdAtRangeStart != null, snacks => snacks.Where(sm => sm.CreatedAt >= createdAtRangeStart))
                     .MapIf(createdAtRangeEnd != null, snacks => snacks.Where(sm => sm.CreatedAt < createdAtRangeEnd))
                     .MapIf(createdBy != null, snacks => snacks.Where(sm => sm.CreatedBy == createdBy))
                     .MapIf(lastModifiedAtRangeStart != null, snacks => snacks.Where(sm => sm.LastModifiedAt >= lastModifiedAtRangeStart))
                     .MapIf(lastModifiedAtRangeEnd != null, snacks => snacks.Where(sm => sm.LastModifiedAt < lastModifiedAtRangeEnd))
                     .MapIf(lastModifiedBy != null, snacks => snacks.Where(sm => sm.LastModifiedBy == lastModifiedBy))
                     .MapIf(deletedAtRangeStart != null, snacks => snacks.Where(sm => sm.DeletedAt >= deletedAtRangeStart))
                     .MapIf(deletedAtRangeEnd != null, snacks => snacks.Where(sm => sm.DeletedAt < deletedAtRangeEnd))
                     .MapIf(deletedBy != null, snacks => snacks.Where(sm => sm.DeletedBy == deletedBy))
                     .MapIf(isDeleted != null, snacks => snacks.Where(sm => sm.IsDeleted == isDeleted))
                     .MapIf(sortings != null, snacks => snacks.OrderBy(sortings!))
                     .MapIf(skipCount is >= 0, snacks => snacks.Skip(skipCount!.Value))
                     .MapIf(maxResultCount is >= 1, snacks => snacks.Take(maxResultCount!.Value))
                     .MapTryAsync(snacks => snacks.ToImmutableListAsync());
    }
}

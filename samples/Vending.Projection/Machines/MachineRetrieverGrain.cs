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
        var slotCountRangeStart = query.SlotCountRange?.Start;
        var slotCountRangeEnd = query.SlotCountRange?.End;
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
        var orderBy = query.OrderBy?.ToOrderByString();
        return Result.Ok(_dbContext.Machines.AsNoTracking())
                     .MapIf(moneyInsideAmountRangeStart != null, machines => machines.Where(m => m.MoneyInside.Amount >= moneyInsideAmountRangeStart))
                     .MapIf(moneyInsideAmountRangeEnd != null, machines => machines.Where(m => m.MoneyInside.Amount < moneyInsideAmountRangeEnd))
                     .MapIf(amountInTransactionRangeStart != null, machines => machines.Where(m => m.AmountInTransaction >= amountInTransactionRangeStart))
                     .MapIf(amountInTransactionRangeEnd != null, machines => machines.Where(m => m.AmountInTransaction < amountInTransactionRangeEnd))
                     .MapIf(slotCountRangeStart != null, machines => machines.Where(m => m.SlotCount >= slotCountRangeStart))
                     .MapIf(slotCountRangeEnd != null, machines => machines.Where(m => m.SlotCount < slotCountRangeEnd))
                     .MapIf(snackCountRangeStart != null, machines => machines.Where(m => m.SnackCount >= snackCountRangeStart))
                     .MapIf(snackCountRangeEnd != null, machines => machines.Where(m => m.SnackCount < snackCountRangeEnd))
                     .MapIf(snackQuantityRangeStart != null, machines => machines.Where(m => m.SnackQuantity >= snackQuantityRangeStart))
                     .MapIf(snackQuantityRangeEnd != null, machines => machines.Where(m => m.SnackQuantity < snackQuantityRangeEnd))
                     .MapIf(snackAmountRangeStart != null, machines => machines.Where(m => m.SnackAmount >= snackAmountRangeStart))
                     .MapIf(snackAmountRangeEnd != null, machines => machines.Where(m => m.SnackAmount < snackAmountRangeEnd))
                     .MapIf(boughtCountRangeStart != null, machines => machines.Where(m => m.BoughtCount >= boughtCountRangeStart))
                     .MapIf(boughtCountRangeEnd != null, machines => machines.Where(m => m.BoughtCount < boughtCountRangeEnd))
                     .MapIf(boughtAmountRangeStart != null, machines => machines.Where(m => m.BoughtAmount >= boughtAmountRangeStart))
                     .MapIf(boughtAmountRangeEnd != null, machines => machines.Where(m => m.BoughtAmount < boughtAmountRangeEnd))
                     .MapIf(createdAtRangeStart != null, machines => machines.Where(m => m.CreatedAt >= createdAtRangeStart))
                     .MapIf(createdAtRangeEnd != null, machines => machines.Where(m => m.CreatedAt < createdAtRangeEnd))
                     .MapIf(createdBy != null, machines => machines.Where(m => m.CreatedBy == createdBy))
                     .MapIf(lastModifiedAtRangeStart != null, machines => machines.Where(m => m.LastModifiedAt >= lastModifiedAtRangeStart))
                     .MapIf(lastModifiedAtRangeEnd != null, machines => machines.Where(m => m.LastModifiedAt < lastModifiedAtRangeEnd))
                     .MapIf(lastModifiedBy != null, machines => machines.Where(m => m.LastModifiedBy == lastModifiedBy))
                     .MapIf(deletedAtRangeStart != null, machines => machines.Where(m => m.DeletedAt >= deletedAtRangeStart))
                     .MapIf(deletedAtRangeEnd != null, machines => machines.Where(m => m.DeletedAt < deletedAtRangeEnd))
                     .MapIf(deletedBy != null, machines => machines.Where(m => m.DeletedBy == deletedBy))
                     .MapIf(isDeleted != null, machines => machines.Where(m => m.IsDeleted == isDeleted))
                     .MapIf(orderBy != null, machines => machines.OrderBy(orderBy!))
                     .MapTryAsync(machines => machines.ToImmutableListAsync());
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<MachineInfo>>> PagedListAsync(MachineRetrieverPagedListQuery query)
    {
        var moneyInsideAmountRangeStart = query.MoneyInsideAmountRange?.Start;
        var moneyInsideAmountRangeEnd = query.MoneyInsideAmountRange?.End;
        var amountInTransactionRangeStart = query.AmountInTransactionRange?.Start;
        var amountInTransactionRangeEnd = query.AmountInTransactionRange?.End;
        var slotCountRangeStart = query.SlotCountRange?.Start;
        var slotCountRangeEnd = query.SlotCountRange?.End;
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
        var orderBy = query.OrderBy?.ToOrderByString();
        var skipCount = query.SkipCount;
        var maxResultCount = query.MaxResultCount;
        return Result.Ok(_dbContext.Machines.AsNoTracking())
                     .MapIf(moneyInsideAmountRangeStart != null, machines => machines.Where(m => m.MoneyInside.Amount >= moneyInsideAmountRangeStart))
                     .MapIf(moneyInsideAmountRangeEnd != null, machines => machines.Where(m => m.MoneyInside.Amount < moneyInsideAmountRangeEnd))
                     .MapIf(amountInTransactionRangeStart != null, machines => machines.Where(m => m.AmountInTransaction >= amountInTransactionRangeStart))
                     .MapIf(amountInTransactionRangeEnd != null, machines => machines.Where(m => m.AmountInTransaction < amountInTransactionRangeEnd))
                     .MapIf(slotCountRangeStart != null, machines => machines.Where(m => m.SlotCount >= slotCountRangeStart))
                     .MapIf(slotCountRangeEnd != null, machines => machines.Where(m => m.SlotCount < slotCountRangeEnd))
                     .MapIf(snackCountRangeStart != null, machines => machines.Where(m => m.SnackCount >= snackCountRangeStart))
                     .MapIf(snackCountRangeEnd != null, machines => machines.Where(m => m.SnackCount < snackCountRangeEnd))
                     .MapIf(snackQuantityRangeStart != null, machines => machines.Where(m => m.SnackQuantity >= snackQuantityRangeStart))
                     .MapIf(snackQuantityRangeEnd != null, machines => machines.Where(m => m.SnackQuantity < snackQuantityRangeEnd))
                     .MapIf(snackAmountRangeStart != null, machines => machines.Where(m => m.SnackAmount >= snackAmountRangeStart))
                     .MapIf(snackAmountRangeEnd != null, machines => machines.Where(m => m.SnackAmount < snackAmountRangeEnd))
                     .MapIf(boughtCountRangeStart != null, machines => machines.Where(m => m.BoughtCount >= boughtCountRangeStart))
                     .MapIf(boughtCountRangeEnd != null, machines => machines.Where(m => m.BoughtCount < boughtCountRangeEnd))
                     .MapIf(boughtAmountRangeStart != null, machines => machines.Where(m => m.BoughtAmount >= boughtAmountRangeStart))
                     .MapIf(boughtAmountRangeEnd != null, machines => machines.Where(m => m.BoughtAmount < boughtAmountRangeEnd))
                     .MapIf(createdAtRangeStart != null, machines => machines.Where(m => m.CreatedAt >= createdAtRangeStart))
                     .MapIf(createdAtRangeEnd != null, machines => machines.Where(m => m.CreatedAt < createdAtRangeEnd))
                     .MapIf(createdBy != null, machines => machines.Where(m => m.CreatedBy == createdBy))
                     .MapIf(lastModifiedAtRangeStart != null, machines => machines.Where(m => m.LastModifiedAt >= lastModifiedAtRangeStart))
                     .MapIf(lastModifiedAtRangeEnd != null, machines => machines.Where(m => m.LastModifiedAt < lastModifiedAtRangeEnd))
                     .MapIf(lastModifiedBy != null, machines => machines.Where(m => m.LastModifiedBy == lastModifiedBy))
                     .MapIf(deletedAtRangeStart != null, machines => machines.Where(m => m.DeletedAt >= deletedAtRangeStart))
                     .MapIf(deletedAtRangeEnd != null, machines => machines.Where(m => m.DeletedAt < deletedAtRangeEnd))
                     .MapIf(deletedBy != null, machines => machines.Where(m => m.DeletedBy == deletedBy))
                     .MapIf(isDeleted != null, machines => machines.Where(m => m.IsDeleted == isDeleted))
                     .MapIf(orderBy != null, machines => machines.OrderBy(orderBy!))
                     .MapIf(skipCount is >= 0, machines => machines.Skip(skipCount!.Value))
                     .MapIf(maxResultCount is >= 1, machines => machines.Take(maxResultCount!.Value))
                     .MapTryAsync(machines => machines.ToImmutableListAsync());
    }
}

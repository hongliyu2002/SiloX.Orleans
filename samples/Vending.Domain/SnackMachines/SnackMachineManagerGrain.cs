using System.Collections.Immutable;
using System.Linq.Dynamic.Core;
using Fluxera.Extensions.Common;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Orleans.Concurrency;
using Orleans.FluentResults;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions.SnackMachines;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.SnackMachines;

[StatelessWorker]
public sealed class SnackMachineManagerGrain : Grain, ISnackMachineManagerGrain
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly DomainDbContext _dbContext;

    /// <inheritdoc />
    public SnackMachineManagerGrain(DomainDbContext dbContext, IGuidGenerator guidGenerator)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _guidGenerator = Guard.Against.Null(guidGenerator, nameof(guidGenerator));
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<SnackMachine>>> ListAsync(SnackMachineManagerListQuery query)
    {
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
        return Result.Ok(_dbContext.SnackMachines.AsNoTracking())
                     .MapIf(createdAtRangeStart != null, machines => machines.Where(sm => sm.CreatedAt >= createdAtRangeStart))
                     .MapIf(createdAtRangeEnd != null, machines => machines.Where(sm => sm.CreatedAt < createdAtRangeEnd))
                     .MapIf(createdBy != null, machines => machines.Where(sm => sm.CreatedBy == createdBy))
                     .MapIf(lastModifiedAtRangeStart != null, machines => machines.Where(sm => sm.LastModifiedAt >= lastModifiedAtRangeStart))
                     .MapIf(lastModifiedAtRangeEnd != null, machines => machines.Where(sm => sm.LastModifiedAt < lastModifiedAtRangeEnd))
                     .MapIf(lastModifiedBy != null, machines => machines.Where(sm => sm.LastModifiedBy == lastModifiedBy))
                     .MapIf(deletedAtRangeStart != null, machines => machines.Where(sm => sm.DeletedAt >= deletedAtRangeStart))
                     .MapIf(deletedAtRangeEnd != null, machines => machines.Where(sm => sm.DeletedAt < deletedAtRangeEnd))
                     .MapIf(deletedBy != null, machines => machines.Where(sm => sm.DeletedBy == deletedBy))
                     .MapIf(isDeleted != null, machines => machines.Where(sm => sm.IsDeleted == isDeleted))
                     .MapIf(sortings != null, machines => machines.OrderBy(sortings!))
                     .MapTryAsync(machines => machines.ToImmutableListAsync());
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<SnackMachine>>> PagedListAsync(SnackMachineManagerPagedListQuery query)
    {
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
        return Result.Ok(_dbContext.SnackMachines.AsNoTracking())
                     .MapIf(createdAtRangeStart != null, machines => machines.Where(sm => sm.CreatedAt >= createdAtRangeStart))
                     .MapIf(createdAtRangeEnd != null, machines => machines.Where(sm => sm.CreatedAt < createdAtRangeEnd))
                     .MapIf(createdBy != null, machines => machines.Where(sm => sm.CreatedBy == createdBy))
                     .MapIf(lastModifiedAtRangeStart != null, machines => machines.Where(sm => sm.LastModifiedAt >= lastModifiedAtRangeStart))
                     .MapIf(lastModifiedAtRangeEnd != null, machines => machines.Where(sm => sm.LastModifiedAt < lastModifiedAtRangeEnd))
                     .MapIf(lastModifiedBy != null, machines => machines.Where(sm => sm.LastModifiedBy == lastModifiedBy))
                     .MapIf(deletedAtRangeStart != null, machines => machines.Where(sm => sm.DeletedAt >= deletedAtRangeStart))
                     .MapIf(deletedAtRangeEnd != null, machines => machines.Where(sm => sm.DeletedAt < deletedAtRangeEnd))
                     .MapIf(deletedBy != null, machines => machines.Where(sm => sm.DeletedBy == deletedBy))
                     .MapIf(isDeleted != null, machines => machines.Where(sm => sm.IsDeleted == isDeleted))
                     .MapIf(sortings != null, machines => machines.OrderBy(sortings!))
                     .MapIf(skipCount is >= 0, machines => machines.Skip(skipCount!.Value))
                     .MapIf(maxResultCount is >= 1, machines => machines.Take(maxResultCount!.Value))
                     .MapTryAsync(machines => machines.ToImmutableListAsync());
    }

    /// <inheritdoc />
    public Task<Result<Guid>> CreateAsync(SnackMachineManagerCreateCommand command)
    {
        var machineId = _guidGenerator.Create();
        return Result.Ok()
                     .MapTry(() => GrainFactory.GetGrain<ISnackMachineGrain>(machineId))
                     .MapTryAsync(grain => grain.InitializeAsync(new SnackMachineInitializeCommand(machineId, command.MoneyInside, command.Slots, command.TraceId, command.OperatedAt, command.OperatedBy)))
                     .MapAsync(() => machineId);
    }

    /// <inheritdoc />
    public Task<Result> DeleteAsync(SnackMachineManagerDeleteCommand command)
    {
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.SnackMachines.Where(sm => sm.IsDeleted == false).AnyAsync(sm => sm.Id == command.MachineId))
                     .EnsureAsync(exists => exists, $"Snack machine {command.MachineId} does not exist or has already been deleted.")
                     .MapTryAsync(() => GrainFactory.GetGrain<ISnackMachineGrain>(command.MachineId))
                     .BindTryAsync(grain => grain.RemoveAsync(new SnackMachineRemoveCommand(command.TraceId, command.OperatedAt, command.OperatedBy)));
    }

    /// <inheritdoc />
    public Task<Result> DeleteManyAsync(SnackMachineManagerDeleteManyCommand command)
    {
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.SnackMachines.Where(sm => sm.IsDeleted == false).Select(sm => sm.Id).Intersect(command.MachineIds).ToListAsync())
                     .EnsureAsync(machineIds => machineIds.Count == command.MachineIds.Count, "Some snack machines do not exist or have already been deleted.")
                     .MapTryAsync(machineIds => machineIds.Select(machineId => GrainFactory.GetGrain<ISnackMachineGrain>(machineId)))
                     .MapTryAsync(grains => grains.Select(grain => grain.RemoveAsync(new SnackMachineRemoveCommand(command.TraceId, command.OperatedAt, command.OperatedBy))))
                     .MapTryAsync(Task.WhenAll)
                     .BindTryAsync(Result.Combine);
    }
}

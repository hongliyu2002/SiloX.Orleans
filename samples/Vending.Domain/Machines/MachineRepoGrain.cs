using Fluxera.Extensions.Common;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Machines;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Machines;

[StatelessWorker]
[Reentrant]
public sealed class MachineRepoGrain : Grain, IMachineRepoGrain
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly DomainDbContext _dbContext;

    /// <inheritdoc />
    public MachineRepoGrain(DomainDbContext dbContext, IGuidGenerator guidGenerator)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _guidGenerator = Guard.Against.Null(guidGenerator, nameof(guidGenerator));
    }

    /// <inheritdoc />
    public Task<Result<Guid>> CreateAsync(MachineRepoCreateCommand command)
    {
        var machineId = _guidGenerator.Create();
        return Result.Ok()
                     .MapTry(() => GrainFactory.GetGrain<IMachineGrain>(machineId))
                     .MapTryAsync(grain => grain.InitializeAsync(new MachineInitializeCommand(machineId, command.MoneyInside, command.Slots, command.TraceId, command.OperatedAt, command.OperatedBy)))
                     .MapAsync(() => machineId);
    }

    /// <inheritdoc />
    public Task<Result> DeleteAsync(MachineRepoDeleteCommand command)
    {
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.Machines.Where(sm => sm.IsDeleted == false).AnyAsync(sm => sm.Id == command.MachineId))
                     .EnsureAsync(exists => exists, $"Snack machine {command.MachineId} does not exist or has already been deleted.")
                     .MapTryAsync(() => GrainFactory.GetGrain<IMachineGrain>(command.MachineId))
                     .BindTryAsync(grain => grain.RemoveAsync(new MachineRemoveCommand(command.TraceId, command.OperatedAt, command.OperatedBy)));
    }

    /// <inheritdoc />
    public Task<Result> DeleteManyAsync(MachineRepoDeleteManyCommand command)
    {
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.Machines.Where(sm => sm.IsDeleted == false).Select(sm => sm.Id).Intersect(command.MachineIds).ToListAsync())
                     .EnsureAsync(machineIds => machineIds.Count == command.MachineIds.Count, "Some snack machines do not exist or have already been deleted.")
                     .MapTryAsync(machineIds => machineIds.Select(machineId => GrainFactory.GetGrain<IMachineGrain>(machineId)))
                     .MapTryAsync(grains => grains.Select(grain => grain.RemoveAsync(new MachineRemoveCommand(command.TraceId, command.OperatedAt, command.OperatedBy))))
                     .MapTryAsync(Task.WhenAll)
                     .BindTryAsync(Result.Combine);
    }
}

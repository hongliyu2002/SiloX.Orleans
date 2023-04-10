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
    public Task<Result<Machine>> CreateAsync(MachineRepoCreateCommand command)
    {
        var machineId = _guidGenerator.Create();
        IMachineGrain grain = null!;
        return Result.Ok()
                     .MapTry(() => grain = GrainFactory.GetGrain<IMachineGrain>(machineId))
                     .BindTryAsync(() => grain.InitializeAsync(new MachineInitializeCommand(machineId, command.MoneyInside, command.Slots, command.TraceId, command.OperatedAt, command.OperatedBy)))
                     .MapAsync(() => grain.GetMachineAsync());
    }

    /// <inheritdoc />
    public Task<Result<Machine>> UpdateAsync(MachineRepoUpdateCommand command)
    {
        var machineId = command.MachineId;
        IMachineGrain grain = null!;
        return Result.Ok()
                     .MapTry(() => grain = GrainFactory.GetGrain<IMachineGrain>(machineId))
                     .BindTryAsync(() => grain.UpdateAsync(new MachineUpdateCommand(command.MoneyInside, command.Slots, command.TraceId, command.OperatedAt, command.OperatedBy)))
                     .MapAsync(() => grain.GetMachineAsync());
    }

    /// <inheritdoc />
    public Task<Result> DeleteAsync(MachineRepoDeleteCommand command)
    {
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.Machines.Where(m => m.IsDeleted == false).AnyAsync(m => m.Id == command.MachineId))
                     .EnsureAsync(exists => exists, $"Machine {command.MachineId} does not exist or has already been deleted.")
                     .MapTryAsync(() => GrainFactory.GetGrain<IMachineGrain>(command.MachineId))
                     .BindTryAsync(grain => grain.DeleteAsync(new MachineDeleteCommand(command.TraceId, command.OperatedAt, command.OperatedBy)));
    }

    /// <inheritdoc />
    public Task<Result> DeleteManyAsync(MachineRepoDeleteManyCommand command)
    {
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.Machines.Where(m => m.IsDeleted == false).Select(m => m.Id).Intersect(command.MachineIds).ToListAsync())
                     .EnsureAsync(machineIds => machineIds.Count == command.MachineIds.Count, "Some machines do not exist or have already been deleted.")
                     .MapTryAsync(machineIds => machineIds.Select(machineId => GrainFactory.GetGrain<IMachineGrain>(machineId)))
                     .MapTryAsync(grains => grains.Select(grain => grain.DeleteAsync(new MachineDeleteCommand(command.TraceId, command.OperatedAt, command.OperatedBy))))
                     .MapTryAsync(Task.WhenAll)
                     .BindTryAsync(Result.Combine);
    }
}
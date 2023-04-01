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
public sealed class SnackMachineRepoGrain : Grain, ISnackMachineRepoGrain
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly DomainDbContext _dbContext;

    /// <inheritdoc />
    public SnackMachineRepoGrain(DomainDbContext dbContext, IGuidGenerator guidGenerator)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _guidGenerator = Guard.Against.Null(guidGenerator, nameof(guidGenerator));
    }

    /// <inheritdoc />
    public Task<Result<Guid>> CreateAsync(SnackMachineRepoCreateCommand command)
    {
        var machineId = _guidGenerator.Create();
        return Result.Ok()
                     .MapTry(() => GrainFactory.GetGrain<ISnackMachineGrain>(machineId))
                     .MapTryAsync(grain => grain.InitializeAsync(new SnackMachineInitializeCommand(machineId, command.MoneyInside, command.Slots, command.TraceId, command.OperatedAt, command.OperatedBy)))
                     .MapAsync(() => machineId);
    }

    /// <inheritdoc />
    public Task<Result> DeleteAsync(SnackMachineRepoDeleteCommand command)
    {
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.SnackMachines.Where(sm => sm.IsDeleted == false).AnyAsync(sm => sm.Id == command.MachineId))
                     .EnsureAsync(exists => exists, $"Snack machine {command.MachineId} does not exist or has already been deleted.")
                     .MapTryAsync(() => GrainFactory.GetGrain<ISnackMachineGrain>(command.MachineId))
                     .BindTryAsync(grain => grain.RemoveAsync(new SnackMachineRemoveCommand(command.TraceId, command.OperatedAt, command.OperatedBy)));
    }

    /// <inheritdoc />
    public Task<Result> DeleteManyAsync(SnackMachineRepoDeleteManyCommand command)
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

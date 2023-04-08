using Fluxera.Extensions.Common;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Orleans.Concurrency;
using Orleans.FluentResults;
using Vending.Domain.Abstractions.Snacks;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Snacks;

[StatelessWorker]
[Reentrant]
public sealed class SnackRepoGrain : Grain, ISnackRepoGrain
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly DomainDbContext _dbContext;

    /// <inheritdoc />
    public SnackRepoGrain(DomainDbContext dbContext, IGuidGenerator guidGenerator)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _guidGenerator = Guard.Against.Null(guidGenerator, nameof(guidGenerator));
    }

    /// <inheritdoc />
    public Task<Result<Snack>> CreateAsync(SnackRepoCreateCommand command)
    {
        var snackId = _guidGenerator.Create();
        ISnackGrain grain = null!;
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.Snacks.Where(s => s.IsDeleted == false).AllAsync(s => s.Name != command.Name))
                     .EnsureAsync(notExist => notExist, $"Snack with name '{command.Name}' already exists.")
                     .MapTryAsync(() => grain = GrainFactory.GetGrain<ISnackGrain>(snackId))
                     .BindTryAsync(() => grain.InitializeAsync(new SnackInitializeCommand(snackId, command.Name, command.PictureUrl, command.TraceId, command.OperatedAt, command.OperatedBy)))
                     .MapAsync(() => grain.GetSnackAsync());
    }

    /// <inheritdoc />
    public Task<Result<Snack>> UpdateAsync(SnackRepoUpdateCommand command)
    {
        var snackId = command.SnackId;
        ISnackGrain grain = null!;
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.Snacks.Where(s => s.IsDeleted == false).AllAsync(s => s.Name != command.Name))
                     .EnsureAsync(notExist => notExist, $"Snack with name '{command.Name}' already exists.")
                     .MapTryAsync(() => grain = GrainFactory.GetGrain<ISnackGrain>(snackId))
                     .BindTryAsync(() => grain.UpdateAsync(new SnackUpdateCommand(command.Name, command.PictureUrl, command.TraceId, command.OperatedAt, command.OperatedBy)))
                     .MapAsync(() => grain.GetSnackAsync());
    }

    /// <inheritdoc />
    public Task<Result> DeleteAsync(SnackRepoDeleteCommand command)
    {
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.Snacks.Where(s => s.IsDeleted == false).AnyAsync(s => s.Id == command.SnackId))
                     .EnsureAsync(exists => exists, $"Snack {command.SnackId} does not exist or has already been deleted.")
                     .MapTryAsync(() => GrainFactory.GetGrain<ISnackGrain>(command.SnackId))
                     .BindTryAsync(grain => grain.DeleteAsync(new SnackDeleteCommand(command.TraceId, command.OperatedAt, command.OperatedBy)));
    }

    /// <inheritdoc />
    public Task<Result> DeleteManyAsync(SnackRepoDeleteManyCommand command)
    {
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.Snacks.Where(s => s.IsDeleted == false).Select(s => s.Id).Intersect(command.SnackIds).ToListAsync())
                     .EnsureAsync(snackIds => snackIds.Count == command.SnackIds.Count, "Some snacks do not exist or have already been deleted.")
                     .MapTryAsync(snackIds => snackIds.Select(snackId => GrainFactory.GetGrain<ISnackGrain>(snackId)))
                     .MapTryAsync(grains => grains.Select(grain => grain.DeleteAsync(new SnackDeleteCommand(command.TraceId, command.OperatedAt, command.OperatedBy))))
                     .MapTryAsync(Task.WhenAll)
                     .BindTryAsync(Result.Combine);
    }
}

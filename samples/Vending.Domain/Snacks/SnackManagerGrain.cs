using System.Collections.Immutable;
using System.Linq.Dynamic.Core;
using Fluxera.Extensions.Common;
using Fluxera.Guards;
using Microsoft.EntityFrameworkCore;
using Orleans.Concurrency;
using Orleans.FluentResults;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions.Snacks;
using Vending.Domain.EntityFrameworkCore;

namespace Vending.Domain.Snacks;

[StatelessWorker]
public sealed class SnackManagerGrain : Grain, ISnackManagerGrain
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly DomainDbContext _dbContext;

    /// <inheritdoc />
    public SnackManagerGrain(DomainDbContext dbContext,
                             IGuidGenerator guidGenerator)
    {
        _dbContext = Guard.Against.Null(dbContext, nameof(dbContext));
        _guidGenerator = Guard.Against.Null(guidGenerator, nameof(guidGenerator));
    }

    /// <inheritdoc />
    public Task<Result<ImmutableList<Snack>>> ListAsync(SnackManagerListQuery query)
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
        return Result.Ok(_dbContext.Snacks.AsNoTracking())
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
    public Task<Result<ImmutableList<Snack>>> PagedListAsync(SnackManagerPagedListQuery query)
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
        return Result.Ok(_dbContext.Snacks.AsNoTracking())
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
    public Task<Result<ImmutableList<Snack>>> SearchingListAsync(SnackManagerSearchingListQuery query)
    {
        var searchCriteria = query.SearchCriteria;
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
                     .MapIf(searchCriteria != null, snacks => snacks.Where(s => s.Name.Contains(searchCriteria!)))
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
    public Task<Result<ImmutableList<Snack>>> SearchingPagedListAsync(SnackManagerSearchingPagedListQuery query)
    {
        var searchCriteria = query.SearchCriteria;
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
                     .MapIf(searchCriteria != null, snacks => snacks.Where(s => s.Name.Contains(searchCriteria!)))
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
    public Task<Result<Guid>> CreateAsync(SnackManagerCreateCommand command)
    {
        var snackId = _guidGenerator.Create();
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.Snacks.Where(s => s.IsDeleted == false).AllAsync(s => s.Name != command.Name))
                     .EnsureAsync(noDuplication => noDuplication, $"Snack with name '{command.Name}' already exists.")
                     .MapTryAsync(() => GrainFactory.GetGrain<ISnackGrain>(snackId))
                     .MapTryAsync(grain => grain.InitializeAsync(new SnackInitializeCommand(snackId, command.Name, command.PictureUrl, command.TraceId, command.OperatedAt, command.OperatedBy)))
                     .MapAsync(() => snackId);
    }

    /// <inheritdoc />
    public Task<Result> DeleteAsync(SnackManagerDeleteCommand command)
    {
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.Snacks.Where(s => s.IsDeleted == false).AnyAsync(s => s.Id == command.SnackId))
                     .EnsureAsync(exists => exists, $"Snack {command.SnackId} does not exist or has already been deleted.")
                     .MapTryAsync(() => GrainFactory.GetGrain<ISnackGrain>(command.SnackId))
                     .BindTryAsync(grain => grain.RemoveAsync(new SnackRemoveCommand(command.TraceId, command.OperatedAt, command.OperatedBy)));
    }

    /// <inheritdoc />
    public Task<Result> DeleteManyAsync(SnackManagerDeleteManyCommand command)
    {
        return Result.Ok()
                     .MapTryAsync(() => _dbContext.Snacks.Where(s => s.IsDeleted == false).Select(s => s.Id).Intersect(command.SnackIds).ToListAsync())
                     .EnsureAsync(snackIds => snackIds.Count == command.SnackIds.Count, "Some snacks do not exist or have already been deleted.")
                     .MapTryAsync(snackIds => snackIds.Select(snackId => GrainFactory.GetGrain<ISnackGrain>(snackId)))
                     .MapTryAsync(grains => grains.Select(grain => grain.RemoveAsync(new SnackRemoveCommand(command.TraceId, command.OperatedAt, command.OperatedBy))))
                     .MapTryAsync(Task.WhenAll)
                     .BindTryAsync(Result.Combine);
    }
}

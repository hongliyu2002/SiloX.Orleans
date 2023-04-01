using System.Collections.Immutable;
using Orleans.Concurrency;
using Orleans.FluentResults;

namespace Vending.Domain.Abstractions.Snacks;

public interface ISnackManagerGrain : IGrainWithStringKey
{
    [AlwaysInterleave]
    Task<Result<ImmutableList<Snack>>> ListAsync(SnackManagerListQuery query);
    
    [AlwaysInterleave]
    Task<Result<ImmutableList<Snack>>> PagedListAsync(SnackManagerPagedListQuery query);
    
    [AlwaysInterleave]
    Task<Result<ImmutableList<Snack>>> SearchingListAsync(SnackManagerSearchingListQuery query);
    
    [AlwaysInterleave]
    Task<Result<ImmutableList<Snack>>> SearchingPagedListAsync(SnackSearchingPagedListQuery query);
    
    Task<Result<Snack>> CreateAsync(SnackManagerCreateCommand command);
    
    Task<Result<Guid>> DeleteAsync(SnackManagerDeleteCommand command);
        
    Task<Result<ImmutableList<Guid>>> DeleteManyAsync(SnackManagerDeleteManyCommand command);
}

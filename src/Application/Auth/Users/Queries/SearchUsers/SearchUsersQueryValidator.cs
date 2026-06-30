using FluentValidation;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Users.Queries.SearchUsers;

public sealed class SearchUsersQueryValidator : AbstractValidator<SearchUsersQuery>
{
    public SearchUsersQueryValidator()
    {
        RuleFor(x => x.Limit).InclusiveBetween(1, PagedApiResponseFactory.MaxPageSize);
        RuleFor(x => x.Offset).GreaterThanOrEqualTo(0);
    }
}

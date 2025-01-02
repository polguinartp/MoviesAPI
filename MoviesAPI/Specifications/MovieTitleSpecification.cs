using Domain.Entities;
using Domain.Specifications;
using System;
using System.Linq.Expressions;

namespace MoviesAPI.Specifications;

public class MovieTitleSpecification : Specification<ShowtimeEntity>
{
    private string _movieTitle;

    public MovieTitleSpecification(string movieTitle)
    {
        _movieTitle = movieTitle;
    }

    public override Expression<Func<ShowtimeEntity, bool>> ToExpression()
    {
        return entity => string.Equals(entity.Movie.Title, _movieTitle.ToUpper(), StringComparison.OrdinalIgnoreCase);
    }
}

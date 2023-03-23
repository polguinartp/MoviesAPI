using Domain.Entities;
using Domain.Specifications;
using System;
using System.Linq.Expressions;

namespace MoviesAPI.Specifications
{
    public class ReleaseDateOlderThanSixMontsSpecification : Specification<ShowtimeEntity>
    {
        private DateTime _date;

        public ReleaseDateOlderThanSixMontsSpecification(DateTime date)
        {
            _date = date;
        }

        public override Expression<Func<ShowtimeEntity, bool>> ToExpression()
        {
            return x => _date >= x.StartDate && _date <= x.EndDate;
        }
    }
}

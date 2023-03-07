using System;
using System.Linq;
using System.Linq.Expressions;

namespace Domain.Specifications.Operators
{
    internal sealed class AndSpecification<T> : Specification<T>
    {
        private Specification<T> _leftSpecification;
        private Specification<T> _rightSpecification;

        public AndSpecification(Specification<T> specification1, Specification<T> specification2)
        {
            _leftSpecification = specification1;
            _rightSpecification = specification2;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var leftExpression = _leftSpecification.ToExpression();
            var rightExpression = _rightSpecification.ToExpression();

            var parameter = leftExpression.Parameters.First();

            var andExpression = Expression.AndAlso(leftExpression.Body, Expression.Invoke(rightExpression, parameter));
            return Expression.Lambda<Func<T, bool>>(andExpression, parameter);
        }
    }
}

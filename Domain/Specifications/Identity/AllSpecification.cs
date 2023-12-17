using System;
using System.Linq.Expressions;

namespace Domain.Specifications.Identity;

internal sealed class AllSpecification<T> : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return x => true;
    }
}

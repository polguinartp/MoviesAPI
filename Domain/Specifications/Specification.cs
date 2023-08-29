using Domain.Entities;
using Domain.Specifications.Identity;
using Domain.Specifications.Operators;
using System;
using System.Linq.Expressions;

namespace Domain.Specifications
{
    public abstract class Specification<T>
    {
        public static readonly Specification<T> All = new AllSpecification<T>();
        public static readonly Specification<T> None = new NoneSpecification<T>();

        public abstract Expression<Func<T, bool>> ToExpression();

        public bool IsSatisfiedBy { get; }

        public Specification<T> And(Specification<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }

        public Specification<T> Or(Specification<T> specification)
        {
            return new OrSpecification<T>(this, specification);
        }

        public Specification<T> Not(Specification<T> specification)
        {
            return new NotSpecification<T>(specification);
        }
    }
}

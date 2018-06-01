using System;
using System.Linq.Expressions;

namespace Minim.Register
{
    public interface IMappingRegistration<TSource, TDest> : IMappingConfiguration<TSource, TDest>
    {
        IMappingConfiguration<TSource, TDest> ConstructWith(Expression<Func<TSource, TDest>> create);
    }
}
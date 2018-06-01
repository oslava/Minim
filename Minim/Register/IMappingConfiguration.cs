using System;
using System.Linq.Expressions;

namespace Minim.Register
{
    public interface IMappingConfiguration<TSource, TDest>
    {
        IMappingConfiguration<TSource, TDest> Configure<TMember>(Expression<Func<TDest, TMember>> destinationMember, Expression<Func<TSource, TMember>> source);
        IMappingConfiguration<TSource, TDest> Ignore<TMember>(Expression<Func<TDest, TMember>> destinationMember);
    }
}
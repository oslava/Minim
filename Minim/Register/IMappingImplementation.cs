using System;
using System.Linq.Expressions;

namespace Minim.Register
{
    public interface IMappingImplementation
    {
        Tuple<LambdaExpression, LambdaExpression>[] GetMemberConfigurations();
        LambdaExpression Create { get; }
        LambdaExpression Default { get; }
        Type Src { get; }
        Type Dest { get; }
	    void BuildUp(Mapper mapper);
    }
}
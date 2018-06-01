using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Minim.Register
{
    public class MappingRegistration<TSource, TDest> : IMappingRegistration<TSource, TDest>, IMappingImplementation
    {
        private Expression<Func<TSource, TDest>> create;
        private readonly IList<Tuple<LambdaExpression, LambdaExpression>> memberConfigurations = 
            new List<Tuple<LambdaExpression, LambdaExpression>>();

        public IMappingConfiguration<TSource, TDest> ConstructWith(Expression<Func<TSource, TDest>> create)
        {
            this.create = create;
            return this;
        }

        public IMappingConfiguration<TSource, TDest> Configure<TMember>(Expression<Func<TDest, TMember>> destinationMember, Expression<Func<TSource, TMember>> source)
        {
            memberConfigurations.Add(new Tuple<LambdaExpression, LambdaExpression>(destinationMember, source));
            return this;
        }

        public IMappingConfiguration<TSource, TDest> Ignore<TMember>(Expression<Func<TDest, TMember>> destinationMember)
        {
            memberConfigurations.Add(new Tuple<LambdaExpression, LambdaExpression>(destinationMember, null));
            return this;
        }

        Tuple<LambdaExpression, LambdaExpression>[] IMappingImplementation.GetMemberConfigurations() => 
            memberConfigurations.ToArray();

        LambdaExpression IMappingImplementation.Create =>
            create;

        public LambdaExpression Default { get; set; }

        Type IMappingImplementation.Src => typeof(TSource);

	    Type IMappingImplementation.Dest => typeof(TDest);

	    void IMappingImplementation.BuildUp(Mapper mapper)
	    {
		    mapper.BuildUp<TSource, TDest>(this);
	    }
	}
}
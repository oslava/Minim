using System;
using System.Linq.Expressions;
using Minim.Register;

namespace Minim
{
    public static class MappingConfigurationExtensions
    {
        public static IMappingConfiguration<TSource, TDest> Default<TSource, TDest>(this IMappingConfiguration<TSource, TDest> config,
            Expression<Func<TDest>> def)
            where TSource : class
        {
            (config as MappingRegistration<TSource, TDest>).Default = def;
            return config;
        }
    }
}
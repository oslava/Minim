using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Minim.Register;

namespace Minim.Generate
{
    public class MemberMappingsResolver : IMemberMappingsResolver
    {
        public IList<MemberMapping> ResolveMappings(IMappingImplementation impl)
        {
	        var src = impl.Src;
	        var dest = impl.Dest;

	        var memberEvals = impl.GetMemberConfigurations()
				.ToLookup(x => ResolveMember(x.Item1), x => x.Item2);
            if (memberEvals.Any(g => g.Count() > 1))
                throw new NotSupportedException();

            var mappings = GetMappings(src, dest)
                .Where(x => !memberEvals[x.Item1].Any(eval => eval == null))
                .Select(x => new MemberMapping
                {
                    Target = x.Item1,
                    Member = x.Item2,
                    Lambda = memberEvals[x.Item1].FirstOrDefault()
                })
                .ToList();

            var voidMapping = mappings.FirstOrDefault(x => x.IsVoid());
            if (voidMapping != null)
            {
                var memberType = voidMapping.Target.MemberType == MemberTypes.Field ? "Field" : "Property";
                throw new MissingMemberException(
                    $"{memberType} {dest.Name}.{voidMapping.Target.Name} has no corresponding member in {src.Name} type.");
            }

            return mappings;
        }

        private static IEnumerable<Tuple<MemberInfo, MemberInfo>> GetMappings(Type src, Type dest)
        {
            var destMembers = dest
                .GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField)
                .Cast<MemberInfo>()
                .Concat(dest.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetSetMethod() != null));
            var srcMembers = src
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Cast<MemberInfo>()
                .Concat(src.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
                .ToLookup(x => x.Name, StringComparer.CurrentCultureIgnoreCase);

            foreach (var memberInfo in destMembers)
            {
                var srcMemberInfo = memberInfo.MemberType == MemberTypes.Field 
                    ? srcMembers[memberInfo.Name].FirstOrDefault() 
                    : srcMembers[memberInfo.Name].LastOrDefault();

                yield return new Tuple<MemberInfo, MemberInfo>(memberInfo, srcMemberInfo);
            }
        }

        private MemberInfo ResolveMember(LambdaExpression memberExpression)
        {
            var expression = memberExpression.Body as MemberExpression;
            if (expression == null)
                throw new NotSupportedException();

            var member = expression.Member;
            if (member.MemberType != MemberTypes.Field && member.MemberType != MemberTypes.Property)
                throw new NotSupportedException();

            return member;
        }
    }
}
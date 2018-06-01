using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Minim.Register;
using DynamicBuilders = System.Tuple<System.Reflection.Emit.TypeBuilder, System.Reflection.Emit.MethodBuilder>;
using TypeMap = System.Tuple<System.Type, System.Type>;

namespace Minim.Generate
{
	public class DynamicMemberResolver
	{
		private readonly Dictionary<IMappingImplementation, DynamicBuilders> cacheBuilders = new Dictionary<IMappingImplementation, DynamicBuilders>();
		private readonly Dictionary<TypeMap, IMappingImplementation> mappings;
		private readonly List<IMappingImplementation> resolvedMappings = new List<IMappingImplementation>();

		public DynamicMemberResolver(Dictionary<TypeMap, IMappingImplementation> mappings)
		{
			this.mappings = mappings;
		}

		public IList<MemberBinding> ResolveMemberBindings(IList<MemberMapping> memberMappings, Func<LambdaExpression, MethodInfo> lambdaMethodBuilder)
		{
			var memberBindings = memberMappings
				.Select(x => new MemberBinding
				{
					Target = x.Target,
					Member = x.Member,
					Method = x.Lambda != null ? lambdaMethodBuilder(x.Lambda) : null
				})
				.ToList();

			foreach (var x in memberBindings)
			{
				if (x.Method != null)
					continue;
				
				var key = new TypeMap(GetMemberType(x.Member), GetMemberType(x.Target));
				IMappingImplementation impl;
				if (mappings.TryGetValue(key, out impl))
				{
					var builders = ResolveBuilders(impl);
					x.Method = builders.Item2;
					x.CallMethodOnMember = true;
				}
			}

			return memberBindings;
		}

		private static Type GetMemberType(MemberInfo member)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Property:
					return ((PropertyInfo)member).PropertyType;
				case MemberTypes.Field:
					return ((FieldInfo)member).FieldType;
				default:
					throw new ArgumentException(nameof(member));
			}
		}

		public DynamicBuilders ResolveBuilders(IMappingImplementation impl)
		{
			DynamicBuilders result;
			if (!cacheBuilders.TryGetValue(impl, out result))
			{
				var dynType = DynamicModuleHost.NewMapperDynamicType();
				var dynMethod = dynType.DefineMethod("Map", MethodAttributes.Static | MethodAttributes.Public, impl.Dest, new[] { impl.Src });
				dynMethod.DefineParameter(1, ParameterAttributes.None, "x");

				cacheBuilders[impl] = result = new DynamicBuilders(dynType, dynMethod);
				resolvedMappings.Add(impl);
			}
			return result;
		}

		public IMappingImplementation[] CaptureResolvedMappings()
		{
			var result = resolvedMappings.ToArray();
			resolvedMappings.Clear();
			return result;
		}
	}
}
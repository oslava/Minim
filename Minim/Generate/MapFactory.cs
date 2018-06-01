using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Minim.Register;

namespace Minim.Generate
{
    public class MapFactory
    {
        private readonly IMemberMappingsResolver memberMappingsResolver;
        private readonly DynamicMemberResolver dynamicMemberResolver;
        private readonly MapEmitter emitter;
        private int lambdaMethodCounter;

        public MapFactory(IMemberMappingsResolver memberMappingsResolver, DynamicMemberResolver dynamicMemberResolver, MapEmitter mapEmitter)
        {
            this.memberMappingsResolver = memberMappingsResolver;
            this.emitter = mapEmitter;
	        this.dynamicMemberResolver = dynamicMemberResolver;
        }

        public Func<TSource, TDest> BuildMapMethod<TSource, TDest>(IMappingImplementation mappingImplementation)
        {
            var memberMappings = memberMappingsResolver.ResolveMappings(mappingImplementation);

	        var builders = dynamicMemberResolver.ResolveBuilders(mappingImplementation);
	        var dynType = builders.Item1;
	        var dynMethod = builders.Item2;

			var memberBindings = dynamicMemberResolver.ResolveMemberBindings(memberMappings, l => CreateLambdaMethod(l, dynType));

	        var gen = dynMethod.GetILGenerator();
            var def = mappingImplementation.Default != null 
                ? CreateDefaultMethod(mappingImplementation.Default, dynType)
                : null;

            emitter.Init(gen, typeof(TSource), def);

            if (mappingImplementation.Create != null)
            {
                var creator = CreateLambdaMethod(mappingImplementation.Create, dynType);
                emitter.Create(gen, creator);
            }
            else
            {
                emitter.Newobj(gen);
            }
            
            emitter.Map(gen, memberBindings);
            emitter.Return(gen);

            dynType.CreateType();
            var methodToken = dynMethod.GetToken().Token;
            var methodInfo = (MethodInfo)DynamicModuleHost.Module.ResolveMethod(methodToken);

            return (Func<TSource, TDest>)Delegate.CreateDelegate(typeof(Func<TSource, TDest>), methodInfo);
        }

        private MethodInfo CreateLambdaMethod(LambdaExpression lambda, TypeBuilder dynType)
        {
            var p1 = lambda.Parameters[0];
            var counter = ++lambdaMethodCounter;
            var dynMethod = dynType.DefineMethod($"LM_{counter}", MethodAttributes.Static | MethodAttributes.Private, lambda.ReturnType, new[] { p1.Type });
            dynMethod.DefineParameter(1, ParameterAttributes.None, p1.Name);
            lambda.CompileToMethod(dynMethod);

            return dynMethod;
        }

        private MethodInfo CreateDefaultMethod(LambdaExpression lambda, TypeBuilder dynType)
        {
            var dynMethod = dynType.DefineMethod("Dflt", MethodAttributes.Static | MethodAttributes.Private, lambda.ReturnType, new Type[0]);
            lambda.CompileToMethod(dynMethod);

            return dynMethod;
        }
    }
}
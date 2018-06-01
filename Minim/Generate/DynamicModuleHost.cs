using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Minim.Generate
{
    internal static class DynamicModuleHost
    {
        private const string DynamicName = "Minim.Dyno";

        private static readonly object SyncRoot = new object();
        private static ModuleBuilder _dynamicModule;
        private static int _dynamicMapperCounter = 0;

        public static ModuleBuilder Module
        {
            get
            {
                var module = _dynamicModule;
                if (module == null)
                {
                    lock (SyncRoot)
                    {
                        if (_dynamicModule == null)
                        {
                            var assemblyFileName = $"{DynamicName}.dll";

                            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                                new AssemblyName($"{DynamicName}, Version=1.1.1.2"),
                                AssemblyBuilderAccess.RunAndSave);
                            _dynamicModule = assemblyBuilder.DefineDynamicModule(assemblyFileName, assemblyFileName);
                        }

                        module = _dynamicModule;
                    }
                }

                return module;
            }
        }

        private static string GetNextMapperName()
        {
            var counter = Interlocked.Increment(ref _dynamicMapperCounter);
            return $"{DynamicName}.Mapper{counter}";
        }

	    public static TypeBuilder NewMapperDynamicType()
		{
			return DynamicModuleHost.Module.DefineType(
				GetNextMapperName(),
				TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.Public);
		}
    }
}
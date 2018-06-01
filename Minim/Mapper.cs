using System;
using System.Collections.Generic;
using Minim.Generate;
using Minim.Register;
using TypeMap = System.Tuple<System.Type, System.Type>;

namespace Minim
{
    public class Mapper
    {
        private readonly Dictionary<TypeMap, IMappingImplementation> mappings;
	    private readonly DynamicMemberResolver dynamicMemberResolver;

	    public Mapper()
        {
            mappings = new Dictionary<TypeMap, IMappingImplementation>();
	        dynamicMemberResolver = new DynamicMemberResolver(mappings);
		}

        public Func<TSource, TDest> MapFunc<TSource, TDest>()
        {
	        var mapFunc = MapEssence<TSource, TDest>.MapFunc;
	        if (mapFunc != null)
				return mapFunc;

	        var key = new TypeMap(typeof(TSource), typeof(TDest));
	        if (!mappings.TryGetValue(key, out var impl))
	        {
		        impl = new MappingRegistration<TSource, TDest>();
	        }

	        var mapFactory = new MapFactory(new MemberMappingsResolver(), dynamicMemberResolver, new MapEmitter(typeof(TDest)));
	        mapFunc = MapEssence<TSource, TDest>.MapFunc = mapFactory.BuildMapMethod<TSource, TDest>(impl);
	        ExpandResolvedMappings();
	        return mapFunc;
        }

	    internal bool BuildUp<TSource, TDest>(IMappingImplementation impl)
	    {
		    if (MapEssence<TSource, TDest>.MapFunc != null)
			    return false;

		    var mapFactory = new MapFactory(new MemberMappingsResolver(), dynamicMemberResolver, new MapEmitter(typeof(TDest)));
		    MapEssence<TSource, TDest>.MapFunc = mapFactory.BuildMapMethod<TSource, TDest>(impl);
		    return true;
		}

	    private void ExpandResolvedMappings()
	    {
		    while (true)
		    {
			    var resolvedMappings = dynamicMemberResolver.CaptureResolvedMappings();
				if (resolvedMappings.Length == 0)
					break;

				foreach (var impl in resolvedMappings)
			    {
				    impl.BuildUp(this);
			    }
		    }
	    }

		public TDest Map<TSource, TDest>(TSource source)
        {
            // TODO : static MapEssence issue
            if (!mappings.ContainsKey(new TypeMap(typeof(TSource), typeof(TDest))))
                MapEssence<TSource, TDest>.MapFunc = null;

            var mapFunc = MapEssence<TSource, TDest>.MapFunc ?? MapFunc<TSource, TDest>();
            return mapFunc(source);
        }

        public IMappingRegistration<TSource, TDest> Register<TSource, TDest>()
        {
            var key = new TypeMap(typeof(TSource), typeof(TDest));
            if (mappings.ContainsKey(key))
                throw new ArgumentException();

            // TODO : static MapEssence issue
            MapEssence<TSource, TDest>.MapFunc = null;

            var mappingRegistration = new MappingRegistration<TSource, TDest>();
            mappings.Add(key, mappingRegistration);

            return mappingRegistration;
        }
    }
}

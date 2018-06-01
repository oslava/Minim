using System.Collections.Generic;
using Minim.Register;

namespace Minim.Generate
{
    public interface IMemberMappingsResolver
    {
        IList<MemberMapping> ResolveMappings(IMappingImplementation impl);
    }
}
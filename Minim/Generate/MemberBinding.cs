using System.Reflection;

namespace Minim.Generate
{
    public class MemberBinding
    {
        public MemberInfo Target { get; set; }
        public MemberInfo Member { get; set; }
        public MethodInfo Method { get; set; }
        public bool CallMethodOnMember { get; set; }
    }
}
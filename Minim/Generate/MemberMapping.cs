using System.Linq.Expressions;
using System.Reflection;

namespace Minim.Generate
{
    public class MemberMapping
    {
        public MemberInfo Target { get; set; }
        public MemberInfo Member { get; set; }
        public LambdaExpression Lambda { get; set; }
    }
}
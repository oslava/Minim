namespace Minim.Generate
{
    internal static class MemberMappingExtensions
    {
        public static bool IsVoid(this MemberMapping m)
        {
            return m.Member == null && m.Lambda == null;
        }
    }
}
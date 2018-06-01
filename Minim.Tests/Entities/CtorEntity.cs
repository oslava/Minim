namespace Minim.Tests.Entities
{
    public class CtorEntity
    {
        public const int TheValue = 401;

        public string Name { get; set; }
        public int Val { get; set; }

        public CtorEntity() : this(TheValue)
        { }

        public CtorEntity(int val)
        {
            Val = val;
        }
    }
}
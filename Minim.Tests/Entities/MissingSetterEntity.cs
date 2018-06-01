namespace Minim.Tests.Entities
{
    public class MissingSetterEntity
    {
        public const int TheValue = 777;

        public MissingSetterEntity()
        {
            Val = TheValue;
        }

        public string Name { get; set; }
        public int Val { get; }
    }
}
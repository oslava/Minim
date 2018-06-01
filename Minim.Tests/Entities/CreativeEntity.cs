namespace Minim.Tests.Entities
{
    public class CreativeEntity
    {
        private CreativeEntity()
        { }

        public static CreativeEntity Build(int val)
        {
            return new CreativeEntity
            {
                Val = val
            };
        }

        public string Name { get; set; }
        public int Val { get; private set; }
    }
}
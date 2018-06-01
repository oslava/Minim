namespace Minim.Tests.Entities
{
    public class GenericEntity<TValue>
    {
        public string Name { get; set; }
        public TValue Val { get; set; }
    }
}
using FluentAssertions;
using NUnit.Framework;

namespace Minim.Tests
{
    [TestFixture]
    public class GraphTests
    {
        [Test]
        public void MapLinkedList()
        {
            var listDepth3 = new ListNode
            {
                Next = new ListNode
                {
                    Next = new ListNode()
                }
            };

            var mapper = new Mapper();
            mapper.Register<ListNode, ListNode>();
            var dest = mapper.Map<ListNode, ListNode>(listDepth3);

            dest.Should().BeEquivalentTo(listDepth3);

            dest.Should().NotBeSameAs(listDepth3);
            dest.Next.Should().NotBeSameAs(listDepth3.Next);
            dest.Next.Next.Should().NotBeSameAs(listDepth3.Next.Next);
        }

        [Test]
        public void MapCyclicClasses()
        {
            var first = new Dual1
            {
                Opponent = new Dual2
                {
                    Opponent = new Dual1()
                }
            };

            var mapper = new Mapper();
            mapper.Register<Dual1, Dual2>();
            mapper.Register<Dual2, Dual1>();
            var dest = mapper.Map<Dual1, Dual2>(first);

            dest.Should().BeEquivalentTo(first);

            dest.Should().NotBeSameAs(first);
            dest.Opponent.Should().NotBeSameAs(first.Opponent);
            dest.Opponent.Opponent.Should().NotBeSameAs(first.Opponent.Opponent);
        }

        public class ListNode
        {
            public ListNode Next { get; set; }
        }

        public class Dual1
        {
            public Dual2 Opponent { get; set; }
        }

        public class Dual2
        {
            public Dual1 Opponent { get; set; }
        }
    }
}
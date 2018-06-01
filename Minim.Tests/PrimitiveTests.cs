using Minim.Tests.Entities;
using NUnit.Framework;

namespace Minim.Tests
{
    [TestFixture]
    public class PrimitiveTests
    {
        [Test]
        public void MapToPrimitive()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };

            var mapper = new Mapper();
            var dest = mapper.Map<SrcEntity, int>(srcEntity);

            Assert.AreEqual(0, dest);
        }

        [Test]
        public void MapToPrimitiveWith()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 507
            };
            
            var mapper = new Mapper();
            mapper.Register<SrcEntity, int>().ConstructWith(x => x.Val);
            var dest = mapper.Map<SrcEntity, int>(srcEntity);

            Assert.AreEqual(507, dest);
        }

        [Test]
        public void MapFromPrimitiveWith()
        {
            var mapper = new Mapper();
            mapper.Register<long, DestEntity>()
                .ConstructWith(x => new DestEntity
                {
                    Val = (int) x
                })
                .Ignore(x => x.Name)
                .Ignore(x => x.Val);
            var destEntity = mapper.Map<long, DestEntity>(911);

            Assert.AreEqual(null, destEntity.Name);
            Assert.AreEqual(911, destEntity.Val);
        }

        [Test]
        public void MapFromPrimitiveConfigure()
        {
            var mapper = new Mapper();
            mapper.Register<long, DestEntity>()
                .Ignore(x => x.Name)
                .Configure(x => x.Val, l => (int)l);
            var destEntity = mapper.Map<long, DestEntity>(912);

            Assert.AreEqual(null, destEntity.Name);
            Assert.AreEqual(912, destEntity.Val);
        }
    }
}
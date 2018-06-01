using System;
using FluentAssertions;
using Minim.Tests.Entities;
using NUnit.Framework;

namespace Minim.Tests
{
    [TestFixture]
    public class CastTests
    {
        public class DestEntityLongest
        {
            public string Name;
            public int Val;
            public int Val2;
        }

        public struct Strong
        {
            public int X;
            public int Y;
        }

        [Test]
        public void TypeSizeOverflow()
        {
            var srcEntity = new GenericEntity<Guid>
            {
                Name = "Slim",
                Val = Guid.NewGuid()
            };

            var mapper = new Mapper();
            mapper.Register<GenericEntity<Guid>, DestEntityLongest>().Ignore(x => x.Val2);

            mapper
                .Invoking(x => x.Map<GenericEntity<Guid>, DestEntityLongest>(srcEntity))
                .Should().Throw<InvalidCastException>();
        }

        [Test]
        public void LongToInt()
        {
            var srcEntity = new GenericEntity<Guid>
            {
                Name = "Slim",
                Val = Guid.NewGuid()
            };

            var mapper = new Mapper();
            var destEntity = mapper.Map<GenericEntity<Guid>, GenericEntity<Guid>>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(srcEntity.Val, destEntity.Val);
        }

        [Test]
        public void IntToLong()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };

            var mapper = new Mapper();
            var destEntity = mapper.Map<SrcEntity, DestEntityLong>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(1001L, destEntity.Val);
        }

        [Test]
        public void IntToUInt()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = -1001
            };

            var mapper = new Mapper();
            var destEntity = mapper.Map<SrcEntity, GenericEntity<uint>>(srcEntity);


            Assert.AreEqual("Slim", destEntity.Name);
            unchecked
            {
                Assert.AreEqual((uint)-1001, destEntity.Val);
            }
        }
    }
}
using System;
using FluentAssertions;
using Minim.Tests.Entities;
using NUnit.Framework;

namespace Minim.Tests
{
    [TestFixture]
    public class MissingMembersTests
    {
        [Test]
        public void MissingSourceMember()
        {
            var srcEntity = new PartialEntity
            {
                Name = "Slim"
            };

            var mapper = new Mapper();

            mapper
                .Invoking(x => x.Map<PartialEntity, DestEntity>(srcEntity))
                .Should().Throw<MissingMemberException>();
        }

        [Test]
        public void MissingSourceMemberWithConfiguration()
        {
            const int V = 100500;
            var srcEntity = new PartialEntity
            {
                Name = "Slim"
            };

            var mapper = new Mapper();
            mapper.Register<PartialEntity, DestEntity>().Configure(x => x.Val, part => V);
            var destEntity = mapper.Map<PartialEntity, DestEntity>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(V, destEntity.Val);
        }

        [Test]
        public void MissingDestinationMember()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };

            var mapper = new Mapper();
            var destEntity = mapper.Map<SrcEntity, PartialEntity>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
        }

        [Test]
        public void MissingDestinationSetter()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };

            var mapper = new Mapper();
            var destEntity = mapper.Map<SrcEntity, MissingSetterEntity>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(MissingSetterEntity.TheValue, destEntity.Val);
        }
    }
}
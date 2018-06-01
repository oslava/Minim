using System;
using Minim.Tests.Entities;
using NUnit.Framework;

namespace Minim.Tests
{
    [TestFixture]
    public class IgnoreTests
    {
        [Test]
        public void IgnoreMember()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };
            
            var mapper = new Mapper();
            mapper.Register<SrcEntity, CtorEntity>().Ignore(x => x.Val);
            var destEntity = mapper.Map<SrcEntity, CtorEntity>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(CtorEntity.TheValue, destEntity.Val);
        }

        [Test]
        public void IgnoreMissingSourceMember()
        {
            var srcEntity = new PartialEntity
            {
                Name = "Slim"
            };

            var mapper = new Mapper();
            mapper.Register<PartialEntity, CtorEntity>().Ignore(x => x.Val);
            var destEntity = mapper.Map<PartialEntity, CtorEntity>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(CtorEntity.TheValue, destEntity.Val);
        }

        [Test]
        public void OverrideCtorInitializationMember()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };
            
            var mapper = new Mapper();
            var destEntity = mapper.Map<SrcEntity, CtorEntity>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(1001, destEntity.Val);
        }
    }
}
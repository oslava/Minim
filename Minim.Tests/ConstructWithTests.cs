using Minim.Tests.Entities;
using NUnit.Framework;

namespace Minim.Tests
{
    [TestFixture]
    public class ConstructWithTests
    {
        [Test]
        public void ConstructWithComplexCtor()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };

            var mapper = new Mapper();
            mapper.Register<SrcEntity, CtorEntity>().ConstructWith(src => new CtorEntity(src.Val));
            var destEntity = mapper.Map<SrcEntity, CtorEntity>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(1001, destEntity.Val);
        }

        [Test]
        public void ConstructWithComplexCtor_NonSourceValue()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };
            
            var mapper = new Mapper();
            mapper.Register<SrcEntity, CtorEntity>()
                .ConstructWith(src => new CtorEntity(257))
                .Ignore(jsdff => jsdff.Val);
            var destEntity = mapper.Map<SrcEntity, CtorEntity>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(257, destEntity.Val);
        }

        [Test]
        public void ConstructWithBuilder()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };

            var mapper = new Mapper();
            mapper.Register<SrcEntity, CreativeEntity>().ConstructWith(kgva => CreativeEntity.Build(kgva.Val));
            var destEntity = mapper.Map<SrcEntity, CreativeEntity>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(1001, destEntity.Val);
        }

        [Test]
        public void ConstructWithForValueType()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };

            var mapper = new Mapper();
            mapper.Register<SrcEntity, DeStruct>().ConstructWith(src => new DeStruct(src));
            var destStruct = mapper.Map<SrcEntity, DeStruct>(srcEntity);

            Assert.AreEqual("Slim", destStruct.Name);
            Assert.AreEqual(1001, destStruct.Val);
            Assert.AreEqual(srcEntity, destStruct.TheThing);
        }

        public struct DeStruct
        {
            public DeStruct(object theThing)
            {
                TheThing = theThing;

                Name = "a";
                Val = 3;
            }

            public string Name { get; set; }
            public long Val { get; set; }
            public object TheThing { get; }
        }
    }
}
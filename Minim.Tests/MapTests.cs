using System;
using FluentAssertions;
using Minim.Register;
using Minim.Tests.Entities;
using NUnit.Framework;

namespace Minim.Tests
{
    [TestFixture]
    public class MapTests
    {
        [Test]
        public void MapProperties()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };
            
            var mapper = new Mapper();
            var destEntity = mapper.Map<SrcEntity, DestEntity>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(1001, destEntity.Val);
        }

        [Test]
        public void MapPropertyToField()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };

            var mapper = new Mapper();
            var fieldEntity = mapper.Map<SrcEntity, FieldEntity>(srcEntity);

            Assert.AreEqual("Slim", fieldEntity.name);
            Assert.AreEqual(1001, fieldEntity.Val);
        }

        [Test]
        public void MapFieldToProperty()
        {
            var fieldEntity = new FieldEntity
            {
                name = "Slim",
                Val = 1001
            };

            var mapper = new Mapper();
            var destEntity = mapper.Map<FieldEntity, DestEntity>(fieldEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(1001, destEntity.Val);
        }

        [Test]
        public void MapSelf()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };

            var mapper = new Mapper();
            var destEntity = mapper.Map<SrcEntity, SrcEntity>(srcEntity);

            Assert.AreNotSame(srcEntity, destEntity);
            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(1001, destEntity.Val);
        }

        [Test]
        public void MultiMap()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };

            var mapper = new Mapper();
            var destEntity = mapper.Map<SrcEntity, DestEntity>(srcEntity);

            var mapper2 = new Mapper();
            var result = mapper2.Map<DestEntity, DestEntityLong>(destEntity);

            Assert.AreEqual("Slim", result.Name);
            Assert.AreEqual(1001L, result.Val);
        }

        [Test]
        public void MapGeneric()
        {
            var srcEntity = new SrcEntity
            {
                Name = "Slim",
                Val = 1001
            };

            var mapper = new Mapper();
            var destEntity = mapper.Map<SrcEntity, GenericEntity<int>>(srcEntity);

            Assert.AreEqual("Slim", destEntity.Name);
            Assert.AreEqual(1001, destEntity.Val);
		}

	    [Test]
	    public void MapToStruct()
	    {
		    var srcEntity = new SrcEntity
		    {
			    Name = "Slim",
			    Val = 1001
		    };

		    var mapper = new Mapper();
		    var destStruct = mapper.Map<SrcEntity, MyStruct>(srcEntity);
			
		    Assert.AreEqual("Slim", destStruct.Name);
		    Assert.AreEqual(1001L, destStruct.Val);
        }

        [Test]
	    public void MapFromStruct()
	    {
		    var valueType = new MyStruct
			{
			    Name = "Slim",
			    Val = 1001
		    };

		    var mapper = new Mapper();
		    var destEntity = mapper.Map<MyStruct, DestEntity>(valueType);

		    Assert.AreEqual("Slim", destEntity.Name);
		    Assert.AreEqual(1001L, destEntity.Val);
        }

        [Test]
        public void Map_null_Should_return_null_When_dest_is_class()
        {
            var mapper = new Mapper();
            mapper.Register<SrcEntity, DestEntity>();
            var destEntity = mapper.Map<SrcEntity, DestEntity>(null);

            destEntity.Should().BeNull();
        }

        [Test]
        public void Map_null_Should_return_default_When_dest_is_struct()
        {
            var mapper = new Mapper();
            mapper.Register<SrcEntity, MyStruct>();
            var destEntity = mapper.Map<SrcEntity, MyStruct>(null);

            destEntity.Should().BeEquivalentTo(new MyStruct());
        }

        [Test]
        public void Map_null_Should_ignore_ConstructWith()
        {
            var mapper = new Mapper();
            mapper.Register<SrcEntity, DestEntity>()
                .ConstructWith(x => new DestEntity { Name = "type S", Val = 456 });
            var destEntity = mapper.Map<SrcEntity, DestEntity>(null);

            destEntity.Should().BeNull();
        }

        [Test]
        public void Map_null_Should_return_config_default_When_dest_is_class()
        {
            var mapper = new Mapper();
            mapper.Register<SrcEntity, DestEntity>()
                .Default(() => new DestEntity {Name = "mikapika", Val = 700});
            var destEntity = mapper.Map<SrcEntity, DestEntity>(null);

            destEntity.Should().BeEquivalentTo(new DestEntity { Name = "mikapika", Val = 700 });
        }

        [Test]
        public void Map_null_Should_return_config_default_When_dest_is_struct()
        {
            var mapper = new Mapper();
            mapper.Register<SrcEntity, MyStruct>()
                .Default(() => new MyStruct { Name = "belka", Val = 100 });
            var destEntity = mapper.Map<SrcEntity, MyStruct>(null);

            destEntity.Should().BeEquivalentTo(new MyStruct { Name = "belka", Val = 100 });
        }

        [Test]
        public void Map_config_default_Should_ignore_ConstructWith()
        {
            var mapper = new Mapper();
            mapper.Register<SrcEntity, DestEntity>()
                .ConstructWith(x => new DestEntity {Name = "type S", Val = 456})
                .Default(() => new DestEntity {Name = "mikapika", Val = 700});
            var destEntity = mapper.Map<SrcEntity, DestEntity>(null);

            destEntity.Should().BeEquivalentTo(new DestEntity { Name = "mikapika", Val = 700 });
        }

        public struct MyStruct
		{
			public string Name { get; set; }
			public long Val { get; set; }
		}
	}
}

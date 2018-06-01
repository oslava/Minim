using Minim.Tests.Entities;
using NUnit.Framework;

namespace Minim.Tests
{
	[TestFixture]
	public class ConfigureTests
	{
		[Test]
		public void MapWithConfiguration()
		{
			var srcEntity = new SrcEntity
			{
				Name = "Slim",
				Val = 1001
			};
            
            var mapper = new Mapper();
			mapper.Register<SrcEntity, DestEntity>().Configure(x => x.Val, part => part.Val * 2);
			var destEntity = mapper.Map<SrcEntity, DestEntity>(srcEntity);

			Assert.AreEqual("Slim", destEntity.Name);
			Assert.AreEqual(2002, destEntity.Val);
		}

		[Test]
		public void MapWithConst()
		{
			const int V = 100500;
			var srcEntity = new SrcEntity
			{
				Name = "Slim",
				Val = 1001
			};
            
            var mapper = new Mapper();
			mapper.Register<SrcEntity, DestEntity>().Configure(x => x.Val, part => V);
			var destEntity = mapper.Map<SrcEntity, DestEntity>(srcEntity);

			Assert.AreEqual("Slim", destEntity.Name);
			Assert.AreEqual(V, destEntity.Val);
		}

		[Test]
		[Ignore("no way to resolve")]
		public void MapWithClosureConfiguration()
		{
			int v = 100500;
			var srcEntity = new SrcEntity
			{
				Name = "Slim",
				Val = 1001
			};

			var mapper = new Mapper();
			mapper.Register<SrcEntity, DestEntity>().Configure(x => x.Val, part => v);
			var destEntity = mapper.Map<SrcEntity, DestEntity>(srcEntity);

			Assert.AreEqual("Slim", destEntity.Name);
			Assert.AreEqual(v, destEntity.Val);
		}
	}
}
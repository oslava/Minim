using Minim.Tests.Entities;
using NUnit.Framework;

namespace Minim.Tests
{
	[TestFixture]
	public class CompositionTests
	{
		[Test]
		public void NestedTest()
		{
			var mapper = new Mapper();
			mapper.Register<CompositionEntity, CompositionDestEntity>();
			mapper.Register<SrcEntity, DestEntity>();

			var entity = new CompositionEntity
			{
				NestedEntity = new SrcEntity
				{
					Name = "Slippage",
					Val = 505
				}
			};

			var compositionDestEntity = mapper.Map<CompositionEntity, CompositionDestEntity>(entity);

			Assert.IsInstanceOf<DestEntity>(compositionDestEntity.NestedEntity);
			Assert.AreEqual("Slippage", compositionDestEntity.NestedEntity.Name);
			Assert.AreEqual(505, compositionDestEntity.NestedEntity.Val);
		}

		[Test]
		public void NestedNullTest()
		{
			var mapper = new Mapper();
			mapper.Register<CompositionEntity, CompositionDestEntity>();
			mapper.Register<SrcEntity, DestEntity>();

			var entity = new CompositionEntity
			{
				NestedEntity = null
			};

			var compositionDestEntity = mapper.Map<CompositionEntity, CompositionDestEntity>(entity);

			Assert.IsNull(compositionDestEntity.NestedEntity);
		}
	}

	public class CompositionEntity
	{
		public SrcEntity NestedEntity { get; set; }
	}

	public class CompositionDestEntity
	{
		public DestEntity NestedEntity { get; set; }
	}
}
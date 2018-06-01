using System;

namespace Minim
{
	// TODO : static class shared by all Mapper instances 
	internal static class MapEssence<TSource, TDest>
	{
		public static Func<TSource, TDest> MapFunc;
	}
}
using System;
using FluentAssertions;
using Minim.Tests.Entities;
using NUnit.Framework;

namespace Minim.Tests
{
    public class DecimalCasts
    {
        private static readonly object[] Cases =
        {
            1234567890d,
            0d,
            -1000d,
            1d,

            0f,
            -1000f,
            1f,

            int.MaxValue,
            int.MinValue,
            0,

            uint.MaxValue,
            uint.MinValue,

            long.MaxValue,
            long.MinValue,
            0L,

            ulong.MaxValue,
            ulong.MinValue,

            short.MaxValue,
            short.MinValue,
            (short) 0,

            ushort.MaxValue,
            ushort.MinValue,

            char.MaxValue,
            char.MinValue,
            'p',

            sbyte.MaxValue,
            sbyte.MinValue,
            (sbyte)0,

            byte.MaxValue,
            byte.MinValue
        };

        [Test]
        [TestCaseSource(nameof(Cases))]
        public void DecimalToTests(object value)
        {
            var methodInfo = typeof(DecimalCasts).GetMethod("DecimalTo").MakeGenericMethod(value.GetType());
            methodInfo.Invoke(null, new[] { value });
        }

        [Test]
        [TestCaseSource(nameof(Cases))]
        public void ToDecimalTests(object value)
        {
            var methodInfo = typeof(DecimalCasts).GetMethod("ToDecimal").MakeGenericMethod(value.GetType());
            methodInfo.Invoke(null, new[] {value});
        }

        public static void DecimalTo<T>(T value)
        {
            var srcEntity = new GenericEntity<decimal>
            {
                Val = typeof(T) == typeof(char) ? Convert.ToDecimal((ushort)(Convert.ToChar(value))) : Convert.ToDecimal(value)
            };

            var destEntity = new Mapper().Map<GenericEntity<decimal>, GenericEntity<T>>(srcEntity);
            destEntity.Val.Should().Be(value);
        }

        public static void ToDecimal<T>(T value)
        {
            var d = typeof(T) == typeof(char) ? Convert.ToDecimal((ushort)(Convert.ToChar(value))) : Convert.ToDecimal(value);
            var srcEntity = new GenericEntity<T>
            {
                Val = value
            };

            var destEntity = new Mapper().Map<GenericEntity<T>, GenericEntity<Decimal>>(srcEntity);
            destEntity.Val.Should().Be(d);
        }
    }
}
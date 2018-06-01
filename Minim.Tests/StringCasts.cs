using System;
using FluentAssertions;
using Minim.Tests.Entities;
using NUnit.Framework;

namespace Minim.Tests
{
    [TestFixture]
    public class StringCasts
    {
        private static readonly object[] Cases =
        {
            double.MaxValue,
            double.MinValue,
            0d,
            1d,

            float.MaxValue,
            float.MinValue,
            0f,
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
            (short)0,

            ushort.MaxValue,
            ushort.MinValue,

            char.MaxValue,
            char.MinValue,
            'p',

            sbyte.MaxValue,
            sbyte.MinValue,
            (sbyte)0,

            byte.MaxValue,
            byte.MinValue,

            Guid.NewGuid(),
        };

        [Test]
        [TestCaseSource(nameof(Cases))]
        public void StringToTests(object value)
        {
            var methodInfo = typeof(StringCasts).GetMethod("StringTo").MakeGenericMethod(value.GetType());
            methodInfo.Invoke(null, new[] { value });
        }

        [Test]
        [TestCaseSource(nameof(Cases))]
        public void ToStringTests(object value)
        {
            var methodInfo = typeof(StringCasts).GetMethod("ToStrCast").MakeGenericMethod(value.GetType());
            methodInfo.Invoke(null, new[] { value });
        }

        public static void StringTo<T>(T value)
        {
            var srcEntity = new GenericEntity<string>
            {
                Val = GetString(value)
            };

            var destEntity = new Mapper().Map<GenericEntity<string>, GenericEntity<T>>(srcEntity);
            destEntity.Val.Should().Be(value);
        }

        public static void ToStrCast<T>(T value)
        {
            var s = GetString(value);
            var srcEntity = new GenericEntity<T>
            {
                Val = value
            };

            var destEntity = new Mapper().Map<GenericEntity<T>, GenericEntity<string>>(srcEntity);
            destEntity.Val.Should().Be(s);
        }

        private static string GetString<T>(T value)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Single:
                    return Convert.ToSingle(value).ToString("R");

                case TypeCode.Double:
                    return Convert.ToDouble(value).ToString("R");

                default:
                    return value.ToString();
            }
        }
    }
}
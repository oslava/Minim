using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Minim.Tests.Entities;
using NUnit.Framework;

namespace Minim.Tests
{
    [TestFixture]
    public class EnumTests
    {
        [Test]
        [TestCaseSource(nameof(AllSources))]
        public void Enums(Sources s)
        {
            var src = new SrcStruct
            {
                Val = s
            };

            var mapper = new Mapper();
            var dest = mapper.Map<SrcStruct, DestStruct>(src);

            dest.Val.Should().Be((Dests)s);
        }

        [Test]
        [TestCaseSource(nameof(AllSources))]
        public void IntToEnum(Sources s)
        {
            var src = new GenericEntity<int>
            {
                Val = (int)s
            };

            var mapper = new Mapper();
            var dest = mapper.Map<GenericEntity<int>, SrcStruct>(src);

            dest.Val.Should().Be(s);
        }

        [Test]
        [TestCaseSource(nameof(AllSources))]
        public void ByteToEnum(Sources s)
        {
            var src = new GenericEntity<byte>
            {
                Val = (byte)s
            };

            var mapper = new Mapper();
            var dest = mapper.Map<GenericEntity<byte>, SrcStruct>(src);

            dest.Val.Should().Be(s);
        }

        [Test]
        [TestCaseSource(nameof(AllSources))]
        public void LongToEnum(Sources s)
        {
            var src = new GenericEntity<long>
            {
                Val = (long)s
            };

            var mapper = new Mapper();
            var dest = mapper.Map<GenericEntity<long>, SrcStruct>(src);

            dest.Val.Should().Be(s);
        }

        [Test]
        [TestCaseSource(nameof(AllSources))]
        public void EnumToByte(Sources s)
        {
            var src = new SrcStruct
            {
                Val = s
            };

            var mapper = new Mapper();
            mapper.Register<SrcStruct, GenericEntity<byte>>().Ignore(x => x.Name);
            var dest = mapper.Map<SrcStruct, GenericEntity<byte>>(src);

            dest.Val.Should().Be((byte)s);
        }

        [Test]
        [TestCaseSource(nameof(AllSources))]
        public void EnumToLong(Sources s)
        {
            var src = new SrcStruct
            {
                Val = s
            };

            var mapper = new Mapper();
            mapper.Register<SrcStruct, GenericEntity<long>>().Ignore(x => x.Name);
            var dest = mapper.Map<SrcStruct, GenericEntity<long>>(src);

            dest.Val.Should().Be((long)s);
        }

        private static IEnumerable<Sources> AllSources()
        {
            return Enum.GetValues(typeof(Sources)).Cast<Sources>();
        }

        public enum Sources
        {
            Zero,
            One,
            Two
        }

        public enum Dests
        {
            None,
            First,
            Second
        }

        public struct SrcStruct
        {
            public Sources Val;
        }

        public struct DestStruct
        {
            public Dests Val;
        }
    }
}
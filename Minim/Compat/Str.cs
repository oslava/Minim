using System;

namespace Minim.Compat
{
    public static class Str
    {
        public static string FromSingle(float x) => x.ToString("R");
        public static string FromDouble(double x) => x.ToString("R");
        public static string FromDecimal(decimal x) => x.ToString();
        public static string FromInt64(long x) => x.ToString();
        public static string FromUInt64(ulong x) => x.ToString();
        public static string FromInt32(int x) => x.ToString();
        public static string FromUInt32(uint x) => x.ToString();
        public static string FromInt16(short x) => x.ToString();
        public static string FromUInt16(ushort x) => x.ToString();
        public static string FromByte(byte x) => x.ToString();
        public static string FromSByte(sbyte x) => x.ToString();
        public static string FromChar(char x) => new string(x, 1);
        public static string FromDateTime(DateTime x) => x.ToString();
        public static string FromBoolean(bool x) => x.ToString();
    }
}
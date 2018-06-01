using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Minim.Compat
{
    internal static class TypeCompat
    {
        static readonly Type TDecimal = typeof(decimal);
        static readonly Type TStr = typeof(Str);

        public static bool TryConvert(ILGenerator il, Type from, Type to)
        {
            if (from == typeof(string))
                return TryParseString(il, to);

            var toCode = Type.GetTypeCode(to);
            if (toCode == TypeCode.Object)
                return false;

            Func<Type, ILGenerator, bool> func;
            return converts.TryGetValue(toCode, out func) && func(from, il);
        }

        private static bool TryParseString(ILGenerator il, Type to)
        {
            var parseMi = to.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new[] { typeof(string) }, null);
            if (parseMi == null)
                return false;

            il.Emit(OpCodes.Call, parseMi);
            return true;
        }

        private static Dictionary<TypeCode, Func<Type, ILGenerator, bool>> converts = new Dictionary<TypeCode, Func<Type, ILGenerator, bool>>
        {
            { TypeCode.Int64, (from, il) => Conv64(from, il, OpCodes.Conv_I8) },
            { TypeCode.UInt64, (from, il) => Conv64(from, il, OpCodes.Conv_U8) },
            { TypeCode.Int32, (from, il) => Conv32(from, il, OpCodes.Conv_I4) },
            { TypeCode.UInt32, (from, il) => Conv32(from, il, OpCodes.Conv_U4) },
            { TypeCode.Int16, (from, il) => Conv16(from, il, OpCodes.Conv_I2) },
            { TypeCode.UInt16, (from, il) => Conv16(from, il, OpCodes.Conv_U2) },
            { TypeCode.Char, (from, il) => Conv16(from, il, OpCodes.Conv_U2) },
            { TypeCode.SByte, (from, il) => Conv8(from, il, OpCodes.Conv_I1) },
            { TypeCode.Byte, (from, il) => Conv8(from, il, OpCodes.Conv_U1) },
            { TypeCode.Double, (from, il) => ConvR(from, il, OpCodes.Conv_R8) },
            { TypeCode.Single, (from, il) => ConvR(from, il, OpCodes.Conv_R4) },
            { TypeCode.Decimal, ConvD },
            { TypeCode.String, ConvS },
        };

        private static bool ConvD(Type from, ILGenerator il)
        {
            ConstructorInfo ctor = null;
            var typeCode = Type.GetTypeCode(from);
            switch (typeCode)
            {
                case TypeCode.Int32:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Char:
                    ctor = TDecimal.GetConstructor(new[] { typeof(int) });
                    break;
                case TypeCode.UInt32:
                    ctor = TDecimal.GetConstructor(new[] { typeof(uint) });
                    break;
                case TypeCode.Int64:
                    ctor = TDecimal.GetConstructor(new[] { typeof(long) });
                    break;
                case TypeCode.UInt64:
                    ctor = TDecimal.GetConstructor(new[] { typeof(ulong) });
                    break;
                case TypeCode.Single:
                    ctor = TDecimal.GetConstructor(new[] { typeof(float) });
                    break;
                case TypeCode.Double:
                    ctor = TDecimal.GetConstructor(new[] { typeof(double) });
                    break;

                default:
                    return false;
            }
            
            if (ctor == null)
                throw new MissingMemberException();

            il.Emit(OpCodes.Newobj, ctor);
            return true;
        }

        private static bool ConvS(Type from, ILGenerator il)
        {
            var typeCode = Type.GetTypeCode(from);
            switch (typeCode)
            {
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Boolean:
                case TypeCode.DateTime:
                    var miToR = TStr.GetMethod("From" + typeCode, BindingFlags.Static | BindingFlags.Public);
                    if (miToR == null)
                        throw new MissingMemberException();
                    
                    il.Emit(OpCodes.Call, miToR);
                    return true;

                default:
                    if (from.IsValueType)
                        il.Emit(OpCodes.Box, from);

                    var objToString = typeof(object).GetMethod("ToString", BindingFlags.Instance | BindingFlags.Public);
                    if (objToString == null)
                        throw new MissingMemberException();
                    
                    il.Emit(OpCodes.Callvirt, objToString);
                    return true;
            }
        }

        private static bool ConvR(Type from, ILGenerator il, OpCode conv)
        {
            var typeCode = Type.GetTypeCode(from);
            switch (typeCode)
            {
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.Double:
                    il.Emit(conv);
                    return true;

                case TypeCode.Decimal:
                    var name = conv == OpCodes.Conv_R8 ? "ToDouble" : "ToSingle";
                    var methodInfo = TDecimal.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
                    if (methodInfo == null)
                        throw new MissingMemberException();

                    il.Emit(OpCodes.Call, methodInfo);
                    return true;

                default:
                    return false;
            }
        }

        private static bool Conv64(Type from, ILGenerator il, OpCode conv)
        {
            var typeCode = Type.GetTypeCode(from);
            switch (typeCode)
            {
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return true;

                case TypeCode.Single:
                case TypeCode.Double:
                    il.Emit(conv);
                    return true;

                case TypeCode.Decimal:
                    var name = conv == OpCodes.Conv_I8 ? "ToInt64" : "ToUInt64";
                    var methodInfo = TDecimal.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
                    if (methodInfo == null)
                        throw new MissingMemberException();

                    il.Emit(OpCodes.Call, methodInfo);
                    return true;

                default:
                    return false;
            }
        }

        private static bool Conv32(Type from, ILGenerator il, OpCode conv)
        {
            var typeCode = Type.GetTypeCode(from);
            switch (typeCode)
            {
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return true;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                    il.Emit(conv);
                    return true;

                case TypeCode.Decimal:
                    var name = conv == OpCodes.Conv_I4 ? "ToInt32" : "ToUInt32";
                    var methodInfo = TDecimal.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
                    if (methodInfo == null)
                        throw new MissingMemberException();

                    il.Emit(OpCodes.Call, methodInfo);
                    return true;

                default:
                    return false;
            }
        }

        private static bool Conv16(Type from, ILGenerator il, OpCode conv)
        {
            var typeCode = Type.GetTypeCode(from);
            switch (typeCode)
            {
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return true;

                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                    il.Emit(conv);
                    return true;

                case TypeCode.Decimal:
                    var name = conv == OpCodes.Conv_I2 ? "ToInt16" : "ToUInt16";
                    var methodInfo = TDecimal.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
                    if (methodInfo == null)
                        throw new MissingMemberException();

                    il.Emit(OpCodes.Call, methodInfo);
                    return true;

                default:
                    return false;
            }
        }

        private static bool Conv8(Type from, ILGenerator il, OpCode conv)
        {
            var typeCode = Type.GetTypeCode(from);
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                    return true;

                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                    il.Emit(conv);
                    return true;

                case TypeCode.Decimal:
                    var name = conv == OpCodes.Conv_I1 ? "ToSByte" : "ToByte";
                    var methodInfo = TDecimal.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
                    if (methodInfo == null)
                        throw new MissingMemberException();

                    il.Emit(OpCodes.Call, methodInfo);
                    return true;

                default:
                    return false;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Minim.Compat;

namespace Minim.Generate
{
    public class MapEmitter
    {
	    private readonly Type dest;
	    private LocalBuilder localValueType;
        private Action<ILGenerator> emitDefault;
        private Action<ILGenerator> emitLoadArg;
        private bool localOnStack;

        public MapEmitter(Type dest)
	    {
		    this.dest = dest;
	    }

	    public void Init(ILGenerator gen, Type src, MethodInfo def)
	    {
	        if (dest.IsValueType)
	        {
	            localValueType = gen.DeclareLocal(dest);
	        }

            if (src.IsValueType)
            {
                emitLoadArg = il => il.Emit(OpCodes.Ldarga_S, (byte)0);
				return;
            }

            emitLoadArg = il => il.Emit(OpCodes.Ldarg_0);

            gen.Emit(OpCodes.Ldarg_0);
			var ifNull = gen.DefineLabel();
			gen.Emit(OpCodes.Brfalse, ifNull);

			emitDefault = il =>
			{
				il.MarkLabel(ifNull);
			    if (def != null)
			    {
			        il.Emit(OpCodes.Call, def);
                }
			    else
			    {
			        if (dest.IsValueType)
			        {
			            il.Emit(OpCodes.Ldloca_S, localValueType);
			            il.Emit(OpCodes.Initobj, dest);
			            il.Emit(OpCodes.Ldloc_0);
			        }
			        else
			        {
			            il.Emit(OpCodes.Ldnull);
			        }
			    }
			    il.Emit(OpCodes.Ret);
			};
		}

        public void Newobj(ILGenerator gen)
		{
			if (dest.IsValueType)
			{
			    gen.Emit(OpCodes.Ldloca_S, localValueType);
                gen.Emit(OpCodes.Initobj, dest);
			}
			else
			{
				var constructorInfo = dest.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);
				gen.Emit(OpCodes.Newobj, constructorInfo);
			}
		}

        public void Create(ILGenerator gen, MethodInfo creator)
        {
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, creator);

            localOnStack = localValueType != null;
        }

        public void Map(ILGenerator gen, IList<MemberBinding> memberMappings)
        {
            if (memberMappings.Count == 0)
                return;

            if (localValueType != null)
            {
                if (localOnStack)
                {
                    gen.Emit(OpCodes.Stloc_0);
                    localOnStack = false;
                }

                gen.Emit(OpCodes.Ldloca_S, localValueType);
            }

            foreach (var m in memberMappings)
            {
                gen.Emit(OpCodes.Dup);

                Type fromType;
                if (m.Method != null)
                {
					if (m.CallMethodOnMember)
						EmitMemberGet(gen, m.Member);
                    else
					    gen.Emit(OpCodes.Ldarg_0);

                    gen.Emit(OpCodes.Call, m.Method);
                    fromType = m.Method.ReturnType;
                }
                else
                {
	                fromType = EmitMemberGet(gen, m.Member);
                }

                if (m.Target.MemberType == MemberTypes.Property)
                {
                    var propertyInfo = (PropertyInfo)m.Target;
                    Convert(fromType, propertyInfo.PropertyType, gen);

                    var setter = propertyInfo.GetSetMethod();
                    gen.Emit(OpCodes.Callvirt, setter);
                }
                else
                {
                    var fieldInfo = (FieldInfo)m.Target;
                    Convert(fromType, fieldInfo.FieldType, gen);

                    gen.Emit(OpCodes.Stfld, fieldInfo);
                }
            }

	        if (localValueType != null)
                gen.Emit(OpCodes.Pop);
        }

        public void Return(ILGenerator gen)
        {
            if (localValueType != null)
            {
                if (!localOnStack)
                    gen.Emit(OpCodes.Ldloc_0);
            }

            gen.Emit(OpCodes.Ret);
            emitDefault?.Invoke(gen);
        }

        private Type EmitMemberGet(ILGenerator gen, MemberInfo member)
        {
            emitLoadArg(gen);

            if (member.MemberType == MemberTypes.Property)
			{
				var propertyInfo = (PropertyInfo)member;
				var getter = propertyInfo.GetGetMethod();
				gen.Emit(OpCodes.Callvirt, getter);

				return propertyInfo.PropertyType;
			}
			else
			{
				var fieldInfo = (FieldInfo)member;
				gen.Emit(OpCodes.Ldfld, fieldInfo);

				return fieldInfo.FieldType;
			}
		}

        private static void Convert(Type fromType, Type toType, ILGenerator gen)
        {
            if (fromType == toType || fromType.IsSubclassOf(toType))
                return;

            var converted = TypeCompat.TryConvert(gen, fromType, toType);
            if (!converted)
                throw new InvalidCastException($"Cannot convert {fromType.Name} to {toType.Name}.");
        }
    }
}
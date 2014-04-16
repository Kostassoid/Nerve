namespace Kostassoid.Nerve.Core.Tools
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;

	internal class FastInvoker
	{
		internal delegate object FastInvokeHandler(object target,
								   object[] paramters);

		private static void EmitCastToReference(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }
 
        private static void EmitBoxIfNeeded(ILGenerator il, Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
        }
 
        private static void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }
 
            if (value > -129 && value < 128)
            {
                il.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, value);
            }
        }

		public static FastInvokeHandler GetMethodInvoker(MethodInfo methodInfo)
		{
			var dynamicMethod = new DynamicMethod(string.Empty,
				typeof (object), new Type[]
								 {
									 typeof (object),
									 typeof (object[])
								 },
				methodInfo.DeclaringType.Module);

			var il = dynamicMethod.GetILGenerator();
			var ps = methodInfo.GetParameters();
			var paramTypes = new Type[ps.Length];

			for (var i = 0; i < paramTypes.Length; i++)
			{
				paramTypes[i] = ps[i].ParameterType;
			}

			var locals = new LocalBuilder[paramTypes.Length];

			for (var i = 0; i < paramTypes.Length; i++)
			{
				locals[i] = il.DeclareLocal(paramTypes[i]);
			}

			for (var i = 0; i < paramTypes.Length; i++)
			{
				il.Emit(OpCodes.Ldarg_1);
				EmitFastInt(il, i);
				il.Emit(OpCodes.Ldelem_Ref);
				EmitCastToReference(il, paramTypes[i]);
				il.Emit(OpCodes.Stloc, locals[i]);
			}

			il.Emit(OpCodes.Ldarg_0);

			for (var i = 0; i < paramTypes.Length; i++)
			{
				il.Emit(OpCodes.Ldloc, locals[i]);
			}

			il.EmitCall(OpCodes.Call, methodInfo, null);

			if (methodInfo.ReturnType == typeof (void))
			{
				il.Emit(OpCodes.Ldnull);
			}
			else
			{
				EmitBoxIfNeeded(il, methodInfo.ReturnType);
			}

			il.Emit(OpCodes.Ret);

			return (FastInvokeHandler) dynamicMethod.CreateDelegate(typeof (FastInvokeHandler));
		}
	}
}
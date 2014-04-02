namespace Kostassoid.Nerve.Lab.Mimic
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;
	using Core;

	public class ProxyGenerator
	{
		readonly string _namespace;

		public ProxyGenerator(string ns)
		{
			_namespace = ns;
		}

		public T Generate<T>(ICell cell)
		{
			var targetType = typeof(T);
			var methods = targetType.GetMethods();
			var assemblyName = new AssemblyName(_namespace);
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(_namespace, _namespace + ".dll");
			var typeBuilder = moduleBuilder.DefineType(targetType.Name + "Proxy", TypeAttributes.Public);
  
			typeBuilder.AddInterfaceImplementation(targetType);  
			var ctorBuilder = typeBuilder.DefineConstructor(  
                  MethodAttributes.Public,  
                  CallingConventions.Standard,  
                  new Type[] { });  

			var ilGenerator = ctorBuilder.GetILGenerator();  
			ilGenerator.EmitWriteLine("Creating Proxy instance");  
			ilGenerator.Emit(OpCodes.Ret);  
			foreach (var methodInfo in methods)  
			{  
				var methodBuilder = typeBuilder.DefineMethod(  
					methodInfo.Name,  
					MethodAttributes.Public | MethodAttributes.Virtual,  
					methodInfo.ReturnType,  
					methodInfo.GetParameters().Select(p => p.GetType()).ToArray()  
					);

				var methodILGen = methodBuilder.GetILGenerator();               
				if (methodInfo.ReturnType == typeof(void))  
				{  
					methodILGen.Emit(OpCodes.Ret);  
				}  
				else  
				{  
					if (methodInfo.ReturnType.IsValueType || methodInfo.ReturnType.IsEnum)  
					{
						var getMethod = typeof(Activator).GetMethod("CreateInstance",new Type[]{methodInfo.ReturnType});                          
						var lb = methodILGen.DeclareLocal(methodInfo.ReturnType);  
						methodILGen.Emit(OpCodes.Ldtoken, lb.LocalType);  
						methodILGen.Emit(OpCodes.Call, methodInfo.ReturnType.GetMethod("GetTypeFromHandle"));
						methodILGen.Emit(OpCodes.Callvirt, getMethod);  
						methodILGen.Emit(OpCodes.Unbox_Any, lb.LocalType);  
                                          
					}  
						else  
					{  
						methodILGen.Emit(OpCodes.Ldnull);  
					}  
					methodILGen.Emit(OpCodes.Ret);  
				}  
				typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);  
			}  
         
			var constructedType = typeBuilder.CreateType();  
			var instance = Activator.CreateInstance(constructedType);  
			return (T)instance; 
			
		}
	}
}
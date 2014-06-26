using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using StructureMap.TypeRules;

// ReSharper disable CheckNamespace
namespace System.Linq.Dynamic
// ReSharper restore CheckNamespace
{
	public class ClassFactory
	{
		public static readonly ClassFactory Instance = new ClassFactory();

		private readonly Cache<Signature, Type> _classes;
		private readonly ModuleBuilder _module;
		private int _classCount;

		static ClassFactory()
		{
		}

		private ClassFactory()
		{
			var name = new AssemblyName("DynamicClasses");
			AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
#if ENABLE_LINQ_PARTIAL_TRUST
			new ReflectionPermission(PermissionState.Unrestricted).Assert();
			try
			{
#endif
				_module = assembly.DefineDynamicModule("Module");
#if ENABLE_LINQ_PARTIAL_TRUST
			}
			finally
			{
				PermissionSet.RevertAssert();
			}
#endif
			_classes = new Cache<Signature, Type>(x => CreateDynamicClass(x));
			new ReaderWriterLock();
		}

		public Type GetDynamicClass(IEnumerable<DynamicProperty> properties, Type baseType=null)
		{
			return _classes[new Signature(properties,baseType)];
		}

		private Type CreateDynamicClass(Signature signature)
		{
			string typeName = "DynamicClass" + (_classCount + 1);
#if ENABLE_LINQ_PARTIAL_TRUST
				new ReflectionPermission(PermissionState.Unrestricted).Assert();
				try
				{
#endif
			TypeBuilder tb = _module.DefineType(typeName, TypeAttributes.Class |
			                                              TypeAttributes.Public, signature._baseType);
			FieldBuilder[] fields = GenerateProperties(tb, signature._properties);
			GenerateEquals(tb, fields);
			GenerateGetHashCode(tb, fields);
//			GenerateToString(tb, fields);

			Type result = tb.CreateType();
			_classCount++;
			Type type = result;
			return type;
#if ENABLE_LINQ_PARTIAL_TRUST
				}
				finally
				{
					PermissionSet.RevertAssert();
				}
#endif
		}

		private FieldBuilder[] GenerateProperties(TypeBuilder tb, DynamicProperty[] properties)
		{
			var fields = new FieldBuilder[properties.Length];
			for (int i = 0; i < properties.Length; i++)
			{
				DynamicProperty dp = properties[i];
				FieldBuilder fb = tb.DefineField("_" + dp.Name, dp.Type, FieldAttributes.Private);
				PropertyBuilder pb = tb.DefineProperty(dp.Name, PropertyAttributes.HasDefault, dp.Type, null);
				MethodBuilder mbGet = tb.DefineMethod("get_" + dp.Name,
				                                      MethodAttributes.Public | MethodAttributes.SpecialName |
				                                      MethodAttributes.HideBySig,
				                                      dp.Type, Type.EmptyTypes);
				ILGenerator genGet = mbGet.GetILGenerator();
				genGet.Emit(OpCodes.Ldarg_0);
				genGet.Emit(OpCodes.Ldfld, fb);
				genGet.Emit(OpCodes.Ret);
				MethodBuilder mbSet = tb.DefineMethod("set_" + dp.Name,
				                                      MethodAttributes.Public | MethodAttributes.SpecialName |
				                                      MethodAttributes.HideBySig,
				                                      null, new[] {dp.Type});
				ILGenerator genSet = mbSet.GetILGenerator();
				genSet.Emit(OpCodes.Ldarg_0);
				genSet.Emit(OpCodes.Ldarg_1);
				genSet.Emit(OpCodes.Stfld, fb);
				genSet.Emit(OpCodes.Ret);
				pb.SetGetMethod(mbGet);
				pb.SetSetMethod(mbSet);
				fields[i] = fb;
			}
			return fields;
		}

		private void GenerateEquals(TypeBuilder tb, IEnumerable<FieldInfo> fields)
		{
			MethodBuilder mb = tb.DefineMethod("Equals",
			                                   MethodAttributes.Public | MethodAttributes.ReuseSlot |
			                                   MethodAttributes.Virtual | MethodAttributes.HideBySig,
			                                   typeof (bool), new[] {typeof (object)});
			ILGenerator gen = mb.GetILGenerator();
			LocalBuilder other = gen.DeclareLocal(tb);
			Label next = gen.DefineLabel();
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Isinst, tb);
			gen.Emit(OpCodes.Stloc, other);
			gen.Emit(OpCodes.Ldloc, other);
			gen.Emit(OpCodes.Brtrue_S, next);
			gen.Emit(OpCodes.Ldc_I4_0);
			gen.Emit(OpCodes.Ret);
			gen.MarkLabel(next);
			foreach (FieldInfo field in fields)
			{
				Type ft = field.FieldType;
				Type ct = typeof (EqualityComparer<>).MakeGenericType(ft);
				next = gen.DefineLabel();
				gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldfld, field);
				gen.Emit(OpCodes.Ldloc, other);
				gen.Emit(OpCodes.Ldfld, field);
				gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", new[] {ft, ft}), null);
				gen.Emit(OpCodes.Brtrue_S, next);
				gen.Emit(OpCodes.Ldc_I4_0);
				gen.Emit(OpCodes.Ret);
				gen.MarkLabel(next);
			}
			gen.Emit(OpCodes.Ldc_I4_1);
			gen.Emit(OpCodes.Ret);
		}

		private void GenerateGetHashCode(TypeBuilder tb, IEnumerable<FieldInfo> fields)
		{
			MethodBuilder mb = tb.DefineMethod("GetHashCode",
			                                   MethodAttributes.Public | MethodAttributes.ReuseSlot |
			                                   MethodAttributes.Virtual | MethodAttributes.HideBySig,
			                                   typeof (int), Type.EmptyTypes);
			ILGenerator gen = mb.GetILGenerator();
			gen.Emit(OpCodes.Ldc_I4_0);
			foreach (FieldInfo field in fields)
			{
				Type ft = field.FieldType;
				Type ct = typeof (EqualityComparer<>).MakeGenericType(ft);
				gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldfld, field);
				gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", new[] {ft}), null);
				gen.Emit(OpCodes.Xor);
			}
			gen.Emit(OpCodes.Ret);
		}

		private void GenerateToString(TypeBuilder tb, IEnumerable<FieldInfo> fields)
		{
			MethodBuilder mb = tb.DefineMethod("ToString",
			                                   MethodAttributes.Public | MethodAttributes.ReuseSlot |
			                                   MethodAttributes.Virtual | MethodAttributes.HideBySig,
			                                   typeof (string), Type.EmptyTypes);


			ILGenerator gen = mb.GetILGenerator();
			var appendObject = typeof (StringBuilder).GetMethod("Append", new[] {typeof (object)});

			gen.Emit(OpCodes.Newobj, typeof (StringBuilder).GetConstructor(Type.EmptyTypes));
			gen.Emit(OpCodes.Stloc_0); // sb
			GenerateAppend(gen, "{ ");
			var fieldInfos = fields.ToList();
			for (int index = 0; index < fieldInfos.Count; index++)
			{
				if (index > 0)
					GenerateAppend(gen, ", ");

				FieldInfo field = fieldInfos[index];

				GenerateAppend(gen, field.Name);
				GenerateAppend(gen, "=");

				gen.Emit(OpCodes.Ldloc_0); // sb
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldfld, field);

				var methodInfo = appendObject;
				if (field.FieldType.IsValueType)
					gen.Emit(OpCodes.Box, field.FieldType);
				gen.Emit(OpCodes.Callvirt, methodInfo);
				gen.Emit(OpCodes.Pop);


			}

			GenerateAppend(gen, " }");

			gen.Emit(OpCodes.Ldloc_0); // sb
			gen.Emit(OpCodes.Callvirt, typeof (object).GetMethod("ToString"));
			gen.Emit(OpCodes.Stloc_1);
			gen.Emit(OpCodes.Ldloc_1);
			gen.Emit(OpCodes.Ret);
		}

		readonly static MethodInfo AppendString = typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) });
		private void GenerateAppend(ILGenerator gen, string value)
		{
			gen.Emit(OpCodes.Ldloc_0); // sb
			gen.Emit(OpCodes.Ldstr, value);
			gen.Emit(OpCodes.Callvirt, AppendString);
			gen.Emit(OpCodes.Pop);
		}
	}

	public class Cache<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> _cache;
		private readonly object _gate;
		private readonly Func<TKey, TValue> _statefulValueProvider;
		public Cache(Func<TKey, TValue> statefulValueProvider)
		{
			_cache = new Dictionary<TKey, TValue>();
			_gate = new object();
			_statefulValueProvider = statefulValueProvider;
		}
		
		public TValue this[TKey key]
		{
			get
			{
				lock (_gate)
				{
					return _cache.GetOrAdd(key, x => _statefulValueProvider(x));
				}
			}
		}
	}
}
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Tp.Core.Annotations;

// ReSharper disable once CheckNamespace
namespace System.Linq.Dynamic
{
    public class ClassFactory
    {
        private static readonly MethodInfo Equality = Reflect.GetMethod(() => string.Equals("", ""));

        [NotNull] public static readonly ClassFactory Instance = new ClassFactory();

        private readonly Cache<Signature, Type> _classes;
        private readonly ModuleBuilder _module;
        private int _classCount;

        private ClassFactory()
        {
            var name = new AssemblyName("DynamicClasses");
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            _module = assembly.DefineDynamicModule("Module");
            _classes = new Cache<Signature, Type>(CreateDynamicClass);
        }

        [NotNull]
        public Type GetDynamicClass(IEnumerable<DynamicProperty> properties, Type baseType = null)
        {
            return _classes[new Signature(properties, baseType)];
        }

        private Type CreateDynamicClass(Signature signature)
        {
            var typeName = GenerateTypeName();
            TypeBuilder tb = _module.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public, signature.BaseType);
            FieldBuilder[] fields = GenerateProperties(tb, signature.Properties);
            GenerateEquals(tb, fields);
            GenerateGetHashCode(tb, fields);
            GenerateToString(tb, fields);
            GenerateIndexer(tb, fields);

            Type result = tb.CreateType();
            return result;
        }

        private string GenerateTypeName()
        {
            string typeName = "DynamicClass" + (Interlocked.Increment(ref _classCount));
            return typeName;
        }

        private static FieldBuilder[] GenerateProperties(TypeBuilder tb, DynamicProperty[] properties)
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
                    null, new[] { dp.Type });
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

        private static void GenerateEquals(TypeBuilder tb, IEnumerable<FieldInfo> fields)
        {
            MethodBuilder mb = tb.DefineMethod("Equals",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(bool), new[] { typeof(object) });
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
                Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                next = gen.DefineLabel();
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(OpCodes.Ldloc, other);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", new[] { ft, ft }), null);
                gen.Emit(OpCodes.Brtrue_S, next);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ret);
                gen.MarkLabel(next);
            }
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Ret);
        }

        private static void GenerateGetHashCode(TypeBuilder tb, IEnumerable<FieldInfo> fields)
        {
            MethodBuilder mb = tb.DefineMethod("GetHashCode",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(int), Type.EmptyTypes);
            ILGenerator gen = mb.GetILGenerator();
            gen.Emit(OpCodes.Ldc_I4_0);
            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", new[] { ft }), null);
                gen.Emit(OpCodes.Xor);
            }
            gen.Emit(OpCodes.Ret);
        }

        private static void GenerateIndexer(TypeBuilder typeBuilder, IEnumerable<FieldInfo> fields)
        {
            const string reservedIndexerName = "Item";
            var ciDefaultMemberAttribute = typeof(DefaultMemberAttribute).GetConstructor(new[] { typeof(string) });
            var abDefaultMemberAttribute = new CustomAttributeBuilder(ciDefaultMemberAttribute, new object[] { reservedIndexerName });
            typeBuilder.SetCustomAttribute(abDefaultMemberAttribute);
            var indexerProperty = typeBuilder.DefineProperty(reservedIndexerName,
                PropertyAttributes.None,
                CallingConventions.ExplicitThis | CallingConventions.HasThis,
                returnType: typeof(object), parameterTypes: new[] { typeof(string) });

            var indexerGetter = GenerateIndexerGetter(typeBuilder, fields, reservedIndexerName);
            indexerProperty.SetGetMethod(indexerGetter);
        }

        private static MethodBuilder GenerateIndexerGetter(TypeBuilder typeBuilder, IEnumerable<FieldInfo> fields,
            string reservedIndexerName)
        {
            var indexerGetter = typeBuilder.DefineMethod("get_" + reservedIndexerName,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, typeof(object),
                new[] { typeof(string) });
            var il = indexerGetter.GetILGenerator();

            il.DeclareLocal(typeof(object));
            il.DeclareLocal(typeof(string));

            var endLabel = il.DefineLabel();


            foreach (var fieldInfo in fields)
            {
                var label = il.DefineLabel();

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldstr, fieldInfo.Name.Substring(1));
                il.Emit(OpCodes.Call, Equality);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Brtrue_S, label);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldInfo);
                if (fieldInfo.FieldType.IsValueType)
                {
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
                }
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Br, endLabel);
                il.MarkLabel(label);
            }

            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Br_S, endLabel);
            il.MarkLabel(endLabel);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            return indexerGetter;
        }

        private static void GenerateToString(TypeBuilder tb, IEnumerable<FieldInfo> fields)
        {
            MethodBuilder mb = tb.DefineMethod(nameof(ToString),
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(string), Type.EmptyTypes);


            ILGenerator gen = mb.GetILGenerator();

            gen.DeclareLocal(typeof(StringBuilder));

            var appendObject = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new[] { typeof(object) });

            gen.Emit(OpCodes.Newobj, typeof(StringBuilder).GetConstructor(Type.EmptyTypes));
            gen.Emit(OpCodes.Stloc_0); // sb
            GenerateAppend(gen, "{ ");
            var fieldInfos = fields.ToList();
            for (int index = 0; index < fieldInfos.Count; index++)
            {
                if (index > 0)
                    GenerateAppend(gen, ", ");

                FieldInfo field = fieldInfos[index];

                GenerateAppend(gen, field.Name.TrimStart('_'));
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
            gen.Emit(OpCodes.Callvirt, typeof(object).GetMethod(nameof(ToString)));
            gen.Emit(OpCodes.Ret);
        }

        private static readonly MethodInfo AppendString = typeof(StringBuilder).GetMethod("Append", new[] { typeof(string) });

        private static void GenerateAppend(ILGenerator gen, string value)
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

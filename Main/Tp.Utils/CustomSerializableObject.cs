using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

namespace Tp.Components
{
    /// <summary>
    /// Base class for objects with non-standard serialization.
    /// </summary>
    [Serializable]
    public class CustomSerializableObject : ISerializable
    {
        public CustomSerializableObject()
        {
        }

        protected CustomSerializableObject(SerializationInfo info, StreamingContext context)
        {
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                FieldInfo fieldInfo = GetType().GetField(enumerator.Name, GetBindingFlags());
                object value = info.GetValue(enumerator.Name, typeof(object));
                fieldInfo.SetValue(this, value);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            FieldInfo[] infos = GetType().GetFields(GetBindingFlags());
            foreach (FieldInfo fieldInfo in infos)
            {
                if (fieldInfo.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length > 0)
                    continue;

                object value = fieldInfo.GetValue(this);

                if (value == null)
                    continue;

                if (value is string && string.IsNullOrEmpty(value.ToString()))
                    continue;

                if ((value is ICollection) && ((ICollection) value).Count == 0)
                    continue;

                info.AddValue(fieldInfo.Name, value);
            }
        }

        private static BindingFlags GetBindingFlags()
        {
            return BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic;
        }
    }
}

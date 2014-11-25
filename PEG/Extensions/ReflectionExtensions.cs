using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace PEG.Extensions
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<PropertyInfo> GetAllPropertiesInAncestry(this Type type)
        {
            Type current = type;
            while (current != null)
            {
                foreach (var property in current.GetProperties())
                    yield return property;
                current = current.BaseType;
            }
            foreach (Type intf in type.GetInterfaces())
            {
                foreach (var property in intf.GetProperties())
                    yield return property;
            }
        }

        public static bool IsGenericList(this Type listType)
        {
            return IsType(listType, typeof(List<>)) || IsType(listType, typeof(Collection<>)) || IsType(listType, typeof(IList<>));
        }

        public static bool IsType(Type type, Type ancestor)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                    type = type.GetGenericTypeDefinition();
                if (type == ancestor)
                    return true;
                type = type.BaseType;
            }
            return false;
        }

        public static Type GetListElementType(this Type listType)
        {
            return listType.GetGenericArgument(typeof(IList<>), 0);
        }

        public static IEnumerable<Type> GetComposition(this Type type)
        {
            return type.GetAncestry(true);
        }

        public static IEnumerable<Type> GetAncestry(this Type type)
        {
            return type.GetAncestry(false);
        }

        public static string GetPath(this PropertyInfo property)
        {
            return property.DeclaringType.FullName + '.' + property.Name;
        }

        public static EnumRecord[] GetEnums(this Type type)
        {
            string[] names = Enum.GetNames(type);
            EnumRecord[] result = new EnumRecord[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i];
                FieldInfo field = type.GetField(name);
                EnumRecord record = new EnumRecord(field);
                result[i] = record;
            }
            return result;
        }

        public struct EnumRecord
        {
            public FieldInfo Field { get; set; }

            public EnumRecord(FieldInfo field)
                : this()
            {
                Field = field;
            }

            public string Name
            {
                get { return Field.Name; }
            }

            public object Value
            {
                get { return Field.GetValue(null); }
            }
        }

        private static IEnumerable<Type> GetAncestry(this Type type, bool includeInterfaces)
        {
            Type current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
            if (includeInterfaces)
            {
                Type[] interfaces = type.GetInterfaces();
                foreach (Type @interface in interfaces)
                {
                    yield return @interface;
                }
            }
        }

        public static Type GetGenericArgument(this Type type, Type typeToGetParameterFrom, int argumentIndex)
        {
            foreach (Type current in type.GetComposition())
            {
                type = current;
                if (!current.IsGenericType)
                    continue;

                Type genericTypeDefinition = current.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeToGetParameterFrom)
                    break;
            }

            Type[] genericArgs = type.GetGenericArguments();

            Type result = genericArgs[argumentIndex];
            return result;
        }
    }
}
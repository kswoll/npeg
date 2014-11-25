using System;
using System.Linq;
using System.Reflection;

namespace PEG.Extensions
{
    public static class AttributeExtensions
    {
        public static T GetAttribute<T>(this MemberInfo type) where T : Attribute
        {
            return type.GetAttributes<T>().FirstOrDefault();
        }

        public static T[] GetAttributes<T>(this MemberInfo type) where T : Attribute
        {
            return Attribute.GetCustomAttributes(type, typeof(T), true).Cast<T>().ToArray();
        }

        public static bool HasAttribute<T>(this MemberInfo type) where T : Attribute
        {
            return Attribute.IsDefined(type, typeof(T), true);
        }
    }
}
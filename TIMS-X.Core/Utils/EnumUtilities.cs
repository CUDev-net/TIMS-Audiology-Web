using System;
using System.ComponentModel;
using System.Reflection;

namespace TIMS_X.Core.Utils
{
    public static class EnumUtilities
    {
        #region EnumUtilities Members

        /// <summary>
        /// Gets the description from the enum if the description attribute has been set
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescriptionFromEnum(Enum value)
        {
            if (value == null)
                return string.Empty;
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// Gets the enum value from an enum type based on the description
        /// </summary>
        /// <param name="value"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static object GetEnumFromDescription(string value, Type enumType)
        {
            string[] names = Enum.GetNames(enumType);
            foreach (string name in names)
            {
                if (GetDescriptionFromEnum((Enum)Enum.Parse(enumType, name)).Equals(value))
                {
                    return Enum.Parse(enumType, name);
                }
            }

            throw new ArgumentException("The string is not a description or value of the specified enum.");
        }

        /// <summary>
        /// Parse enum
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEnum Parse<TEnum>(object value) where TEnum : struct
        {
            TEnum result;

            if (Enum.TryParse(value.ToString(), true, out result))
            {
                return result;
            }

            return default(TEnum);
        }

        #endregion EnumUtilities Members

    }
}

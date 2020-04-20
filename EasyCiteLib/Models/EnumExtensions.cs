using System;
using System.ComponentModel;
using System.Reflection;

namespace EasyCiteLib.Models
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                        return attr.Description;

                    return field.Name;
                }
            }

            return null;
        }

        public static string GetHelpText(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    HelpTextAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(HelpTextAttribute)) as HelpTextAttribute;
                    if (attr != null)
                        return attr.HelpText;
                }
            }

            return null;
        }
    }

    public class HelpTextAttribute : Attribute
    {
        public string HelpText { get; private set; }
        public HelpTextAttribute(string helpText)
        {
            HelpText = helpText;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Enums.Enums
{
    public static class EnumUtilities
    {
        #region EnumDecorations

        public class Description : Attribute
        {
            public readonly string Text;

            public Description(string text)
            {
                Text = text;
            }
        }

        public static string GetDescription(this Enum value)
        {

            Description attribute = value.GetType()
                .GetRuntimeField(value.ToString())
                .GetCustomAttributes(typeof(Description), false)
                .SingleOrDefault() as Description;
            return attribute == null ? value.ToString() : attribute.Text;
        }

        #endregion
    }
}

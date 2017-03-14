using System;
using System.Linq;
using System.Reflection;

namespace MALClient.Models.Enums
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

        public class PageIndexEnumMember : Attribute
        {
            public bool OffPage { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether page needs user library
            /// </summary>
            /// <value>
            /// <c>true</c> if [requires synchronize block]; otherwise, <c>false</c>.
            /// </value>
            public bool RequiresSyncBlock { get; set; } = false;
        }

        public class AnimeListWorkModeEnumMember : Attribute
        {
            public bool AllowLoadingMore { get; set; }
        }

        public static string GetDescription(this Enum value)
        {
            try
            {
                Description attribute = value.GetType()
                    .GetRuntimeField(value.ToString())
                    .GetCustomAttributes(typeof(Description), false)
                    .SingleOrDefault() as Description;
                return attribute == null ? value.ToString() : attribute.Text;
            }
            catch (Exception)
            {
                return value.ToString();
            }

        }

        public static TAttr GetAttribute<TAttr>(this Enum value) where TAttr : Attribute
        {
            try
            {
                var attribute = value.GetType()
                    .GetRuntimeField(value.ToString())
                    .GetCustomAttributes(typeof(TAttr), false)
                    .SingleOrDefault() as TAttr;
                return attribute;
            }
            catch (Exception)
            {
                return null;
            }

        }

        #endregion
    }
}

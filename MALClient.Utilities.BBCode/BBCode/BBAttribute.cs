using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeKicker.BBCode
{
    public class BBAttribute
    {
        public BBAttribute(string id, string name)
            : this(id, name, null, HtmlEncodingMode.HtmlAttributeEncode)
        {
        }
        public BBAttribute(string id, string name, Func<IAttributeRenderingContext, string> contentTransformer)
            : this(id, name, contentTransformer, HtmlEncodingMode.HtmlAttributeEncode)
        {
        }
        public BBAttribute(string id, string name, Func<IAttributeRenderingContext, string> contentTransformer, HtmlEncodingMode htmlEncodingMode)
        {
            if (id == null) throw new ArgumentNullException("id");
            if (name == null) throw new ArgumentNullException("name");
            if (!Enum.IsDefined(typeof(HtmlEncodingMode), htmlEncodingMode)) throw new ArgumentException("htmlEncodingMode");

            ID = id;
            Name = name;
            ContentTransformer = contentTransformer;
            HtmlEncodingMode = htmlEncodingMode;
        }

        public string ID { get; private set; } //ID is used to reference the attribute value
        public string Name { get; private set; } //Name is used during parsing
        public Func<IAttributeRenderingContext, string> ContentTransformer { get; private set; } //allows for custom modification of the attribute value before rendering takes place
        public HtmlEncodingMode HtmlEncodingMode { get; set; }

        public static Func<IAttributeRenderingContext, string> AdaptLegacyContentTransformer(Func<string, string> contentTransformer)
        {
            return contentTransformer == null ? (Func<IAttributeRenderingContext, string>)null : ctx => contentTransformer(ctx.AttributeValue);
        }
    }
    public interface IAttributeRenderingContext
    {
        BBAttribute Attribute { get; }
        string AttributeValue { get; }
        string GetAttributeValueByID(string id);
    }
}

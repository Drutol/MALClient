using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeKicker.BBCode
{
    public class BBTag
    {
        public const string ContentPlaceholderName = "content";

        public BBTag(string name, string openTagTemplate, string closeTagTemplate, bool autoRenderContent, BBTagClosingStyle tagClosingClosingStyle, Func<string, string> contentTransformer, bool enableIterationElementBehavior, params BBAttribute[] attributes)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (openTagTemplate == null) throw new ArgumentNullException("openTagTemplate");
            if (closeTagTemplate == null) throw new ArgumentNullException("closeTagTemplate");
            if (!Enum.IsDefined(typeof(BBTagClosingStyle), tagClosingClosingStyle)) throw new ArgumentException("tagClosingClosingStyle");

            Name = name;
            OpenTagTemplate = openTagTemplate;
            CloseTagTemplate = closeTagTemplate;
            AutoRenderContent = autoRenderContent;
            TagClosingStyle = tagClosingClosingStyle;
            ContentTransformer = contentTransformer;
            EnableIterationElementBehavior = enableIterationElementBehavior;
            Attributes = attributes ?? new BBAttribute[0];
        }
        
        public BBTag(string name, string openTagTemplate, string closeTagTemplate, bool autoRenderContent, BBTagClosingStyle tagClosingClosingStyle, Func<string, string> contentTransformer, params BBAttribute[] attributes)
            : this(name, openTagTemplate, closeTagTemplate, autoRenderContent, tagClosingClosingStyle, contentTransformer, false, attributes)
        {
        }

        public BBTag(string name, string openTagTemplate, string closeTagTemplate, bool autoRenderContent, bool requireClosingTag, Func<string, string> contentTransformer, params BBAttribute[] attributes)
            : this(name, openTagTemplate, closeTagTemplate, autoRenderContent, requireClosingTag ? BBTagClosingStyle.RequiresClosingTag : BBTagClosingStyle.AutoCloseElement, contentTransformer, attributes)
        {
        }

        public BBTag(string name, string openTagTemplate, string closeTagTemplate, bool autoRenderContent, bool requireClosingTag, params BBAttribute[] attributes)
            : this(name, openTagTemplate, closeTagTemplate, autoRenderContent, requireClosingTag, null, attributes)
        {
        }

        public BBTag(string name, string openTagTemplate, string closeTagTemplate, params BBAttribute[] attributes)
            : this(name, openTagTemplate, closeTagTemplate, true, true, attributes)
        {
        }

        public string Name { get; private set; }
        public string OpenTagTemplate { get; private set; }
        public string CloseTagTemplate { get; private set; }
        public bool AutoRenderContent { get; private set; }
        public bool EnableIterationElementBehavior { get; private set; }
        public bool RequiresClosingTag
        {
            get { return TagClosingStyle == BBTagClosingStyle.RequiresClosingTag; }
        }
        public BBTagClosingStyle TagClosingStyle { get; private set; }
        public Func<string, string> ContentTransformer { get; private set; } //allows for custom modification of the tag content before rendering takes place
        public BBAttribute[] Attributes { get; private set; }

        public BBAttribute FindAttribute(string name)
        {
            return Array.Find(Attributes, a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }

    public enum BBTagClosingStyle
    {
        RequiresClosingTag = 0,
        AutoCloseElement = 1,
        LeafElementWithoutContent = 2, //leaf elements have no content - they are closed immediately
    }

    public enum HtmlEncodingMode
    {
        HtmlEncode = 0,
        HtmlAttributeEncode = 1,
        UnsafeDontEncode = 2,
    }
}
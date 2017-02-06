using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MALClient.BBCode.SyntaxTree
{
    public sealed class TextNode : SyntaxTreeNode
    {
        public TextNode(string text)
            : this(text, null)
        {
        }
        public TextNode(string text, string htmlTemplate)
            : base(null)
        {
            if (text == null) throw new ArgumentNullException("text");
            Text = text;
            HtmlTemplate = htmlTemplate;
        }

        public string Text { get; private set; }
        public string HtmlTemplate { get; private set; }

        public override string ToHtml()
        {
            return HtmlTemplate == null ? WebUtility.HtmlEncode(Text) : HtmlTemplate.Replace("${content}", WebUtility.HtmlEncode(Text));
        }
        public override string ToBBCode()
        {
            return Text.Replace("\\", "\\\\").Replace("[", "\\[").Replace("]", "\\]");
        }
        public override string ToText()
        {
            return Text;
        }

        public override SyntaxTreeNode SetSubNodes(IEnumerable<SyntaxTreeNode> subNodes)
        {
            if (subNodes == null) throw new ArgumentNullException("subNodes");
            if (subNodes.Any()) throw new ArgumentException("subNodes cannot contain any nodes for a TextNode");
            return this;
        }
        internal override SyntaxTreeNode AcceptVisitor(SyntaxTreeVisitor visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");
            return visitor.Visit(this);
        }

        protected override bool EqualsCore(SyntaxTreeNode b)
        {
            var casted = (TextNode)b;
            return Text == casted.Text && HtmlTemplate == casted.HtmlTemplate;
        }
    }
}
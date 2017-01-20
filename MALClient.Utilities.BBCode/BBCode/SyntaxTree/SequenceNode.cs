using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeKicker.BBCode.SyntaxTree
{
    public sealed class SequenceNode : SyntaxTreeNode
    {
        public SequenceNode()
        {
        }
        public SequenceNode(SyntaxTreeNodeCollection subNodes)
            : base(subNodes)
        {
            if (subNodes == null) throw new ArgumentNullException("subNodes");
        }
        public SequenceNode(IEnumerable<SyntaxTreeNode> subNodes)
            : base(subNodes)
        {
            if (subNodes == null) throw new ArgumentNullException("subNodes");
        }

        public override string ToHtml()
        {
            return string.Concat(SubNodes.Select(s => s.ToHtml()).ToArray());
        }
        public override string ToBBCode()
        {
            return string.Concat(SubNodes.Select(s => s.ToBBCode()).ToArray());
        }
        public override string ToText()
        {
            return string.Concat(SubNodes.Select(s => s.ToText()).ToArray());
        }

        public override SyntaxTreeNode SetSubNodes(IEnumerable<SyntaxTreeNode> subNodes)
        {
            if (subNodes == null) throw new ArgumentNullException("subNodes");
            return new SequenceNode(subNodes);
        }
        internal override SyntaxTreeNode AcceptVisitor(SyntaxTreeVisitor visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");
            return visitor.Visit(this);
        }
        protected override bool EqualsCore(SyntaxTreeNode b)
        {
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeKicker.BBCode.SyntaxTree
{
    public abstract class SyntaxTreeNode : IEquatable<SyntaxTreeNode>
    {
        protected SyntaxTreeNode()
        {
            SubNodes = new SyntaxTreeNodeCollection();
        }
        protected SyntaxTreeNode(ISyntaxTreeNodeCollection subNodes)
        {
            SubNodes = subNodes ?? new SyntaxTreeNodeCollection();
        }
        protected SyntaxTreeNode(IEnumerable<SyntaxTreeNode> subNodes)
        {
            SubNodes = subNodes == null ? new SyntaxTreeNodeCollection() : new SyntaxTreeNodeCollection(subNodes);
        }

        public override string ToString()
        {
            return ToBBCode();
        }

        //not null
        public ISyntaxTreeNodeCollection SubNodes { get; private set; }

        public abstract string ToHtml();
        public abstract string ToBBCode();
        public abstract string ToText();

        public abstract SyntaxTreeNode SetSubNodes(IEnumerable<SyntaxTreeNode> subNodes);
        internal abstract SyntaxTreeNode AcceptVisitor(SyntaxTreeVisitor visitor);
        protected abstract bool EqualsCore(SyntaxTreeNode b);

        //equality members
        public bool Equals(SyntaxTreeNode other)
        {
            return this == other;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as SyntaxTreeNode);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode(); //TODO
        }

        public static bool operator ==(SyntaxTreeNode a, SyntaxTreeNode b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null)) return false;
            if (ReferenceEquals(b, null)) return false;
            if (a.GetType() != b.GetType()) return false;

            if (a.SubNodes.Count != b.SubNodes.Count) return false;
            if (!ReferenceEquals(a.SubNodes, b.SubNodes))
            {
                for (int i = 0; i < a.SubNodes.Count; i++)
                    if (a.SubNodes[i] != b.SubNodes[i]) return false;
            }

            return a.EqualsCore(b);
        }
        public static bool operator !=(SyntaxTreeNode a, SyntaxTreeNode b)
        {
            return !(a == b);
        }
    }
}
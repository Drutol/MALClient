using System;
using System.Collections.Generic;
using System.Linq;
using CodeKicker.BBCode.SyntaxTree;
using System.Diagnostics;

namespace CodeKicker.BBCode
{
    /// <summary>
    /// This class is useful for creating a custom parser. You can customize which tags are available
    /// and how they are translated to HTML.
    /// In order to use this library, we require a link to http://codekicker.de/ from you. Licensed unter the Creative Commons Attribution 3.0 Licence: http://creativecommons.org/licenses/by/3.0/.
    /// </summary>
    public class BBCodeParser
    {
        public BBCodeParser(IList<BBTag> tags)
            : this(ErrorMode.ErrorFree, null, tags)
        {
        }

        public BBCodeParser(ErrorMode errorMode, string textNodeHtmlTemplate, IList<BBTag> tags)
        {
            if (!Enum.IsDefined(typeof(ErrorMode), errorMode)) throw new ArgumentOutOfRangeException("errorMode");
            if (tags == null) throw new ArgumentNullException("tags");

            ErrorMode = errorMode;
            TextNodeHtmlTemplate = textNodeHtmlTemplate;
            Tags = tags;
        }

        public IList<BBTag> Tags { get; private set; }
        public string TextNodeHtmlTemplate { get; private set; }
        public ErrorMode ErrorMode { get; private set; }

        public virtual string ToHtml(string bbCode)
        {
            if (bbCode == null) throw new ArgumentNullException("bbCode");
            return ParseSyntaxTree(bbCode).ToHtml();
        }
        public virtual SequenceNode ParseSyntaxTree(string bbCode)
        {
            if (bbCode == null) throw new ArgumentNullException("bbCode");

            Stack<SyntaxTreeNode> stack = new Stack<SyntaxTreeNode>();
            var rootNode = new SequenceNode();
            stack.Push(rootNode);

            int end = 0;
            while (end < bbCode.Length)
            {
                if (MatchTagEnd(bbCode, ref end, stack))
                    continue;

                if (MatchStartTag(bbCode, ref end, stack))
                    continue;

                if (MatchTextNode(bbCode, ref end, stack))
                    continue;

                if (ErrorMode != ErrorMode.ErrorFree)
                    throw new BBCodeParsingException(""); //there is no possible match at the current position

                AppendText(bbCode[end].ToString(), stack); //if the error free mode is enabled force interpretation as text if no other match could be made
                end++;
            }

            Debug.Assert(end == bbCode.Length); //assert bbCode was matched entirely

            while (stack.Count > 1) //close all tags that are still open and can be closed implicitly
            {
                var node = (TagNode)stack.Pop();
                if (node.Tag.RequiresClosingTag && ErrorMode == ErrorMode.Strict) throw new BBCodeParsingException("TagNotClosed");
            }

            if (stack.Count != 1)
            {
                Debug.Assert(ErrorMode != ErrorMode.ErrorFree);
                throw new BBCodeParsingException(""); //only the root node may be left
            }

            return rootNode;
        }

        bool MatchTagEnd(string bbCode, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            int end = pos;

            var tagEnd = ParseTagEnd(bbCode, ref end);
            if (tagEnd != null)
            {
                while (true)
                {
                    var openingNode = stack.Peek() as TagNode; //could also be a SequenceNode
                    if (openingNode == null && ErrorOrReturn("TagNotOpened", tagEnd)) return false;
                    Debug.Assert(openingNode != null); //ErrorOrReturn will either or throw make this stack frame exit

                    if (!openingNode.Tag.Name.Equals(tagEnd, StringComparison.OrdinalIgnoreCase))
                    {
                        //a nesting imbalance was detected

                        if (openingNode.Tag.RequiresClosingTag && ErrorOrReturn("TagNotMatching", tagEnd, openingNode.Tag.Name))
                            return false;
                        else
                            stack.Pop();
                    }
                    else
                    {
                        //the opening node properly matches the closing node
                        stack.Pop();
                        break;
                    }
                }
                pos = end;
                return true;
            }

            return false;
        }
        bool MatchStartTag(string bbCode, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            int end = pos;
            var tag = ParseTagStart(bbCode, ref end);
            if (tag != null)
            {
                if (tag.Tag.EnableIterationElementBehavior)
                {
                    //this element behaves like a list item: it allows tags as content, it auto-closes and it does not nest.
                    //the last property is ensured by closing all currently open tags up to the opening list element

                    var isThisTagAlreadyOnStack = stack.OfType<TagNode>().Any(n => n.Tag == tag.Tag);
                    //if this condition is false, no nesting would occur anyway

                    if (isThisTagAlreadyOnStack)
                    {
                        var openingNode = stack.Peek() as TagNode; //could also be a SequenceNode
                        Debug.Assert(openingNode != null); //isThisTagAlreadyOnStack would have been false

                        if (openingNode.Tag != tag.Tag && ErrorMode == ErrorMode.Strict && ErrorOrReturn("TagNotMatching", tag.Tag.Name, openingNode.Tag.Name)) return false;

                        while (true)
                        {
                            var poppedOpeningNode = (TagNode)stack.Pop();

                            if (poppedOpeningNode.Tag != tag.Tag)
                            {
                                //a nesting imbalance was detected

                                if (openingNode.Tag.RequiresClosingTag && ErrorMode == ErrorMode.Strict && ErrorOrReturn("TagNotMatching", tag.Tag.Name, openingNode.Tag.Name))
                                    return false;
                                //close the (wrongly) open tag. we have already popped so do nothing.
                            }
                            else
                            {
                                //the opening node matches the closing node
                                //close the already open li-item. we have already popped. we have already popped so do nothing.
                                break;
                            }
                        }
                    }
                }

                stack.Peek().SubNodes.Add(tag);
                if (tag.Tag.TagClosingStyle != BBTagClosingStyle.LeafElementWithoutContent)
                    stack.Push(tag); //leaf elements have no content - they are closed immediately
                pos = end;
                return true;
            }

            return false;
        }
        bool MatchTextNode(string bbCode, ref int pos, Stack<SyntaxTreeNode> stack)
        {
            int end = pos;

            var textNode = ParseText(bbCode, ref end);
            if (textNode != null)
            {
                AppendText(textNode, stack);
                pos = end;
                return true;
            }

            return false;
        }
        void AppendText(string textToAppend, Stack<SyntaxTreeNode> stack)
        {
            var currentNode = stack.Peek();
            var lastChild = currentNode.SubNodes.Count == 0 ? null : currentNode.SubNodes[currentNode.SubNodes.Count - 1] as TextNode;

            TextNode newChild;
            if (lastChild == null)
                newChild = new TextNode(textToAppend, TextNodeHtmlTemplate);
            else
                newChild = new TextNode(lastChild.Text + textToAppend, TextNodeHtmlTemplate);

            if (currentNode.SubNodes.Count != 0 && currentNode.SubNodes[currentNode.SubNodes.Count - 1] is TextNode)
                currentNode.SubNodes[currentNode.SubNodes.Count - 1] = newChild;
            else
                currentNode.SubNodes.Add(newChild);
        }

        TagNode ParseTagStart(string input, ref int pos)
        {
            var end = pos;

            if (!ParseChar(input, ref end, '[')) return null;

            var tagName = ParseName(input, ref end);
            if (tagName == null) return null;

            var tag = Tags.SingleOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));
            if (tag == null && ErrorOrReturn("UnknownTag", tagName)) return null;

            var result = new TagNode(tag);

            var defaultAttrValue = ParseAttributeValue(input, ref end);
            if (defaultAttrValue != null)
            {
                var attr = tag.FindAttribute("");
                if (attr == null && ErrorOrReturn("UnknownAttribute", tag.Name, "\"Default Attribute\"")) return null;
                result.AttributeValues.Add(attr, defaultAttrValue);
            }

            while (true)
            {
                ParseWhitespace(input, ref end);
                var attrName = ParseName(input, ref end);
                if (attrName == null) break;

                var attrVal = ParseAttributeValue(input, ref end);
                if (attrVal == null && ErrorOrReturn("")) return null;

                if (tag.Attributes == null && ErrorOrReturn("UnknownTag", tag.Name)) return null;
                var attr = tag.FindAttribute(attrName);
                if (attr == null && ErrorOrReturn("UnknownTag", tag.Name, attrName)) return null;

                if (result.AttributeValues.ContainsKey(attr) && ErrorOrReturn("DuplicateAttribute", tagName, attrName)) return null;
                result.AttributeValues.Add(attr, attrVal);
            }
            if (!ParseChar(input, ref end, ']') && ErrorOrReturn("TagNotClosed", tagName)) return null;

            pos = end;
            return result;
        }
        string ParseTagEnd(string input, ref int pos)
        {
            var end = pos;

            if (!ParseChar(input, ref end, '[')) return null;
            if (!ParseChar(input, ref end, '/')) return null;

            var tagName = ParseName(input, ref end);
            if (tagName == null) return null;

            ParseWhitespace(input, ref end);

            if (!ParseChar(input, ref end, ']'))
            {
                if (ErrorMode == ErrorMode.ErrorFree) return null;
                else throw new BBCodeParsingException("");
            }

            pos = end;
            return tagName;
        }
        string ParseText(string input, ref int pos)
        {
            int end = pos;
            bool escapeFound = false;
            bool anyEscapeFound = false;
            while (end < input.Length)
            {
                if (input[end] == '[' && !escapeFound) break;
                if (input[end] == ']' && !escapeFound)
                {
                    if (ErrorMode == ErrorMode.Strict)
                        throw new BBCodeParsingException("NonescapedChar");
                }

                if (input[end] == '\\' && !escapeFound)
                {
                    escapeFound = true;
                    anyEscapeFound = true;
                }
                else if (escapeFound)
                {
                    if (!(input[end] == '[' || input[end] == ']' || input[end] == '\\'))
                    {
                        if (ErrorMode == ErrorMode.Strict)
                            throw new BBCodeParsingException("EscapeChar");
                    }
                    escapeFound = false;
                }

                end++;
            }

            if (escapeFound)
            {
                if (ErrorMode == ErrorMode.Strict)
                    throw new BBCodeParsingException("");
            }

            var result = input.Substring(pos, end - pos);

            if (anyEscapeFound)
            {
                var result2 = new char[result.Length];
                int writePos = 0;
                bool lastWasEscapeChar = false;
                for (int i = 0; i < result.Length; i++)
                {
                    if (!lastWasEscapeChar && result[i] == '\\')
                    {
                        if (i < result.Length - 1)
                        {
                            if (!(result[i + 1] == '[' || result[i + 1] == ']' || result[i + 1] == '\\'))
                                result2[writePos++] = result[i]; //the next char was not escapable. write the slash into the output array
                            else
                                lastWasEscapeChar = true; //the next char is meant to be escaped so the backslash is skipped
                        }
                        else
                        {
                            result2[writePos++] = '\\'; //the backslash was the last char in the string. just write it into the output array
                        }
                    }
                    else
                    {
                        result2[writePos++] = result[i];
                        lastWasEscapeChar = false;
                    }
                }
                result = new string(result2, 0, writePos);
            }

            pos = end;
            return result == "" ? null : result;
        }

        static string ParseName(string input, ref int pos)
        {
            int end = pos;
            for (; end < input.Length && (char.ToLower(input[end]) >= 'a' && char.ToLower(input[end]) <= 'z' || (input[end]) >= '0' && (input[end]) <= '9' || input[end] == '*'); end++) ;
            if (end - pos == 0) return null;

            var result = input.Substring(pos, end - pos);
            pos = end;
            return result;
        }
        static string ParseAttributeValue(string input, ref int pos)
        {
            var end = pos;

            if (end >= input.Length || input[end] != '=') return null;
            end++;

            var endIndex = input.IndexOfAny(" []".ToCharArray(), end);
            if (endIndex == -1) endIndex = input.Length;

            var valStart = pos + 1;
            var result = input.Substring(valStart, endIndex - valStart);
            pos = endIndex;
            return result;
        }
        static bool ParseWhitespace(string input, ref int pos)
        {
            int end = pos;
            while (end < input.Length && char.IsWhiteSpace(input[end]))
                end++;

            var found = pos != end;
            pos = end;
            return found;
        }
        static bool ParseChar(string input, ref int pos, char c)
        {
            if (pos >= input.Length || input[pos] != c) return false;
            pos++;
            return true;
        }

        bool ErrorOrReturn(string msgKey, params string[] parameters)
        {
            if (ErrorMode == ErrorMode.ErrorFree) return true;
            else throw new BBCodeParsingException(msgKey);
        }
    }

    public enum ErrorMode
    {
        /// <summary>
        /// Every syntax error throws a BBCodeParsingException.
        /// </summary>
        Strict,

        /// <summary>
        /// Syntax errors with obvious meaning will be corrected automatically.
        /// </summary>
        TryErrorCorrection,

        /// <summary>
        /// The parser will never throw an exception. Invalid tags like "array[0]" will be interpreted as text.
        /// </summary>
        ErrorFree,
    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Templates.Core.PostActions.Catalog.Merge
{
    public class ResourceDictionaryWriter : XmlTextWriter
    {
        private TextWriter writer;
        private const string intend = "    ";

        public ResourceDictionaryWriter(TextWriter w) : base(w)
        {
            writer = w;
        }

        public ResourceDictionaryWriter(Stream w, Encoding encoding) : base(w, encoding)
        {
        }

        public ResourceDictionaryWriter(string filename, Encoding encoding) : base(filename, encoding)
        {
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                localName = prefix + ":" + localName;
                prefix = "";
            }

            base.WriteStartElement(prefix, localName, ns);
        }

        public new void WriteAttributeString(string prefix, string localName,  string value)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                localName = prefix + ":" + localName;
            }

            WriteAttributeString(localName, value);
        }

        public void WriteResourceDictionary(XElement e)
        {
            WriteStartElement(e.Name.LocalName);
            WriteNamespaceDeclaration("xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            WriteNamespaceDeclaration("xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml");
            WriteNewLine();
            WriteNewLine();
            foreach (var node in e.Elements())
            {
                WriteElement(node);
            }
            WriteFullEndElement();
            WriteNewLine();
        }

        private void WriteElement(XElement e)
        {
            WriteComments(e);
            WriteRaw(intend);
            WriteStartElement(e.GetPrefixOfNamespace(e.Name.Namespace), e.Name.LocalName, "");
            WriteAttributes(e);
            if (e.Descendants().Count() > 0)
            {
                WriteChildElements(e);
            }
            else
            {
                WriteRaw(e.Value);
            }
            WriteEndElement();
            WriteNewLine();
            if (e.Name.LocalName == "Style")
            {
                WriteNewLine();
            }
        }

        private void WriteNewLine()
        {
            WriteRaw("\r\n");
        }

        private void WriteNamespaceDeclaration(string key, string value)
        {
            writer.WriteLine();
            WriteAttributeString("   " + key, value);
        }

        private void WriteComments(XElement e)
        {
            if (e.PreviousNode != null && e.PreviousNode.NodeType == XmlNodeType.Comment)
            {
                WriteRaw(intend);
                WriteComment((e.PreviousNode as XComment).Value);
                WriteNewLine();
            }
        }

        private void WriteChildElements(XElement e)
        {
            WriteNewLine();
            foreach (var n in e.Descendants())
            {
                WriteRaw(intend);
                WriteRaw(intend);
                WriteStartElement(e.GetPrefixOfNamespace(n.Name.Namespace), n.Name.LocalName, "");
                WriteAttributes(n);
                WriteEndElement();
                WriteNewLine();
            }
            WriteRaw(intend);
        }

        private void WriteAttributes(XElement e)
        {
            foreach (var a in e.Attributes())
            {
                WriteAttributeString(e.GetPrefixOfNamespace(a.Name.Namespace), a.Name.LocalName, a.Value);
            }
        }
    }
}

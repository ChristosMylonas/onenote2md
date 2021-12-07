namespace Onenote2md.Core
{
    using Onenote2md.Shared;
    using Onenote2md.Shared.OneNoteObjectModel;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml.Linq;

    public class MDGenerator : IPageGenerator
    {
        #region Fields
        private const string MathMLStart = "<!--[if mathML]>";
        private const string MathMLEnd = "<![endif]-->";
        private readonly OneNoteApplication oneNoteApp;
        #endregion

        #region Constructors
        public MDGenerator(OneNoteApplication oneNoteApplication)
        {
            this.oneNoteApp = oneNoteApplication;
        }
        #endregion

        public IPageLinkResolver LinkResolver { get; set; }

        #region IPageGenerator
        public MarkdownPage PreviewMD(Page page)
        {
            MDWriter tempWriter = new MDWriter(@"c:\temp\onenote2md", true);
            return DoGenerateMD(page, tempWriter);
        }

        public void GeneratePageMD(Page page, IWriter writer)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (page.PageSettings == null)
            {
                throw new ArgumentException("The page isn't populated with enough details.", nameof(page));
            }

            var md = DoGenerateMD(page, writer);
            writer.WritePage(md);
        }

        protected MarkdownPage DoGenerateMD(Page page, IWriter writer)
        {
            StringBuilder mdContent = new StringBuilder();

            // create context
            var context = new MarkdownGeneratorContext(writer, page)
            {
                PageTitle = GetPageTitle(page)
            };

            GeneratePageTitle(context, mdContent);

            if (page.Items != null && page.Items.Any())
            {
                foreach (PageObject pageObject in page.Items)
                {
                    if (pageObject is Outline outline)
                    {
                        StringBuilder outlineMarkdown = this.DoGenerateOEChildren(outline.OEChildren, context);
                        mdContent.Append(outlineMarkdown);
                    }
                    else if (pageObject is Image image)
                    {
                        string md = this.DoGenerateImage(image, context);
                        mdContent.Append(md);
                    }
                    else if (pageObject is InsertedFile insertedFile)
                    {
                        string md = this.DoGenerateInsertedFile(insertedFile, context);
                        mdContent.Append(md);
                    }
                    else if (pageObject is MediaFile mediaFile)
                    {

                    }
                    else
                    {
                        // Unsupported objects such as FutureObject, InkDrawing.
                    }
                }
            }

            MarkdownPage markdownPage = new MarkdownPage
            {
                Content = mdContent.ToString(),
                Title = context.PageTitle,
                Filename = context.GetPageFullPath()
            };
            return markdownPage;
        }

        protected StringBuilder DoGenerateOEChildren(OEChildren[] oeChildren, MarkdownGeneratorContext context)
        {
            StringBuilder markdown = new StringBuilder();
            if (oeChildren == null || !oeChildren.Any())
            {
                return markdown;
            }

            foreach (OEChildren child in oeChildren)
            {
                if (child.Items == null)
                {
                    continue;
                }

                foreach (object item in child.Items)
                {
                    if (item is OE element)
                    {
                        // An OE object is considered to be a Markdown paragraph.
                        StringBuilder paragraphMarkdown = new StringBuilder();

                        // Style
                        var quickStyleDef = context.GetQuickStyleDef(element.quickStyleIndex);
                        string paragraphStyle = quickStyleDef?.name ?? "p";
                        int headingLevel = 0;
                        switch (paragraphStyle)
                        {
                            case "h1":
                            case "h2":
                            case "h3":
                            case "h4":
                            case "h5":
                            case "h6":
                                headingLevel = paragraphStyle[1] - '0';
                                break;
                            case "PageTitle":
                                headingLevel = 1;
                                break;
                        }

                        // Tag(s)
                        if (element.Tag != null && element.Tag.Any())
                        {
                            foreach (Tag tag in element.Tag)
                            {
                                TagDef tagDef = context.GetTagDef(tag.index);
                                string tagMD = tag.GetMD(tagDef);
                                paragraphMarkdown.Append(tagMD);
                            }
                        }

                        // List
                        if (element.List?.Item != null)
                        {
                            if (element.List.Item is Bullet)
                            {
                                paragraphMarkdown.Append("* ");
                            }
                            else if (element.List.Item is Number number)
                            {
                                paragraphMarkdown.Append(number.text + " ");
                            }
                        }

                        // Text
                        if (element.Items != null)
                        {
                            foreach (object o in element.Items)
                            {
                                if (o is TextRange textRange)
                                {
                                    string md;
                                    if (paragraphStyle == "code")
                                    {
                                        md = MDGenerator.ToPlainText(textRange);
                                    }
                                    else
                                    {
                                        md = MDGenerator.ToMarkdown(textRange, this.LinkResolver, context);
                                    }

                                    paragraphMarkdown.Append(md);
                                }
                                else if (o is Image image)
                                {
                                    string md = this.DoGenerateImage(image, context);
                                    paragraphMarkdown.Append(md);
                                }
                                else if (o is InsertedFile insertedFile)
                                {
                                    string md = this.DoGenerateInsertedFile(insertedFile, context);
                                    paragraphMarkdown.Append(md);
                                }
                                else if (o is Table table)
                                {
                                    string md = this.DoGenerateTable(table, context);
                                    paragraphMarkdown.Append(md);
                                }
                                else if (o is MediaFile mediaFile)
                                {

                                }
                                else
                                {
                                    // Unsupported objects such as FutureObject, InkDrawing, InkParagraph, InkWord.
                                }
                            }

                            if (paragraphMarkdown.Length > 0)
                            {
                                paragraphMarkdown.AppendLine();
                            }
                        }

                        // More indentented children
                        if (element.OEChildren != null && element.OEChildren.Any())
                        {
                            // Recursion
                            context.IndentLevel++;
                            StringBuilder childParagraph = this.DoGenerateOEChildren(element.OEChildren, context);
                            context.IndentLevel--;
                            paragraphMarkdown.Append(childParagraph);
                        }

                        // Determine the indent prefix
                        string indentPrefix = string.Empty;
                        if (context.IndentLevel > 0)
                        {
                            indentPrefix = new string('>', context.IndentLevel) + " ";
                        }

                        if (context.InCodeBlock && paragraphStyle != "code")
                        {
                            // Finish the previous code block
                            markdown.AppendLine(context.CodeBlockEnd);
                            context.CodeBlockEnd = null;
                            context.InCodeBlock = false;
                        }

                        // Append the paragraph to the final document
                        if (headingLevel > 0)
                        {
                            if (paragraphMarkdown.Length > 0)
                            {
                                string headingPrefix = new string('#', headingLevel) + " ";
                                markdown.Append(headingPrefix);
                                markdown.AppendLine(paragraphMarkdown.ToString());
                            }
                        }
                        else if (paragraphStyle == "code")
                        {
                            markdown.Append(indentPrefix);
                            if (!context.InCodeBlock)
                            {
                                // Start a code block. Note the block end should match.
                                markdown.AppendLine("```");
                                context.InCodeBlock = true;
                                context.CodeBlockEnd = indentPrefix + "```";
                            }

                            markdown.Append(paragraphMarkdown.ToString());
                        }
                        else // paragraphStyle in ["blockquote", "cite", "p"]
                        {
                            markdown.Append(indentPrefix);
                            markdown.Append(paragraphMarkdown.ToString());
                            if (element.List?.Item == null && context.IndentLevel == 0)
                            {
                                markdown.AppendLine();
                            }
                        }
                    }
                    else
                    {
                        // Unsupported items such as HtmlContent.
                    }
                }

                if (context.InCodeBlock)
                {
                    markdown.AppendLine(context.CodeBlockEnd);
                    context.CodeBlockEnd = null;
                    context.InCodeBlock = false;
                }
            }

            return markdown;
        }

        protected string DoGenerateImage(Image image, MarkdownGeneratorContext context)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            byte[] bytes = null;
            string fullPath = context.GetAttachmentPath(Path.GetFileNameWithoutExtension(context.Page.MarkdownFileName) + "." + image.format);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            if (image.Item is CallbackID callbackID)
            {
                string stringValue = oneNoteApp.GetBinaryPageContent(context.Page.ID, callbackID.callbackID);
                bytes = Convert.FromBase64String(stringValue);
            }
            else if (image.Item is byte[])
            {
                bytes = image.Item as byte[];
            }
            else if (image.Item is FilePath filePath)
            {
                if (File.Exists(filePath.path))
                {
                    File.Copy(filePath.path, fullPath);
                }
            }

            if (bytes != null)
            {
                context.Writer.WritePageImage(fullPath, bytes);
            }

            var imageFilename = Path.GetFileName(fullPath);
            var imageMarkdown = $"![{imageFilename}]({imageFilename.Replace(" ", "%20")})";
            return imageMarkdown;
        }

        protected string DoGenerateInsertedFile(InsertedFile file, MarkdownGeneratorContext context)
        {
            string oldPathAndName = file.pathCache;
            if (!File.Exists(oldPathAndName))
            {
                oldPathAndName = file.pathSource;
            }

            string targetPath = context.GetAttachmentPath(file.preferredName);
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            if (File.Exists(oldPathAndName))
            {
                File.Copy(oldPathAndName, targetPath);
            }

            string actualFileName = Path.GetFileName(targetPath);
            string markdown = $"[{actualFileName}]({actualFileName.Replace(" ", "%20")})";
            return markdown;
        }

        protected string DoGenerateTable(Table table, MarkdownGeneratorContext context)
        {
            StringBuilder markdown = new StringBuilder();
            if (table == null || table.Row == null || table.Row.FirstOrDefault()?.Cell == null)
            {
                return markdown.ToString();
            }

            for (int r = 0; r < table.Row.Length; r++)
            {
                foreach (Cell cell in table.Row[r].Cell)
                {
                    markdown.Append("| ");
                    markdown.Append(this.DoGenerateOEChildren(cell.OEChildren, context).ToString().Trim());
                    markdown.Append(" ");
                }

                markdown.AppendLine("|");

                if (r == 0)
                {
                    for (int i = 0; i < table.Columns.Length; i++)
                    {
                        markdown.Append("| - ");
                    }

                    markdown.AppendLine("|");
                }
            }

            return markdown.ToString();
        }
        #endregion

        #region Generation helpers
        protected string GetPageTitle(Page page)
        {
            var result = string.Empty;
            if (page?.Title?.OE?.Any() == true)
            {
                // Find the first TextRange(<one:T>) child element.
                if (page.Title.OE[0].Items?.FirstOrDefault(o => o is TextRange) is TextRange textRange)
                {
                    result = ToPlainText(textRange);
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                // If a page is not given a title, the page.name is auto-generated such as "untitled page".
                result = page.name;
            }
            
            return result;
        }

        protected void GeneratePageTitle(MarkdownGeneratorContext context, StringBuilder results)
        {
            results.AppendLine("# " + MDGenerator.EscapeMarkdown(context.PageTitle));
        }

        private static string ToPlainText(TextRange textRange)
        {
            if (textRange == null || string.IsNullOrEmpty(textRange.Value))
            {
                return string.Empty;
            }

            if (textRange.Value.StartsWith(MDGenerator.MathMLStart))
            {
                return textRange.Value;
            }

            // The textRange.Value is a multi-line HTML segment and is HTML encoded.
            // *** Example value: ***
            // <span
            // style='font-family:"Microsoft YaHei"' lang=zh-CN>格式测试</span><span
            // style='font-family:Calibri' lang=en-US>&lt;</span><span style='font-family:
            // "Microsoft YaHei"' lang=en-US>FormatTest&gt;</span>
            // *** End of example value ***
            string result = Regex.Replace(textRange.Value, @"</?\w+[^>]*?>", string.Empty);
            result = HttpUtility.HtmlDecode(result);
            return result;
        }

        /// <summary>
        /// Convert the <one:T> tag and its value to Markdown source with format.
        /// </summary>
        /// <param name="textRange"></param>
        /// <returns></returns>
        private static string ToMarkdown(TextRange textRange, IPageLinkResolver linkResolver, MarkdownGeneratorContext context)
        {
            if (textRange == null || string.IsNullOrEmpty(textRange.Value))
            {
                return string.Empty;
            }

            StringBuilder markdown = new StringBuilder(textRange.Value.Length);
            if (textRange.Value.StartsWith(MDGenerator.MathMLStart))
            {
                // This is a MathML formular
                markdown.Append(textRange.Value);
            }
            else
            {
                Stack<HtmlToken> openTags = new Stack<HtmlToken>();
                int i = 0;
                do
                {
                    HtmlToken token = NextToken(textRange.Value, i);
                    if (token == null)
                    {
                        break;
                    }

                    i += token.ParsedLength;
                    if (token.IsBeginTag && token.IsEndTag)
                    {
                        // Single tag
                        if (token.TagName == "br")
                        {
                            // Do nothing here. A new line will be appended at the end of the 
                        }
                    }
                    else if (token.IsBeginTag)
                    {
                        openTags.Push(token);
                        if (token.TagName == "a")
                        {
                            markdown.Append('[');
                        }

                        if (token.Format.Ignore)
                        {
                            continue;
                        }

                        // Bold and italic both uses '*" and Markdown doesn't define the ambiguity
                        // especially when nested. We do not render both formats for a single tag.
                        // However the generated Markdown might be incorrect if there're nested tags
                        // with bold and italic formats.
                        if (token.Format.Bold)
                        {
                            markdown.Append("**");
                        }
                        else if (token.Format.Italic)
                        {
                            markdown.Append("*");
                        }

                        if (token.Format.Highlighted)
                        {
                            markdown.Append("==");
                        }

                        if (token.Format.LineThrough)
                        {
                            markdown.Append("~~");
                        }

                        if (token.Format.Subscript)
                        {
                            markdown.Append("<sub>");
                        }

                        if (token.Format.Superscript)
                        {
                            markdown.Append("<sup>");
                        }

                        if (token.Format.Underlined)
                        {
                            markdown.Append("<u>");
                        }
                    }
                    else if (token.IsEndTag)
                    {
                        if (openTags.Count == 0)
                        {
                            throw new ArgumentException(String.Format("Cannot find the matching tag for </{0}> in {1}.", token.TagName, textRange.Value));
                        }

                        HtmlToken beginToken = openTags.Pop();
                        TagFormat format = beginToken.Format;
                        if (format.TagName != token.TagName)
                        {
                            throw new ArgumentException(String.Format("The begin tag <{0}> mismatches for </{1}> in {2}.", format.TagName, token.TagName, textRange.Value));
                        }

                        // The end tag order should be reversed as the begin tag order.
                        if (format.Underlined)
                        {
                            markdown.Append("</u>");
                        }

                        if (format.Superscript)
                        {
                            markdown.Append("</sup>");
                        }

                        if (format.Subscript)
                        {
                            markdown.Append("</sub>");
                        }

                        if (format.LineThrough)
                        {
                            markdown.Append("~~");
                        }

                        if (format.Highlighted)
                        {
                            markdown.Append("==");
                        }

                        if (format.Bold)
                        {
                            markdown.Append("**");
                        }
                        else if (format.Italic)
                        {
                            markdown.Append("*");
                        }

                        if (beginToken.TagName == "a")
                        {
                            string link = beginToken.href;
                            if (linkResolver != null)
                            {
                                link = linkResolver.ResolvePageLink(link, context.Page.MarkdownRelativePath);
                                link = link.Replace(" ", "%20");
                            }

                            markdown.Append("](" + link + ")");
                        }
                    }
                    else if (token.IsText)
                    {
                        if (token.TagName == "mml")
                        {
                            markdown.Append('\'');
                            markdown.Append(token.Text);
                            markdown.Append('\'');
                        }
                        else
                        {
                            string plainText = HttpUtility.HtmlDecode(token.Text);
                            string mdSource = MDGenerator.EscapeMarkdown(plainText);
                            markdown.Append(mdSource);
                        }
                    }
                } while (i < textRange.Value.Length);
            }

            return markdown.ToString();
        }

        /// <summary>
        /// Escape the special characters in Markdown.
        /// The <see cref="clearText"/> should be what you would like to see in a rendered Markdown document.
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        private static string EscapeMarkdown(string clearText)
        {
            if (string.IsNullOrEmpty(clearText))
            {
                return string.Empty;
            }

            HashSet<char> specialChars = new HashSet<char>(new char[] { '\\', '`', '*', '_', '{', '}', '[', ']', '#', '+', '!', '|', '<', '&' });
            StringBuilder markdownSource = new StringBuilder(clearText.Length * 2);
            for (int i = 0; i < clearText.Length; i++)
            {
                char c = clearText[i];
                if (specialChars.Contains(c))
                {
                    markdownSource.Append(@"\" + c);
                }
                else if (c == '=')
                {
                    // Only "==" (markdown highlight) needs to be escaped
                    if (i + 1 < clearText.Length && clearText[i + 1] == '=')
                    {
                        markdownSource.Append(@"\=");
                    }
                }
                else
                {
                    markdownSource.Append(c);
                }
            }

            return markdownSource.ToString();
        }

        private static HtmlToken NextToken(string value, int startIndex)
        {
            if (string.IsNullOrEmpty(value) || startIndex >= value.Length)
            {
                return null;
            }

            HtmlToken token = new HtmlToken();
            if (value.Length >= (startIndex + MDGenerator.MathMLStart.Length)
                && value.Substring(startIndex, MDGenerator.MathMLStart.Length) == MDGenerator.MathMLStart)
            {
                int endIndex = value.IndexOf(MDGenerator.MathMLEnd, startIndex);
                if (endIndex < 0)
                {
                    throw new ArgumentException($"Can't find the end of MathML: {value}@{startIndex}");
                }

                token.TagName = "mml";
                token.IsText = true;
                token.Text = value.Substring(startIndex, endIndex - startIndex + MDGenerator.MathMLEnd.Length);
                token.ParsedLength = token.Text.Length;
            }
            else if (value.Length >= (startIndex + 2) && value.Substring(startIndex, 2) == "</")
            {
                token.IsEndTag = true;
            }
            else if (value[startIndex] == '<')
            {
                token.IsBeginTag = true;
            }
            else
            {
                token.IsText = true;
                int nextTagIndex = value.IndexOf('<', startIndex);
                if (nextTagIndex < 0)
                {
                    token.Text = value.Substring(startIndex);
                }
                else
                {
                    token.Text = value.Substring(startIndex, nextTagIndex - startIndex);
                }

                token.ParsedLength = token.Text.Length;
            }

            if (token.IsBeginTag || token.IsEndTag)
            {
                Regex tagRegex = new Regex(@"\G</?(?<tagName>\w+)(?<attributes>[^>]*?)>", RegexOptions.Compiled);
                Match m = tagRegex.Match(value, startIndex);
                if (m.Success)
                {
                    token.TagName = m.Groups["tagName"].Value;
                    token.ParsedLength = m.Length;
                    token.Format = new TagFormat()
                    {
                        TagName = token.TagName,
                        Ignore = true,
                    };

                    string attributes = m.Groups["attributes"].Value;
                    if (string.Compare(token.TagName, "br", StringComparison.OrdinalIgnoreCase) == 0 || m.Value.EndsWith("/>"))
                    {
                        token.IsEndTag = true;
                        token.TagName = "br";
                    }
                    else if (string.Compare(token.TagName, "a", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        token.TagName = "a";
                        Dictionary<string, string> attrDict = MDGenerator.ParseHtmlAttributes(attributes);
                        if (attrDict.ContainsKey("href"))
                        {
                            token.href = attrDict["href"];
                        }
                    }

                    // Parse formats                    
                    if (attributes.Contains("text-decoration:underline"))
                    {
                        token.Format.Underlined = true;
                        token.Format.Ignore = false;
                    }

                    if (attributes.Contains("font-weight:bold"))
                    {
                        token.Format.Bold = true;
                        token.Format.Ignore = false;
                    }

                    if (attributes.Contains("font-style:italic"))
                    {
                        token.Format.Italic = true;
                        token.Format.Ignore = false;
                    }

                    if (attributes.Contains("background:yellow"))
                    {
                        token.Format.Highlighted = true;
                        token.Format.Ignore = false;
                    }

                    if (attributes.Contains("text-decoration:line-through"))
                    {
                        token.Format.LineThrough = true;
                        token.Format.Ignore = false;
                    }

                    if (attributes.Contains("vertical-align:sub"))
                    {
                        token.Format.Subscript = true;
                        token.Format.Ignore = false;
                    }

                    if (attributes.Contains("vertical-align:super"))
                    {
                        token.Format.Superscript = true;
                        token.Format.Ignore = false;
                    }
                }
                else
                {
                    throw new ArgumentException($"The tag regular expression missed the match unexpectedly: {value}@{startIndex}");
                }
            }

            return token;
        }

        private static Dictionary<string, string> ParseHtmlAttributes(string attributes)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(attributes))
            {
                return result;
            }

            foreach (string attribute in Regex.Split(attributes, @"\s"))
            {
                if (string.IsNullOrWhiteSpace(attribute))
                {
                    continue;
                }

                int valueIndex = attribute.IndexOf('=');
                if (valueIndex < 0)
                {
                    result[attribute] = attribute;
                }
                else
                {
                    string name = attribute.Substring(0, valueIndex);
                    string value = attribute.Substring(valueIndex + 1);
                    if (value.StartsWith("'"))
                    {
                        value = value.Trim('\'');
                    }
                    else if (value.StartsWith("\""))
                    {
                        value = value.Trim('"');
                    }

                    value = HttpUtility.HtmlDecode(value);
                    result[name.ToLowerInvariant()] = value;
                }
            }

            return result;
        }

        private class HtmlToken
        {
            public bool IsBeginTag { get; set; }

            public bool IsEndTag { get; set; }

            public bool IsText { get; set; }

            public int ParsedLength { get; set; }

            public string TagName { get; set; }

            public string Text { get; set; }

            public string href { get; set; }

            public TagFormat Format { get; set; }
        }

        /// <summary>
        /// An HTML tag with formats. In OneNote I see <span> only, but this can be any tag
        /// like <span>, <font>. We only care about formats with markdown equivalents.
        /// Unsupported formats are ignored.
        /// </summary>
        private class TagFormat
        {
            public string TagName { get; set; }

            public bool Bold { get; set; }

            public bool Italic { get; set; }

            public bool Highlighted { get; set; }

            public bool LineThrough { get; set; }

            public bool Subscript { get; set; }

            public bool Superscript { get; set; }

            public bool Underlined { get; set; }

            /// <summary>
            /// Indicates this is an empty tag or all formats are ignored.
            /// </summary>
            public bool Ignore { get; set; }
        }
        #endregion
    }
}

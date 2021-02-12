using Onenote2md.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Onenote2md.Core
{
    public class MDGenerator : IGenerator
    {
        #region Fields
        private NotebookParser parser;
        private XNamespace ns;
        private Microsoft.Office.Interop.OneNote.Application onenoteApp;

        static Dictionary<string, string> spanReplacements = new Dictionary<string, string>()
        {
            { "<span style='font-weight:bold'>", " **" },
            { "<span style='font-weight:bold;text-decoration:underline'>", " **" }
        };
        #endregion

        #region Constructors
        public MDGenerator(NotebookParser parser)
        {
            this.parser = parser;
            this.onenoteApp = this.parser.GetOneNoteApp();

            var doc = parser.GetXDocument(
                null, Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks);
            ns = doc.Root.Name.Namespace;
        }
        #endregion

        #region Helpers
        protected string NormalizeName(string source)
        {
            string ns = "{http://schemas.microsoft.com/office/onenote/2013/onenote}";
            if (String.IsNullOrWhiteSpace(source))
                return source;

            if (source.StartsWith(ns))
                source = source.Replace(ns, "");

            return source;
        }

        protected string TextReplacement(string source)
        {
            return source.Replace("&nbsp;**", "**");
        }

        protected string ReplaceMultiline(string source)
        {
            return source.Replace("\n", " ");
        }

        protected string ConvertSpanToMd(string source)
        {
            foreach (var item in spanReplacements)
            {
                if (source.Contains(item.Key))
                {
                    source = source.Replace(item.Key, item.Value);
                    source = source.Replace("** ", "**");
                    source = source.Replace("</span>&nbsp;", item.Value.Trim());
                    source = source.Replace("</span>", item.Value.Trim());
                    //source = source.Replace("&nbsp;>", " ");
                    break;
                }
            }

            return source;
        }

        protected string GetAttibuteValue(XElement element, string attributeName)
        {
            var v = element.Attributes().Where(q => q.Name == attributeName).FirstOrDefault();
            if (v != null)
                return v.Value;
            else
                return null;
        }

        protected string GetElementValue(XElement element)
        {
            return element.Value;
        }
        #endregion

        #region IGenerator
        public MarkdownPage PreviewMD(string parentId)
        {
            MDWriter tempWriter = new MDWriter(@"c:\temp\onenote2md", true);
            return DoGenerateMD(parentId, tempWriter);
        }

        public void GeneratePageMD(string parentId, IWriter writer)
        {
            var md = DoGenerateMD(parentId, writer);
            writer.WritePage(md);
        }

        public void GenerateSectionMD(string sectionName, IWriter writer)
        {
            var sectionId = parser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections, sectionName);

            GenerateSectionMD(sectionId, sectionName, writer);
        }

        public void GenerateSectionMD(string sectionId, string sectionName, IWriter writer)
        {
            if (!String.IsNullOrEmpty(sectionId))
            {
                var pageIds = parser.GetChildObjectIds(
                    sectionId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren,
                    ObjectType.Page);

                try
                {
                    writer.PushDirectory(sectionName);

                    foreach (var pageId in pageIds)
                    {
                        GeneratePageMD(pageId, writer);
                    }
                }
                finally
                {
                    writer.PopDirectory();
                }
            }
        }

        public void GenerateSectionGroupMD(string sectionGroupId, string sectionGroupName, IWriter writer)
        {
            if (!String.IsNullOrEmpty(sectionGroupId))
            {
                var subSectionGroups = parser.GetChildObjectMap(
                    sectionGroupId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren,
                    ObjectType.SectionGroup);

                try
                {
                    writer.PushDirectory(sectionGroupName);

                    foreach (var item in subSectionGroups)
                    {
                        GenerateSectionGroupMD(item.Key, item.Value, writer);
                    }

                    var subSection = parser.GetChildObjectMap(
                        sectionGroupId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren,
                        ObjectType.Section);

                    foreach (var section in subSection)
                    {
                        GenerateSectionMD(section.Key, section.Value, writer);
                    }
                }
                finally
                {
                    writer.PopDirectory();
                }
            }
        }


        public void GenerateNotebookMD(string notebookName, IWriter writer)
        {
            var notebookId = parser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks, notebookName);

            if (!String.IsNullOrEmpty(notebookId))
            {
                var subSectionGroups = parser.GetChildObjectMap(
                    notebookId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren,
                    ObjectType.SectionGroup);

                foreach (var item in subSectionGroups)
                {
                    GenerateSectionGroupMD(item.Key, item.Value, writer);
                }

                var subSection = parser.GetChildObjectMap(
                    notebookId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren,
                    ObjectType.Section);

                foreach (var section in subSection)
                {
                    GenerateSectionMD(section.Key, section.Value, writer);
                }
            }
        }

        protected MarkdownPage DoGenerateMD(string parentId, IWriter writer)
        {
            MarkdownPage markdownPage = new MarkdownPage();

            var scope = Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren;
            var doc = parser.GetXDocument(parentId, scope);

            StringBuilder mdContent = new StringBuilder();

            // create context
            var quickStyles = GetQuickStyleDef(doc);
            var tags = GetTagDef(doc);
            var context = new MarkdownGeneratorContext(writer, parentId, quickStyles, tags);
            var pageTitle = GetPageTitle(doc);
            context.SetPageTitle(pageTitle);

            var titleElement = GetTitleElement(doc);
            GenerateChildObjectMD(titleElement, context, 0, mdContent);


            var childenContent = DoGenerateMDRoots("OEChildren", doc, context);
            if (String.IsNullOrWhiteSpace(childenContent))
            {
                var directImageContent = DoGenerateMDRoots("Image", doc, context);
                if (!String.IsNullOrWhiteSpace(directImageContent))
                    mdContent.Append(directImageContent);
            }
            else
            {
                mdContent.Append(childenContent);
            }


            markdownPage.Content = mdContent.ToString();
            markdownPage.Title = context.GetPageTitle();
            markdownPage.Filename = context.GetPageFullPath();

            return markdownPage;
        }

        protected string DoGenerateMDRoots(string rootNodeName, XDocument doc, MarkdownGeneratorContext context)
        {
            var result = new StringBuilder();

            var children = doc.Descendants(ns + rootNodeName).FirstOrDefault();
            if (children != null && children.HasElements)
            {
                var rootElements = children
                    .Elements()
                    .ToList();
                foreach (var rootElement in rootElements)
                {
                    int level = 0;
                    GenerateChildObjectMD(rootElement, context, level, result);
                }

                if (context.HasPairedContent())
                {
                    result.Append(context.Get().Content);
                    context.Reset();
                }
            }

            return result.ToString();
        }
        #endregion

        #region Generation helpers
        protected Dictionary<string, QuickStyleDef> GetQuickStyleDef(XDocument doc)
        {
            var nodeName = "QuickStyleDef";

            var result = new Dictionary<string, QuickStyleDef>();
            var quickStyleDefs = doc.Descendants(ns + nodeName);
            if (quickStyleDefs != null)
            {
                foreach (var item in quickStyleDefs)
                {
                    QuickStyleDef def = new QuickStyleDef();
                    def.Index = GetAttibuteValue(item, "index");
                    def.Name = GetAttibuteValue(item, "name");
                    result.Add(def.Index, def);
                }

            }

            return result;
        }

        protected Dictionary<string, TagDef> GetTagDef(XDocument doc)
        {
            var nodeName = "TagDef";

            var result = new Dictionary<string, TagDef>();
            var tagDefs = doc.Descendants(ns + nodeName);
            if (tagDefs != null)
            {
                foreach (var item in tagDefs)
                {
                    var def = new TagDef();
                    def.Index = GetAttibuteValue(item, "index");
                    def.Name = GetAttibuteValue(item, "name");
                    def.Symbol = GetAttibuteValue(item, "symbol");
                    def.Type = GetAttibuteValue(item, "type");
                    result.Add(def.Index, def);
                }
            }

            return result;
        }

        protected string GetPageTitle(XDocument doc)
        {
            var nodeName = "Title";

            var result = "";
            var element = doc.Descendants(ns + nodeName).FirstOrDefault();
            if (element != null)
            {
                var title = element.Descendants(ns + "OE").FirstOrDefault();
                if (title != null)
                    return title.Value.ToString();
            }

            return result;
        }

        protected XElement GetTitleElement(XDocument doc)
        {
            var nodeName = "Title";

            var element = doc.Descendants(ns + nodeName).FirstOrDefault();

            return element;
        }

        protected void GenerateChildObjectMD(
            XElement node, MarkdownGeneratorContext context, long level, StringBuilder results)
        {
            if (node != null)
            {
                string name = NormalizeName(node.Name.ToString());
                bool stdTraversal = true;


                StringBuilder content = new StringBuilder();
                switch (name)
                {
                    case "OE":
                        {
                            if (context.HasPairedContent())
                            {
                                content.Append(context.Get().Content);
                                context.Reset();
                            }

                            if (context.TableInfo.IsOnTable())
                            {

                            }
                            else
                            {
                                var quickStyleIndex = GetAttibuteValue(node, "quickStyleIndex");
                                if (!String.IsNullOrEmpty(quickStyleIndex))
                                {
                                    var quickStyleDef = context.GetQuickStyleDef(quickStyleIndex);
                                    if (quickStyleDef != null)
                                    {
                                        var mdContent = quickStyleDef.GetMD();
                                        if (!mdContent.WillAppendLine())
                                            content.AppendLine();
                                        context.Set(mdContent);
                                        content.Append(mdContent.Content);
                                    }
                                }
                            }
                        }
                        break;

                    case "T":
                        {
                            string v = ReplaceMultiline(node.Value);

                            v = ConvertSpanToMd(v);
                            v = TextReplacement(v);
                            content.Append(v);
                        }
                        break;

                    case "Bullet":
                        {
                            content.Append("- ");
                        }
                        break;

                    case "Number":
                        {
                            content.Append("1. ");
                        }
                        break;

                    case "Tag":
                        {
                            var tagIndex = GetAttibuteValue(node, "index");
                            var tagDef = context.GetTagDef(tagIndex);

                            if (tagDef != null)
                            {
                                if (tagDef.Name.Equals("To Do", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var completed = GetAttibuteValue(node, "completed");
                                    if (completed == "true")
                                        content.Append("- [x] ");
                                    else
                                        content.Append("- [ ] ");
                                }
                                else
                                {
                                    var tagMdContent = tagDef.GetMD();
                                    content.Append("- ");
                                    content.Append(tagMdContent.Content);
                                }
                            }
                        }
                        break;



                    case "Table":
                        {
                            if (context.HasPairedContent())
                            {
                                content.Append(context.Get().Content);
                                context.Reset();
                            }

                            stdTraversal = false;
                            context.TableInfo.SetOnTable();
                            results.Append(content);

                            if (node.HasElements)
                            {
                                var subs = node.Elements().ToList();
                                foreach (var item in subs)
                                {
                                    GenerateChildObjectMD(item, context, ++level, results);
                                }
                            }

                            results.AppendLine();
                            context.TableInfo.Reset();
                        }
                        break;

                    case "Column":
                        {
                            context.TableInfo.AppendTableColumn();
                        }
                        break;

                    case "Row":
                        {
                            if (context.TableInfo.IsOnTable())
                            {
                                if (context.TableInfo.OnHeaderRow())
                                {
                                    content.AppendLine();
                                    var columns = context.TableInfo.GetTableColumnCount();
                                    for (int i = 0; i < columns; i++)
                                    {
                                        content.Append("| - ");
                                    }
                                    content.Append("|");
                                }

                                stdTraversal = false;

                                content.AppendLine();
                                context.TableInfo.AppendRow();

                                results.Append(content);

                                if (node.HasElements)
                                {
                                    var subs = node.Elements().ToList();
                                    foreach (var item in subs)
                                    {
                                        GenerateChildObjectMD(item, context, ++level, results);
                                    }
                                }

                                results.Append(" |");
                            }
                            else
                            {
                                // how we get here?
                            }

                        }
                        break;

                    case "Cell":
                        {
                            if (context.TableInfo.IsOnTable())
                            {
                                content.Append(" | ");
                                //context.Set(new MarkdownContent("|", true));
                            }
                            else
                            {
                                // how we get here?
                            }

                        }
                        break;

                    case "Image":
                        {
                            var format = GetAttibuteValue(node, "format");
                            if (String.IsNullOrEmpty(format))
                                format = "png";
                            context.ImageDef.SetWithinImage(format);
                        }
                        break;

                    case "Size":
                        {
                            var width = GetAttibuteValue(node, "width");
                            var height = GetAttibuteValue(node, "height");


                            var w = Convert.ToDecimal(width);
                            var h = Convert.ToDecimal(height);

                            context.ImageDef.SetDimensions(w, h);

                        }
                        break;

                    case "CallbackID":
                        {
                            var id = GetAttibuteValue(node, "callbackID");

                            string stringValue;
                            onenoteApp.GetBinaryPageContent(context.ParentId, id, out stringValue);

                            if (!context.ImageDef.IsWithinImage())
                                context.ImageDef.SetWithinImage("png");

                            var fullPath = context.GetPageImageFullPath();
                            var bytes = Convert.FromBase64String(stringValue);
                            context.Writer.WritePageImage(fullPath, bytes);

                            var altText = context.GetPageImageFilename();
                            var contentFullPath = $"file://{fullPath}";
                            contentFullPath = contentFullPath.Replace(@"\", @"/");
                            contentFullPath = HttpUtility.UrlPathEncode(contentFullPath);

                            var image = $"![{altText}]({contentFullPath})";
                            //Lwn![test_2.](file://c:/Storage/Repositories/OneGitNote/Tester/aa/test_2.png)

                            content.Append(image);
                            context.ImageDef.Reset();


                        }
                        break;

                    case "OCRData":
                        {

                        }
                        break;

                    case "InsertedFile":
                        {
                            var oldPathAndName = GetAttibuteValue(node, "pathCache");
                            var newName = GetAttibuteValue(node, "preferredName");
                            var fullPath = context.GetInsertedFilePath(newName);


                            if (!File.Exists(fullPath))
                                File.Copy(oldPathAndName, fullPath);

                            var altText = newName;
                            var contentFullPath = $"file://{fullPath}";
                            contentFullPath = contentFullPath.Replace(@"\", @"/");
                            contentFullPath = HttpUtility.UrlPathEncode(contentFullPath);

                            var insertedFile = $"[{altText}]({contentFullPath})";

                            content.Append(insertedFile);
                        }
                        break;



                    default:
                        break;
                }

                if (stdTraversal)
                {
                    results.Append(content);

                    if (node.HasElements)
                    {
                        var subs = node.Elements().ToList();
                        foreach (var item in subs)
                        {
                            GenerateChildObjectMD(item, context, ++level, results);
                        }
                    }
                }
            }
        }
        #endregion
    }
}

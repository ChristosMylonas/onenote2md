using OneNoteParser.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OneNoteParser
{
    public static class Parser
    {
        private static XNamespace ns = null;
        private static Microsoft.Office.Interop.OneNote.Application onenoteApp = new Microsoft.Office.Interop.OneNote.Application();

        #region Constructors
        static Parser()
        {
            GetNamespace();
        }



        private static void GetNamespace()
        {
            string xml;
            onenoteApp.GetHierarchy(null, Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks, out xml);

            var doc = XDocument.Parse(xml);
            ns = doc.Root.Name.Namespace;
        }
        #endregion

        public static string GetObjectId(Microsoft.Office.Interop.OneNote.HierarchyScope scope, string objectName)
        {
            return GetObjectId(null, scope, objectName);
        }

        public static void Close(string notebookId)
        {
            onenoteApp.CloseNotebook(notebookId);
        }

        public static string GetObjectId(string parentId,
            Microsoft.Office.Interop.OneNote.HierarchyScope scope, string objectName)
        {
            string xml;
            onenoteApp.GetHierarchy(parentId, scope, out xml);

            var doc = XDocument.Parse(xml);
            var nodeName = "";

            switch (scope)
            {
                case (Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks):
                    nodeName = "Notebook";
                    break;

                case (Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages):
                    nodeName = "Page";
                    break;

                case (Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections):
                    nodeName = "Section";
                    break;

                default:
                    return null;
            }

            var node = doc.Descendants(ns + nodeName).Where(n => n.Attribute("name").Value == objectName).FirstOrDefault();

            if (node == null)
                return null;
            else
                return node.Attribute("ID").Value;
        }

        public static List<string> GetChildObjectNames(string parentId,
            Microsoft.Office.Interop.OneNote.HierarchyScope scope)
        {
            string xml;
            onenoteApp.GetHierarchy(parentId, scope, out xml);

            var doc = XDocument.Parse(xml);
            var nodeName = "";

            switch (scope)
            {
                case (Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks):
                    nodeName = "Notebook";
                    break;

                case (Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages):
                    nodeName = "Page";
                    break;

                case (Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections):
                    nodeName = "Section";
                    break;

                case (Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren):
                    nodeName = "OEChildren";
                    break;


                default:
                    return null;
            }

            var names = doc.Descendants(ns + nodeName)
                .Select(n => n.Attribute("name").Value)
                .ToList();

            return names;
        }

        public static List<string> GetChildObjectIDs(string parentId)
        {
            var scope = Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren;
            string xml;
            onenoteApp.GetHierarchy(parentId, scope, out xml);

            var doc = XDocument.Parse(xml);
            var nodeName = "OEChildren";

            List<string> result = new List<string>();
            var children = doc.Descendants(ns + nodeName).FirstOrDefault();
            if (children != null)
            {
                var names = children
                    .Descendants()
                    .Where(q => q.Attribute("objectID") != null)
                    .Select(q => q.Attribute("objectID").Value)
                    .ToList();
                result.AddRange(names);
            }

            return result;
        }

        public static string GetChildObjectID(string parentId, string objectID)
        {
            var scope = Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren;
            string xml;
            onenoteApp.GetHierarchy(parentId, scope, out xml);

            var doc = XDocument.Parse(xml);
            var nodeName = "OEChildren";

            var children = doc.Descendants(ns + nodeName).FirstOrDefault();
            if (children != null)
            {
                var item = children
                    .Descendants()
                    .Where(q => q.Attribute("objectID") != null)
                    .Where(q => q.Attribute("objectID").Value == objectID)
                    .FirstOrDefault();
                if (item != null)
                    return item.Value;
            }

            return null;
        }


        public static List<string> LogChildObjects(string parentId)
        {
            var scope = Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren;
            string xml;
            onenoteApp.GetHierarchy(parentId, scope, out xml);

            var doc = XDocument.Parse(xml);
            var nodeName = "OEChildren";

            List<string> results = new List<string>();
            var children = doc.Descendants(ns + nodeName).FirstOrDefault();
            if (children != null && children.HasElements)
            {
                var rootElements = children
                    .Elements()
                    //.Where(q => q.Attribute("objectID") != null)
                    //.Select(q => q.Attribute("objectID").Value)
                    .ToList();
                foreach (var rootElement in rootElements)
                {
                    int level = 0;
                    LogChildObject(rootElement, level, results);
                }
            }

            return results;
        }

        public static string NormalizeName(string source)
        {
            string ns = "{http://schemas.microsoft.com/office/onenote/2013/onenote}";
            if (String.IsNullOrWhiteSpace(source))
                return source;

            if (source.StartsWith(ns))
                source = source.Replace(ns, "");

            return source;
        }

        public static string ReplaceMultiline(string source)
        {
            return source.Replace("\n", " ");
        }

        public static void LogChildObject(XElement node, int level, List<string> results)
        {
            if (node != null)
            {
                string name = NormalizeName(node.Name.ToString());


                string content = "";
                switch (name)
                {
                    case "T":
                        content = node.Value;
                        break;

                    default:
                        break;
                }

                var s = "";
                s = s.PadLeft(level * 3);
                s += name;
                if (!String.IsNullOrEmpty(content))
                {
                    s += $" [{content}])";
                }

                results.Add(s);


                if (node.HasElements)
                {
                    var subs = node.Elements().ToList();
                    foreach (var item in subs)
                    {
                        LogChildObject(item, ++level, results);
                    }
                }
            }
        }



        public static string GetPageContent(string sectionId, string pageId)
        {
            string xml;
            onenoteApp.GetPageContent(pageId, out xml, Microsoft.Office.Interop.OneNote.PageInfo.piAll);
            var doc = XDocument.Parse(xml);
            return xml;
        }

        public static string GetAttibuteValue(XElement element, string attributeName)
        {
            var v = element.Attributes().Where(q => q.Name == attributeName).FirstOrDefault();
            if (v != null)
                return v.Value;
            else
                return null;
        }


        public static Dictionary<string, QuickStyleDef> GetQuickStyleDef(XDocument doc)
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

        public static string GetPageTitle(XDocument doc)
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

        public static XElement GetTitleElement(XDocument doc)
        {
            var nodeName = "Title";

            var element = doc.Descendants(ns + nodeName).FirstOrDefault();

            return element;
        }
        public static string GenerateMD(string parentId)
        {
            var scope = Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren;
            string xml;
            onenoteApp.GetHierarchy(parentId, scope, out xml);

            var doc = XDocument.Parse(xml);

            StringBuilder results = new StringBuilder();

            var quickStyles = GetQuickStyleDef(doc);
            var titleElement = GetTitleElement(doc);
            var context = new MarkdownGeneratorContext();

            GenerateChildObjectMD(titleElement, context, 0, quickStyles, results);


            var nodeName = "OEChildren";


            var children = doc.Descendants(ns + nodeName).FirstOrDefault();
            if (children != null && children.HasElements)
            {
                var rootElements = children
                    .Elements()
                    .ToList();
                foreach (var rootElement in rootElements)
                {
                    int level = 0;
                    GenerateChildObjectMD(rootElement, context, level, quickStyles, results);
                }

                if (context.HasPairedContent())
                {
                    results.Append(context.Get().Content);
                    context.Reset();
                }
            }

            return results.ToString();
        }


        public static void GenerateChildObjectMD(
            XElement node, MarkdownGeneratorContext context, long level, Dictionary<string, QuickStyleDef> quickStyleDefs, StringBuilder results)
        {
            if (node != null)
            {
                string name = NormalizeName(node.Name.ToString());


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

                            var quickStyleIndex = GetAttibuteValue(node, "quickStyleIndex");
                            if (quickStyleDefs.ContainsKey(quickStyleIndex))
                            {
                                var mdContent = quickStyleDefs[quickStyleIndex].GetMD();
                                if (!mdContent.WillAppendLine())
                                    content.AppendLine();
                                context.Set(mdContent);
                                content.Append(mdContent.Content);
                            }
                        }
                        break;

                    case "T":
                        {
                            string v = ReplaceMultiline(node.Value);
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
                            var completed = GetAttibuteValue(node, "completed");
                            if (completed == "true")
                                content.Append("- [x] ");
                            else
                                content.Append("- [ ] ");
                        }
                        break;

                    default:
                        break;
                }

                results.Append(content);


                if (node.HasElements)
                {
                    var subs = node.Elements().ToList();
                    foreach (var item in subs)
                    {
                        GenerateChildObjectMD(item, context, ++level, quickStyleDefs, results);
                    }
                }
            }
        }




    }
}

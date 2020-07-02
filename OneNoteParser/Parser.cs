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
        #region Singleton fields
        private static XNamespace ns = null;
        private static Microsoft.Office.Interop.OneNote.Application onenoteApp = new Microsoft.Office.Interop.OneNote.Application();
        #endregion

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


        public static List<string> GenerateMD(string parentId)
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
                    .ToList();
                foreach (var rootElement in rootElements)
                {
                    int level = 0;
                    GenerateChildObjectMD(rootElement, level, results);
                }
            }

            return results;
        }


        public static void GenerateChildObjectMD(XElement node, int level, List<string> results)
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
                        GenerateChildObjectMD(item, ++level, results);
                    }
                }
            }
        }




    }
}

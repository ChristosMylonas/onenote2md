using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Onenote2md.Core
{
    public class NotebookParser
    {
        #region Fields
        Microsoft.Office.Interop.OneNote.Application onenoteApp;
        XNamespace ns;
        #endregion

        #region Constructors
        public NotebookParser()
        {
            onenoteApp = new Microsoft.Office.Interop.OneNote.Application();

            Initialize();
        }

        private void Initialize()
        {
            string xml;
            onenoteApp.GetHierarchy(null, Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks, out xml);

            var doc = XDocument.Parse(xml);
            ns = doc.Root.Name.Namespace;
        }
        #endregion

        public Microsoft.Office.Interop.OneNote.Application GetOneNoteApp()
        {
            return onenoteApp;
        }

        public void Close(string notebookId)
        {
            onenoteApp.CloseNotebook(notebookId);
        }

        public string GetNodeNameBasedOnScope(Microsoft.Office.Interop.OneNote.HierarchyScope scope)
        {
            string nodeName;

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
                    return nodeName = "";
            }

            return nodeName;
        }


        public string GetObjectId(Microsoft.Office.Interop.OneNote.HierarchyScope scope, string objectName)
        {
            return GetObjectId(null, scope, objectName);
        }

        public string GetObjectId(string parentId,
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

        public List<string> GetChildObjectNames(string parentId,
            Microsoft.Office.Interop.OneNote.HierarchyScope scope)
        {
            string xml;
            onenoteApp.GetHierarchy(parentId, scope, out xml);

            var doc = XDocument.Parse(xml);
            var nodeName = GetNodeNameBasedOnScope(scope);

            var names = doc.Descendants(ns + nodeName)
                .Select(n => n.Attribute("name").Value)
                .ToList();

            return names;
        }

        public List<string> GetChildObjectIds(string parentId,
            Microsoft.Office.Interop.OneNote.HierarchyScope scope)
        {
            string xml;
            onenoteApp.GetHierarchy(parentId, scope, out xml);

            var doc = XDocument.Parse(xml);
            var nodeName = GetNodeNameBasedOnScope(scope);

            var names = doc.Descendants(ns + nodeName)
                .Where(q => q.Attribute("ID") != null)
                .Select(q => q.Attribute("ID").Value)
                .ToList();

            return names;
        }

        public IDictionary<string, string> GetChildObjectMap(
            string parentId,
            Microsoft.Office.Interop.OneNote.HierarchyScope scope)
        {
            string xml;
            onenoteApp.GetHierarchy(parentId, scope, out xml);

            var doc = XDocument.Parse(xml);
            var nodeName = GetNodeNameBasedOnScope(scope);

            var result = new Dictionary<string, string>();
            var pool = doc.Descendants(ns + nodeName).ToList(); 
            foreach (var item in pool)
            {
                if (item.Attribute("ID") != null)
                {
                    result.Add(item.Attribute("ID").Value, item.Attribute("name").Value);
                }

            }

            return result;
        }

        public List<string> GetChildObjectIDs(string parentId)
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


        public string GetChildObjectID(string parentId, string objectID)
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


        public List<string> LogChildObjects(string parentId)
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



        public string GetPageContent(string sectionId, string pageId)
        {
            string xml;
            onenoteApp.GetPageContent(pageId, out xml, Microsoft.Office.Interop.OneNote.PageInfo.piAll);

            var doc = XDocument.Parse(xml);
            return xml;
        }



    }
}

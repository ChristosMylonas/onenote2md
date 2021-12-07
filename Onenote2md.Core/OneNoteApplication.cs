namespace Onenote2md.Core
{
    using Microsoft.Office.Interop.OneNote;
    using Onenote2md.Shared;
    using Onenote2md.Shared.OneNoteObjectModel;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Encapsulates the OneNote COM interface.
    /// This class expose the OneNote API in two ways, one way in OneNote Object Model (ONOM, higher level API)
    /// and the other way in XML (lower level API). In most cases the ONOM is easier to use.
    /// The ONOM is inspired by project https://github.com/shanecastle/onom
    /// </summary>
    public class OneNoteApplication
    {
        private static Lazy<OneNoteApplication> lazyInstance = new Lazy<OneNoteApplication>();

        public static OneNoteApplication Instance
        {
            get { return lazyInstance.Value; }
        }

        private Application InteropApplication { get; } = new Application();

        public Notebooks GetNotebooks()
        {
            // XXX: Figure out how to deal with performance implictions of this.
            // Maybe make this a paramater.
            return XMLDeserialize<Notebooks>(GetHierarchy(String.Empty, HierarchyScope.hsNotebooks));
        }

        /// <summary>
        /// Gets the notebook with the specified name. The name is case-insensitive.
        /// If multiple matches are found, an exact match will be returned or the first one.
        /// </summary>
        /// <param name="notebookName"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Notebook GetNotebook(string notebookName)
        {
            Notebook noteBook = this.GetByName(GetNotebooks().Notebook, notebookName, n => n.name);
            return noteBook;
        }

        public IEnumerable<SectionGroup> GetSectionGroups(Notebook notebook)
        {
            string xml = this.GetHierarchy(notebook.ID, HierarchyScope.hsChildren);
            return XMLDeserialize<Notebook>(xml).SectionGroup;
        }

        public IEnumerable<SectionGroup> GetSectionGroups(SectionGroup parentSectionGroup)
        {
            string xml = this.GetHierarchy(parentSectionGroup.ID, HierarchyScope.hsChildren);
            return XMLDeserialize<SectionGroup>(xml).SectionGroup1;
        }

        public IEnumerable<Section> GetSections(Notebook notebook)
        {
            return GetSections(notebook, true);
        }

        public IEnumerable<Section> GetSections(SectionGroup sectionGroup)
        {
            return GetSections(sectionGroup, true);
        }

        /// <summary>
        /// GetSections is slower if you enumerate pages. If Performance is a concern set <see cref="includePages"/> to false.
        /// </summary>
        /// <param name="notebook"></param>
        /// <param name="includePages"></param>
        /// <returns></returns>

        public IEnumerable<Section> GetSections(Notebook notebook, bool includePages)
        {
            string xml = GetHierarchy(notebook.ID, includePages ? HierarchyScope.hsPages : HierarchyScope.hsSections);
            return XMLDeserialize<Notebook>(xml).Section;
        }

        public IEnumerable<Section> GetSections(SectionGroup sectionGroup, bool includePages)
        {
            string xml = GetHierarchy(sectionGroup.ID, includePages ? HierarchyScope.hsPages : HierarchyScope.hsSections);
            return XMLDeserialize<SectionGroup>(xml).Section;
        }

        /// <summary>
        /// Gets the specified section. The pages within the section are included.
        /// The name is case-insensitive. If multiple matches are found, an exact match will be
        /// returned or the first one.
        /// </summary>
        /// <param name="notebook"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public Section GetSection(Notebook notebook, string sectionName)
        {
            Section section = this.GetByName(this.GetSections(notebook), sectionName, n => n.name);
            return section;
        }

        /// <summary>
        /// Gets the specified section without pages.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public Section GetSection(string sectionName)
        {
            return this.GetObject<Section>(null, sectionName);
        }

        /// <summary>
        /// Returns the hierarchy as an XML string (instead of an out paramater).
        /// </summary>
        /// <param name="startNodeID"></param>
        /// <param name="hsScope"></param>
        /// <returns></returns>
        public string GetHierarchy(string startNodeID, HierarchyScope hsScope)
        {
            this.InteropApplication.GetHierarchy(startNodeID, hsScope, out string output, XMLSchema.xs2013);
            return output;
        }

        public XDocument GetHierarchyXDocument(string startNodeID, HierarchyScope scope)
        {
            string xml = this.GetHierarchy(startNodeID, scope);
            var doc = XDocument.Parse(xml);
            return doc;
        }

        public T GetObject<T>(string parentId, string objectName) where T : class
        {
            HierarchyScope scope;
            Type objectType = typeof(T);
            if (objectType == typeof(Notebook))
            {
                scope = HierarchyScope.hsNotebooks;
            }
            else if (objectType == typeof(Page))
            {
                scope = HierarchyScope.hsPages;
            }
            else if (objectType == typeof(Section))
            {
                scope = HierarchyScope.hsSections;
            }
            else
            {
                throw new Exception("Unknown object type " + typeof(T));
            }

            var doc = this.GetHierarchyXDocument(parentId, scope);
            var nodeName = "";
            switch (scope)
            {
                case (HierarchyScope.hsNotebooks):
                    nodeName = "Notebook";
                    break;

                case (HierarchyScope.hsPages):
                    nodeName = "Page";
                    break;

                case (HierarchyScope.hsSections):
                    nodeName = "Section";
                    break;

                default:
                    return null;
            }

            XElement node = this.GetByName(doc.Descendants(doc.Root.Name.Namespace + nodeName), objectName, n => n.Attribute("name").Value);
            if (node == null)
            {
                return null;
            }

            T model = XMLDeserialize<T>(node.ToString());
            return model;
        }

        public void CloseNotebook(string notebookID)
        {
            this.InteropApplication.CloseNotebook(notebookID);
        }

        /// <summary>
        /// Simple syntax XML string to object, but very inefficient.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T XMLDeserialize<T>(string input)
        {
            return XMLDeserialize<T>(input, null);
        }

        /// <summary>
        /// XML string to object. Specify <see cref="rootElementName"/> when the root tag name is different with the type name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="rootElementName"></param>
        /// <returns></returns>
        public static T XMLDeserialize<T>(string input, string rootElementName = null)
        {
            XmlSerializer xmlSerializer;
            if (string.IsNullOrEmpty(rootElementName))
            {
                xmlSerializer = new XmlSerializer(typeof(T));
            }
            else
            {
                XmlRootAttribute rootAttribute = new XmlRootAttribute()
                {
                    ElementName = rootElementName,
                    Namespace = "http://schemas.microsoft.com/office/onenote/2013/onenote"
                };
                xmlSerializer = new XmlSerializer(typeof(T), rootAttribute);
            }

            return (T)xmlSerializer.Deserialize(new StringReader(input));
        }

        public string GetNodeNameBasedOnScope(HierarchyScope scope)
        {
            string nodeName;

            switch (scope)
            {
                case (HierarchyScope.hsNotebooks):
                    nodeName = "Notebook";
                    break;

                case (HierarchyScope.hsPages):
                    nodeName = "Page";
                    break;

                case (HierarchyScope.hsSections):
                    nodeName = "Section";
                    break;

                case (HierarchyScope.hsChildren):
                    nodeName = "OEChildren";
                    break;


                default:
                    return nodeName = "";
            }

            return nodeName;
        }

        public string GetNodeNameBasedOnObjectType(ObjectType objectType)
        {
            string nodeName;

            switch (objectType)
            {
                case ObjectType.Notebook:
                    nodeName = "Notebook";
                    break;

                case ObjectType.Page:
                    nodeName = "Page";
                    break;

                case ObjectType.SectionGroup:
                    nodeName = "SectionGroup";
                    break;

                case ObjectType.Section:
                    nodeName = "Section";
                    break;

                case ObjectType.Children:
                    nodeName = "OEChildren";
                    break;

                default:
                    return nodeName = "";
            }

            return nodeName;
        }

        public List<string> GetChildObjectNames(string parentId, HierarchyScope scope)
        {
            var doc = this.GetHierarchyXDocument(parentId, scope);
            var nodeName = GetNodeNameBasedOnScope(scope);

            var names = doc.Descendants(doc.Root.Name.Namespace + nodeName)
                .Select(n => n.Attribute("name").Value)
                .ToList();

            return names;
        }

        public List<string> GetChildObjectIds(string parentId,
            HierarchyScope scope,
            ObjectType objectType)
        {
            var doc = this.GetHierarchyXDocument(parentId, scope);
            var nodeName = GetNodeNameBasedOnObjectType(objectType);

            var names = doc.Descendants(doc.Root.Name.Namespace + nodeName)
                .Where(q => q.Attribute("ID") != null)
                .Select(q => q.Attribute("ID").Value)
                .ToList();

            return names;
        }

        public IDictionary<string, string> GetChildObjectMap(
            string parentId,
            HierarchyScope scope,
            ObjectType requestedObject)
        {
            var doc = this.GetHierarchyXDocument(parentId, scope);
            var nodeName = GetNodeNameBasedOnObjectType(requestedObject);

            var result = new Dictionary<string, string>();
            var pool = doc.Descendants(doc.Root.Name.Namespace + nodeName).ToList();
            foreach (var item in pool)
            {
                if (item.Attribute("isRecycleBin")?.Value == "true")
                {
                    continue;
                }

                var id = item.Attribute("ID").Value;
                if (id != null && id != parentId)
                {
                    result.Add(id, item.Attribute("name").Value);
                }
            }

            return result;
        }

        public List<string> GetChildObjectIDs(string parentId)
        {
            var doc = this.GetHierarchyXDocument(parentId, HierarchyScope.hsChildren);
            var nodeName = "OEChildren";

            List<string> result = new List<string>();
            var children = doc.Descendants(doc.Root.Name.Namespace + nodeName).FirstOrDefault();
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
            var doc = this.GetHierarchyXDocument(parentId, HierarchyScope.hsChildren);
            var nodeName = "OEChildren";

            var children = doc.Descendants(doc.Root.Name.Namespace + nodeName).FirstOrDefault();
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
            var doc = this.GetHierarchyXDocument(parentId, HierarchyScope.hsChildren);
            var nodeName = "OEChildren";

            List<string> results = new List<string>();
            var children = doc.Descendants(doc.Root.Name.Namespace + nodeName).FirstOrDefault();
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

        public static void LogChildObject(XElement node, int level, List<string> results)
        {
            if (node != null)
            {
                string name = NormalizeName(node.Name.ToString());
                string content = String.Empty;
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

        public Page GetPage(string pageID)
        {
            string xml = this.GetPageXml(pageID);
            return XMLDeserialize<Page>(xml);
        }

        public string GetPageXml(string pageID)
        {
            this.InteropApplication.GetPageContent(pageID, out string xml, PageInfo.piAll, XMLSchema.xs2013);
            return xml;
        }

        internal string GetBinaryPageContent(string parentId, string id)
        {
            this.InteropApplication.GetBinaryPageContent(parentId, id, out string stringValue);
            return stringValue;
        }

        private static string NormalizeName(string source)
        {
            string ns = "{http://schemas.microsoft.com/office/onenote/2013/onenote}";
            if (String.IsNullOrWhiteSpace(source))
                return source;

            if (source.StartsWith(ns))
                source = source.Replace(ns, "");

            return source;
        }

        /// <summary>
        /// The name is case-insensitive. If multiple matches are found, an exact match will be
        /// returned or the first one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="targetName"></param>
        /// <param name="nameSelector"></param>
        /// <returns></returns>
        private T GetByName<T>(IEnumerable<T> collection, string targetName, Func<T, string> nameSelector) where T : class
        {
            if (collection == null || !collection.Any())
            {
                return default;
            }

            var matches = collection.Where(e => string.Compare(nameSelector(e), targetName, StringComparison.OrdinalIgnoreCase) == 0).ToArray();
            if (matches.Length == 0)
            {
                return default;
            }

            if (matches.Length == 1)
            {
                return matches[0];
            }
            else
            {
                return matches.FirstOrDefault(n => nameSelector(n) == targetName) ?? matches[0];
            }
        }
    }
}

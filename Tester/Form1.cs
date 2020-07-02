using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Tester
{
    public partial class Form1 : Form
    {
        static Microsoft.Office.Interop.OneNote.Application onenoteApp = new Microsoft.Office.Interop.OneNote.Application();
        static XNamespace ns = null;


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetNamespace();
            string notebookId = GetObjectId(null, Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks, "trash");
            string sectionId = GetObjectId(notebookId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections, "Section1");
            string pageId = CreatePage(sectionId, "Test");
        }


        static void GetNamespace()
        {
            string xml;
            onenoteApp.GetHierarchy(null, Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks, out xml);

            var doc = XDocument.Parse(xml);
            ns = doc.Root.Name.Namespace;
        }

        static string GetObjectId(string parentId, Microsoft.Office.Interop.OneNote.HierarchyScope scope, string objectName)
        {
            string xml;
            onenoteApp.GetHierarchy(parentId, scope, out xml);

            var doc = XDocument.Parse(xml);
            var nodeName = "";

            switch (scope)
            {
                case (Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks): nodeName = "Notebook"; break;
                case (Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages): nodeName = "Page"; break;
                case (Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections): nodeName = "Section"; break;
                default:
                    return null;
            }

            var node = doc.Descendants(ns + nodeName).Where(n => n.Attribute("name").Value == objectName).FirstOrDefault();

            return node.Attribute("ID").Value;
        }

        static string CreatePage(string sectionId, string pageName)
        {
            // Create the new page
            string pageId;
            onenoteApp.CreateNewPage(sectionId, out pageId, Microsoft.Office.Interop.OneNote.NewPageStyle.npsBlankPageWithTitle);

            // Get the title and set it to our page name
            string xml;
            onenoteApp.GetPageContent(pageId, out xml, Microsoft.Office.Interop.OneNote.PageInfo.piAll);
            var doc = XDocument.Parse(xml);
            var title = doc.Descendants(ns + "T").First();
            title.Value = pageName;


            // Update the page
            onenoteApp.UpdatePageContent(doc.ToString());

            return pageId;
        }

        static string GetPage(string sectionId, string pageId)
        {
            // Get the title and set it to our page name
            string xml;
            onenoteApp.GetPageContent(pageId, out xml, Microsoft.Office.Interop.OneNote.PageInfo.piAll);
            var doc = XDocument.Parse(xml);
            return xml;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            GetNamespace();
            string notebookId = GetObjectId(null, Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks, "trash");
            string sectionId = GetObjectId(notebookId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections, "Section2");
            string pageId = GetObjectId(sectionId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages, "Page 2");
            string pageXml = GetPage(sectionId, pageId);

        }
    }
}

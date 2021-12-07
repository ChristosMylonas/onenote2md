using Onenote2md.Shared.OneNoteObjectModel;
using Onenote2md.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Onenote2md.Core.Tester
{
    public partial class Form1 : Form
    {
        private readonly OneNoteApplication oneNoteApp = OneNoteApplication.Instance;

        public Form1()
        {
            InitializeComponent();

            this.notebookBox.Text = "Study";
            this.sectionBox.Text = "IT";
            this.pageBox.Text = "Singleton";
            this.objectBox.Text = "{CB2F744C-FE13-4700-B9BF-04BE84F4E953}{31}{B0}";
            this.txtOutDir.Text = @"D:\oss\onenote2md\Onenote2md.Cmd\bin\Debug\net6.0-windows";
        }

        private void BtnGetSections_Click(object sender, EventArgs e)
        {
            Notebook notebook = this.oneNoteApp.GetNotebook(notebookBox.Text);
            if (notebook == null)
                Log("Unknown notebook or not opened");
            else
            {
                var sectionNames = this.oneNoteApp.GetSections(notebook).Select(s => s.name).ToList();                
                Log(sectionNames);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            logList.Items.Clear();
        }

        private void Log(string msg)
        {
            logList.Items.Add(msg.Trim());
        }

        private void Log(List<string> msgs)
        {
            foreach (var item in msgs)
            {
                logList.Items.Add(item.Trim());
            }
        }

        private void BtnGetPages_Click(object sender, EventArgs e)
        {
            Notebook notebook = this.oneNoteApp.GetNotebook(this.notebookBox.Text);
            Section section = this.oneNoteApp.GetSection(notebook, sectionBox.Text);
            if (section == null)
                Log("Unknown section");
            else
            {
                var names = section.Page.Select(p => p.name).ToList();
                Log(names);
            }
        }

        private void BtnCopy_Click(object sender, EventArgs e)
        {
            if (logList.SelectedItem != null)
                Clipboard.SetText(logList.SelectedItem.ToString());
        }

        private void BtnGetChildObjectIDs_Click(object sender, EventArgs e)
        {
            Page page = this.oneNoteApp.GetObject<Page>(null, pageBox.Text);
            if (page == null)
                Log("Unknown page");
            else
            {
                Page pageDetails = this.oneNoteApp.GetPage(page.ID);
                var children = pageDetails.Items.Select(i => i.objectID).ToList();
                Log(children);
            }
        }

        private void BtnGetObject_Click(object sender, EventArgs e)
        {
            Page page = this.oneNoteApp.GetObject<Page>(null, pageBox.Text);
            if (page == null)
                Log("Unknown page");
            else
            {
                var objectResult = this.oneNoteApp.GetPage(page.ID).Items.FirstOrDefault(i => string.Compare(i.objectID, objectBox.Text, StringComparison.InvariantCultureIgnoreCase) == 0)?.ToString();
                if (String.IsNullOrEmpty(objectResult))
                    Log("Unknown object");
                else
                {
                    Log(objectResult);
                }
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Page page = this.oneNoteApp.GetObject<Page>(null, pageBox.Text);
            if (page == null)
                Log("Unknown page");
            else
            {
                var children = this.oneNoteApp.LogChildObjects(page.ID);
                Log(children);
            }
        }

        private void BtnGetPageContent_Click(object sender, EventArgs e)
        {
            Section section = this.oneNoteApp.GetSection(sectionBox.Text);
            if (section == null)
                Log("Unknown section");
            else
            {
                Page page = this.oneNoteApp.GetObject<Page>(null, pageBox.Text);
                if (page == null)
                    Log("Unknown page");
                else
                {
                    var content = this.oneNoteApp.GetPageXml(page.ID);
                    Log(content);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            mdPreviewBox.Clear();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Page page = this.oneNoteApp.GetObject<Page>(null, pageBox.Text);
            if (page == null)
                Log("Unknown page");
            else
            {
                var generator = new MDGenerator(this.oneNoteApp);
                var md = generator.PreviewMD(page);
                mdPreviewBox.AppendText(md.Content);
            }
        }

        private void BtnCloseNotebook_Click(object sender, EventArgs e)
        {
            Notebook notebook = this.oneNoteApp.GetNotebook(notebookBox.Text);
            if (notebook == null)
            {
                Log("Unknown notebook or not opened");
            }

            this.oneNoteApp.CloseNotebook(notebook.ID);
        }

        private void BtnGeneratePageMd_Click(object sender, EventArgs e)
        {
            var outputDirectory = txtOutDir.Text;
            var writer = new MDWriter(outputDirectory, true);
            Page page = this.oneNoteApp.GetObject<Page>(null, pageBox.Text);
            if (page == null)
                Log("Unknown page");
            else
            {
                var generator = new MDGenerator(this.oneNoteApp);
                generator.GeneratePageMD(page, writer);
            }
        }

        private void BtnGenerateSectionMd_Click(object sender, EventArgs e)
        {
            var sectionName = sectionBox.Text;
            var outputDirectory = txtOutDir.Text;
            Section section = this.oneNoteApp.GetSection(sectionName);
            if (section == null)
                Log("Unknown section");
            else
            {
                var writer = new MDWriter(outputDirectory, true);
                INotebookGenerator notebookGenerator = new NotebookParser(this.oneNoteApp, new MDGenerator(this.oneNoteApp));
                notebookGenerator.GenerateSectionMD(section, writer);
            }
        }

        private void BtnGenerateNotebookMd_Click(object sender, EventArgs e)
        {
            var notebookName = notebookBox.Text.Trim();
            var outputDirectory = txtOutDir.Text.Trim();

            INotebookGenerator notebookParser = new NotebookParser(this.oneNoteApp, new MDGenerator(this.oneNoteApp));
            var writer = new MDWriter(outputDirectory, true);
            Notebook notebook = this.oneNoteApp.GetNotebook(notebookName);
            var generator = new MDGeneratorWorker(notebookParser, notebook, writer);
            generator.RunWorkerCompleted += Generator_RunWorkerCompleted;
            generator.RunWorkerAsync();
        }

        private void Generator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Completed");
        }
    }
}

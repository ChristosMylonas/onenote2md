﻿using Onenote2md.Shared;
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
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var notebookParser = new NotebookParser();
            var notebookId = notebookParser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks, notebookBox.Text);

            if (String.IsNullOrEmpty(notebookId))
                Log("Unknown notebook");
            else
            {
                var names = notebookParser.GetChildObjectNames(notebookId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections);
                
                Log(names);
            }

        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            logList.Items.Clear();
        }

        private void Log(string msg)
        {
            logList.Items.Add(msg);
        }

        private void Log(List<string> msgs)
        {
            foreach (var item in msgs)
            {
                logList.Items.Add(item);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var notebookParser = new NotebookParser();
            var sectionId = notebookParser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections, sectionBox.Text);

            if (String.IsNullOrEmpty(sectionId))
                Log("Unknown section");
            else
            {
                var names = notebookParser.GetChildObjectNames(sectionId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages);
                Log(names);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (logList.SelectedItem != null)
                Clipboard.SetText(logList.SelectedItem.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var notebookParser = new NotebookParser();
            var pageId = notebookParser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages, pageBox.Text);

            if (String.IsNullOrEmpty(pageId))
                Log("Unknown page");
            else
            {
                var children = notebookParser.GetChildObjectIDs(pageId);
                Log(children);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            var notebookParser = new NotebookParser();
            var pageId = notebookParser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages, pageBox.Text);

            if (String.IsNullOrEmpty(pageId))
                Log("Unknown page");
            else
            {
                var objectResult = notebookParser.GetChildObjectID(
                    pageId, objectBox.Text);

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
            var notebookParser = new NotebookParser();
            var pageId = notebookParser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages, pageBox.Text);

            if (String.IsNullOrEmpty(pageId))
                Log("Unknown page");
            else
            {
                var children = notebookParser.LogChildObjects(pageId);
                Log(children);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var notebookParser = new NotebookParser();
            var sectionId = notebookParser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections, sectionBox.Text);

            if (String.IsNullOrEmpty(sectionId))
                Log("Unknown section");
            else
            {
                var pageId = notebookParser.GetObjectId(
                    Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages, pageBox.Text);

                if (String.IsNullOrEmpty(pageId))
                    Log("Unknown page");
                else
                {
                    var content = notebookParser.GetPageContent(sectionId, pageId);
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
            var notebookParser = new NotebookParser();
            var pageId = notebookParser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages, pageBox.Text);

            if (String.IsNullOrEmpty(pageId))
                Log("Unknown page");
            else
            {
                var generator = new MDGenerator(notebookParser);
                var md = generator.PreviewMD(pageId);
                mdPreviewBox.AppendText(md.Content);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var notebookParser = new NotebookParser();
            var notebookId = notebookParser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks, notebookBox.Text);

            if (String.IsNullOrEmpty(notebookId))
                Log("Unknown notebook");
            else
            {
                notebookParser.Close(notebookId);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            var outputDirectory = textBox1.Text;
            var writer = new MDWriter(outputDirectory, true);

            var notebookParser = new NotebookParser();
            var pageId = notebookParser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsPages, pageBox.Text);

            if (String.IsNullOrEmpty(pageId))
                Log("Unknown page");
            else
            {
                var generator = new MDGenerator(notebookParser);
                generator.GeneratePageMD(pageId, writer);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            var sectionName = sectionBox.Text;
            var outputDirectory = textBox1.Text;

            var notebookParser = new NotebookParser();

            var sectionId = notebookParser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections, sectionName);

            if (String.IsNullOrEmpty(sectionId))
                Log("Unknown section");
            else
            {
                var writer = new MDWriter(outputDirectory, true);
                var pageIds = notebookParser.GetChildObjectIds(
                    sectionId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren,
                    ObjectType.Page);

                foreach (var pageId in pageIds)
                {
                    var generator = new MDGenerator(notebookParser);

                    generator.GeneratePageMD(pageId, writer);
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            var sectionName = sectionBox.Text;
            var outputDirectory = textBox1.Text;

            var notebookParser = new NotebookParser();
            var writer = new MDWriter(outputDirectory, true);
            var generator = new MDGenerator(notebookParser);
            generator.GenerateSectionMD(sectionName, writer);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            var notebookName = notebookBox.Text;
            var outputDirectory = textBox1.Text;

            var notebookParser = new NotebookParser();
            var writer = new MDWriter(outputDirectory, true);
            var generator = new MDGeneratorWorker(notebookParser, notebookName, writer);
            generator.RunWorkerCompleted += Generator_RunWorkerCompleted;
            generator.RunWorkerAsync();
        }

        private void Generator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Completed");
        }
    }
}

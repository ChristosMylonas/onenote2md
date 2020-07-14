using Onenote2md.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onenote2md.Core
{
    public class MDGeneratorWorker : BackgroundWorker
    {
        MDGenerator generator;

        NotebookParser parser;
        string notebookName;
        IWriter writer;

        public MDGeneratorWorker(NotebookParser parser, string notebookName, IWriter writer)
        {
            this.parser = parser;
            this.notebookName = notebookName;
            this.writer = writer;

            this.DoWork += MDGeneratorWorker_DoWork;
            generator = new MDGenerator(parser);
        }

        private void MDGeneratorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!e.Cancel)
            {
                generator.GenerateNotebookMD(notebookName, writer);
                e.Result = true;
            }

        }
    }
}

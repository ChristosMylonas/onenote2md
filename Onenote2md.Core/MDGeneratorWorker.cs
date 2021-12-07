using Onenote2md.Shared;
using Onenote2md.Shared.OneNoteObjectModel;
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
        private readonly INotebookGenerator notebookGenerator;
        private readonly Notebook notebook;
        private readonly IWriter writer;

        public MDGeneratorWorker(INotebookGenerator notebookGenerator, Notebook notebook, IWriter writer)
        {
            this.notebookGenerator = notebookGenerator;
            this.notebook = notebook;
            this.writer = writer;

            this.DoWork += MDGeneratorWorker_DoWork;
        }

        private void MDGeneratorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!e.Cancel)
            {
                this.notebookGenerator.GenerateNotebookMD(this.notebook, writer);
                e.Result = true;
            }
        }
    }
}

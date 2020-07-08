using Onenote2md.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onenote2md.Core
{
    public class MDWriter
    {
        string outputDirectory;

        public MDWriter(string outputDirectory)
        {
            this.outputDirectory = outputDirectory;

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);
        }


        public void WritePage(MarkdownPage page)
        {
           

        }
    }
}

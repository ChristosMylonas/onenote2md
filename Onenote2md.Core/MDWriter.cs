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
        bool overwrite;

        public MDWriter(string outputDirectory, bool overwrite)
        {
            this.outputDirectory = outputDirectory;
            this.overwrite = overwrite;

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);
        }


        public void WritePage(MarkdownPage page)
        {
            WriteFile(page);
        }

        protected string BuildFullPath(string pageFilename)
        {
            return Path.Combine(outputDirectory, pageFilename);
        }

        protected void WriteFile(MarkdownPage page)
        {
            string fullPath = BuildFullPath(page.Filename);

            if (File.Exists(fullPath))
            {
                if (!overwrite)
                    return;
            }

            using (FileStream fileStream = File.Create(fullPath))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    writer.Write(page.Content);
                }
            }
        }
    }
}

using Onenote2md.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onenote2md.Core
{
    public class MDWriter : IWriter
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

        public string GetOutputDirectory()
        {
            return outputDirectory;
        }

        public void WritePage(MarkdownPage page)
        {
            WriteFile(page);
        }

        public void WritePageImage(string fullPath, byte[] image)
        {
            using (var imageFile = new FileStream(fullPath, FileMode.Create))
            {
                imageFile.Write(image, 0, image.Length);
                imageFile.Flush();
            }
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

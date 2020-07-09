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
        string rootOutputDirectory;
        bool overwrite;
        List<string> subDirectories;

        public MDWriter(string rootOutputDirectory, bool overwrite)
        {
            this.rootOutputDirectory = rootOutputDirectory;
            this.overwrite = overwrite;

            if (!Directory.Exists(rootOutputDirectory))
                Directory.CreateDirectory(rootOutputDirectory);

            subDirectories = new List<string>();
        }

        public void SetSubDirectories(IEnumerable<string> dirs)
        {
            subDirectories.Clear();
            subDirectories.AddRange(dirs);
        }

        public string GetOutputDirectory()
        {
            if (!subDirectories.Any())
                return rootOutputDirectory;
            else
            {
                var paths = new List<string>() { rootOutputDirectory };
                paths.AddRange(subDirectories);
                return Path.Combine(paths.ToArray());
            }
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

        //protected string BuildFullPath(string pageFilename)
        //{
        //    return Path.Combine(outputDirectory, pageFilename);
        //}

        protected void WriteFile(MarkdownPage page)
        {
            //string fullPath = BuildFullPath(page.Filename);
            string fullPath = page.Filename;

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

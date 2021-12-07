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
        bool overwrite;
        Stack<string> subDirectories;

        public string RootOutputDirectory { get; private set; }

        public MDWriter(string rootOutputDirectory, bool overwrite)
        {
            this.RootOutputDirectory = rootOutputDirectory;
            this.overwrite = overwrite;

            if (!Directory.Exists(rootOutputDirectory))
                Directory.CreateDirectory(rootOutputDirectory);

            subDirectories = new Stack<string>();
        }

        public void PushDirectory(string dir)
        {
            string filteredDir = FileHelper.MakeValidFileName(dir);
            subDirectories.Push(filteredDir);
        }

        public void PopDirectory()
        {
            subDirectories.Pop();
        }

        public string GetOutputDirectory()
        {
            if (!subDirectories.Any())
                return RootOutputDirectory;
            else
            {
                var paths = new List<string>() { RootOutputDirectory };
                paths.AddRange(subDirectories.Reverse());
                return Path.Combine(paths.ToArray());                
            }
        }

        public void WritePage(MarkdownPage page)
        {
            WriteFile(page);
        }

        public void WritePageImage(string fullPath, byte[] image)
        {
            this.EnsureDirectoryExists(fullPath);
            using (var imageFile = new FileStream(fullPath, FileMode.Create))
            {
                imageFile.Write(image, 0, image.Length);
                imageFile.Flush();
            }
        }

        protected void WriteFile(MarkdownPage page)
        {
            string fullPath = page.Filename;
            this.EnsureDirectoryExists(fullPath);

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

        private void EnsureDirectoryExists(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            string dir = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(dir);
        }
    }
}

using Onenote2md.Shared;
using Onenote2md.Shared.OneNoteObjectModel;
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
        private readonly MDGeneratorOptions options;

        public MDWriter(MDGeneratorOptions options)
        {
            this.options = options;

            if (!Directory.Exists(this.options.RootOutputDirectory))
            {
                Directory.CreateDirectory(this.options.RootOutputDirectory);
            }
        }

        public void WritePage(MarkdownPage page)
        {
            WriteFile(page);
        }

        public string WriteAttachment(Page page, string preferredFileName, byte[] fileContent)
        {
            string fullPath = this.ResolvePageAttachmentPath(page, preferredFileName);
            this.EnsureDirectoryExists(fullPath);
            using (var imageFile = new FileStream(fullPath, FileMode.Create))
            {
                imageFile.Write(fileContent, 0, fileContent.Length);
                imageFile.Flush();
            }

            string relativePath = this.GetPageRelativePath(page, fullPath);
            return relativePath;
        }

        /// <summary>
        /// Copies the notebook external attachment to the page attachment path.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="originalAttachmentPath"></param>
        /// <returns></returns>
        public string CopyAttachment(Page page, string originalAttachmentPath, string preferredFileName = null)
        {
            if (string.IsNullOrWhiteSpace(preferredFileName))
            {
                preferredFileName = Path.GetFileName(originalAttachmentPath);
            }

            string targetFullPath = this.ResolvePageAttachmentPath(page, preferredFileName);
            if (File.Exists(originalAttachmentPath))
            {
                this.EnsureDirectoryExists(targetFullPath);
                File.Copy(originalAttachmentPath, targetFullPath);
                return this.GetPageRelativePath(page, targetFullPath);
            }
            else
            {
                // Can't find the attachment. Return as is and let the author decide the action.
                return originalAttachmentPath;
            }
        }

        public string GetPageFullPath(Page page)
        {
            var fullPath = Path.Combine(this.options.RootOutputDirectory, page.MarkdownRelativePath);
            return fullPath;
        }

        /// <summary>
        /// Gets a relative path for the fullPath which can be used as a link path inside the page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public string GetPageRelativePath(Page page, string fullPath)
        {
            string pageFullPath = Path.Combine(this.options.RootOutputDirectory, page.MarkdownRelativePath);
            Uri pageUri = new Uri(pageFullPath);
            Uri relativeUri = pageUri.MakeRelativeUri(new Uri(fullPath));
            return relativeUri.ToString();
        }

        /// <summary>
        /// Tries to find a non-existing file name for the attachment.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="preferredFileName"></param>
        /// <returns></returns>
        private string ResolvePageAttachmentPath(Page page, string preferredFileName)
        {
            var outputDirectory = MDWriter.GetAttachmentDirectory(page, this.options);
            var fileCount = 0;
            if (string.Compare(Path.GetExtension(preferredFileName), ".md", StringComparison.OrdinalIgnoreCase) == 0)
            {
                // The attachment name may conflict with the Markdown file. Append another postfix.
                preferredFileName = Path.GetFileNameWithoutExtension(preferredFileName) + "_atch.md";
            }

            var fullPath = Path.Combine(outputDirectory, preferredFileName);
            while (File.Exists(fullPath))
            {
                fileCount++;
                string newFileName = Path.GetFileNameWithoutExtension(preferredFileName) + "_" + fileCount + Path.GetExtension(preferredFileName);
                fullPath = Path.Combine(outputDirectory, newFileName);
            }

            return fullPath;
        }

        protected void WriteFile(MarkdownPage page)
        {
            string fullPath = page.Filename;
            this.EnsureDirectoryExists(fullPath);

            if (File.Exists(fullPath))
            {
                if (!this.options.Overwrite)
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

        /// <summary>
        /// Gets the full path of the attachment directory of the specified page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static string GetAttachmentDirectory(Page page, MDGeneratorOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.AttachmentLocation != AttachmentLocation.Root && page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            // subPath is relative to RootOutputDirectory
            string subPath = string.Empty;
            switch (options.AttachmentLocation)
            {
                case AttachmentLocation.Root:
                    if (!string.IsNullOrWhiteSpace(options.AttachmentSubDir))
                    {
                        subPath = FileHelper.MakeValidFileName(options.AttachmentSubDir);
                    }

                    break;
                case AttachmentLocation.BesidesPage:
                    subPath = Path.GetDirectoryName(page.MarkdownRelativePath);
                    break;
                case AttachmentLocation.SubDir:
                    subPath = Path.GetDirectoryName(page.MarkdownRelativePath);
                    if (!string.IsNullOrWhiteSpace(options.AttachmentSubDir))
                    {
                        subPath = Path.Combine(subPath, FileHelper.MakeValidFileName(options.AttachmentSubDir));
                    }
                    break;
                case AttachmentLocation.PageNameDir:
                    subPath = Path.GetDirectoryName(page.MarkdownRelativePath);
                    string lastDir = Path.GetFileNameWithoutExtension(page.MarkdownFileName) + options.AttachmentSubDir;
                    subPath = Path.Combine(subPath, FileHelper.MakeValidFileName(lastDir));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options.AttachmentLocation));
            }

            string dirFullPath = Path.Combine(options.RootOutputDirectory, subPath.Trim());
            return dirFullPath;
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

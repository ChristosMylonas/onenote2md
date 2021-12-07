using Onenote2md.Shared.OneNoteObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onenote2md.Shared
{
    public class MarkdownGeneratorContext
    {
        private readonly Dictionary<string, QuickStyleDef> quickStyleDefs;
        private readonly Dictionary<string, TagDef> tagDefs;

        public MarkdownGeneratorContext(
            IWriter writer,
            Page page)
        {
            this.Writer = writer;
            this.Page = page;
            if (page?.QuickStyleDef?.Length > 0)
            {
                this.quickStyleDefs = page.QuickStyleDef.ToDictionary(qsd => qsd.index);
            }

            if (page?.TagDef?.Length > 0)
            {
                this.tagDefs = page.TagDef.ToDictionary(td => td.index);
            }
        }

        public QuickStyleDef GetQuickStyleDef(string key)
        {
            if (key == null)
            {
                return null;
            }

            if (quickStyleDefs.ContainsKey(key))
                return quickStyleDefs[key];
            else
                return null;
        }

        public TagDef GetTagDef(string key)
        {
            if (tagDefs.ContainsKey(key))
                return tagDefs[key];
            else
                return null;
        }     

        public string GetPageFullPath()
        {
            var fullPath = Path.Combine(this.Writer.RootOutputDirectory, this.Page.MarkdownRelativePath);
            return fullPath;
        }

        public string GetAttachmentPath(string preferredFileName)
        {
            var mdPath = Path.Combine(this.Writer.RootOutputDirectory, this.Page.MarkdownRelativePath);
            var outputDirectory = Path.GetDirectoryName(mdPath);
            var fullPath = Path.Combine(outputDirectory, preferredFileName);
            var fileCount = 0;
            while (File.Exists(fullPath))
            {
                fileCount++;
                string newFileName = Path.GetFileNameWithoutExtension(preferredFileName) + "_" + fileCount + Path.GetExtension(fullPath);
                fullPath = Path.Combine(outputDirectory, newFileName);
            }
            return fullPath;
        }

        public IWriter Writer { get; private set; }

        public Page Page { get; private set; }

        public string PageTitle { get; set; }

        public int IndentLevel { get; set; }

        public bool InCodeBlock { get; set; }

        public string CodeBlockEnd { get; set; }
    }
}

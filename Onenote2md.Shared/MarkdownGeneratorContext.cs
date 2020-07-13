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
        Dictionary<string, QuickStyleDef> quickStyleDefs;
        Dictionary<string, TagDef> tagDefs;
        TableDef tableDef;
        ImageDef imageDef;
        MarkdownContent lastContent;
        string pageTitle;
        IWriter writer;


        public MarkdownGeneratorContext(
            IWriter writer,
            string parentId,
            Dictionary<string, QuickStyleDef> quickStyleDefs, Dictionary<string, TagDef> tagDefs,
            MarkdownContent content)
        {
            this.writer = writer;
            this.ParentId = parentId;
            this.quickStyleDefs = quickStyleDefs;
            this.tagDefs = tagDefs;
            lastContent = content;
            tableDef = new TableDef();
            imageDef = new ImageDef();
        }

        public MarkdownGeneratorContext(
            IWriter writer,
            string parentId,
            Dictionary<string, QuickStyleDef> quickStyleDefs, Dictionary<string, TagDef> tagDefs)
        {
            this.writer = writer;
            this.ParentId = parentId;
            this.quickStyleDefs = quickStyleDefs;
            this.tagDefs = tagDefs;

            lastContent = null;
            tableDef = new TableDef();
            imageDef = new ImageDef();
        }

        public QuickStyleDef GetQuickStyleDef(string key)
        {
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

        public bool HasContent()
        {
            if (lastContent != null)
                return true;
            else
                return false;
        }

        public bool HasPairedContent()
        {
            if (HasContent())
                return lastContent.IsPair;
            else
                return false;
        }

        public void Reset()
        {
            lastContent = null;
        }

        public void Set(MarkdownContent content)
        {
            lastContent = content;
        }

        public MarkdownContent Get()
        {
            return lastContent;
        }

        public void SetPageTitle(string pageTitle)
        {
            this.pageTitle = pageTitle;
        }

        public string GetPageTitle()
        {
            return pageTitle;
        }

        public string GetPageFilename()
        {
            var pageFilename = $"{pageTitle}.md";

            return FileHelper.MakeValidFileName(pageFilename);
        }

        public void EnsureDirectoryExists(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public string GetPageFullPath()
        {
            var outputDirectory = writer.GetOutputDirectory();
            EnsureDirectoryExists(outputDirectory);

            var fullPath = Path.Combine(outputDirectory, GetPageFilename());
            return fullPath;
        }

        public string GetPageImageFullPath()
        {
            var outputDirectory = writer.GetOutputDirectory();
            EnsureDirectoryExists(outputDirectory);
            var filename = ImageDef.GetFilename(pageTitle);

            return Path.Combine(outputDirectory, FileHelper.MakeValidFileName(filename));
        }


        public string GetPageImageFilename()
        {
            return ImageDef.GetFilename(pageTitle);
        }

        public TableDef TableInfo { get { return tableDef; } }

        public ImageDef ImageDef { get { return imageDef; } }

        public string ParentId { get; private set; }
        public IWriter Writer { get { return writer; } }
    }
}

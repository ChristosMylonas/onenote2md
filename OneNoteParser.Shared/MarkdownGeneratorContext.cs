using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNoteParser.Shared
{
    public class MarkdownGeneratorContext
    {
        Dictionary<string, QuickStyleDef> quickStyleDefs;
        Dictionary<string, TagDef> tagDefs;
        TableDef tableDef;
        ImageDef imageDef;
        MarkdownContent lastContent;

        public MarkdownGeneratorContext(
            string parentId,
            Dictionary<string, QuickStyleDef> quickStyleDefs, Dictionary<string, TagDef> tagDefs,
            MarkdownContent content)
        {
            this.ParentId = parentId;
            this.quickStyleDefs = quickStyleDefs;
            this.tagDefs = tagDefs;
            lastContent = content;
            tableDef = new TableDef();
            imageDef = new ImageDef();
        }

        public MarkdownGeneratorContext(
            string parentId,
            Dictionary<string, QuickStyleDef> quickStyleDefs, Dictionary<string, TagDef> tagDefs)
        {
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

        public TableDef TableInfo { get { return tableDef; } }

        public ImageDef ImageDef { get { return imageDef; } }

        public string ParentId { get; private set; }
    }
}

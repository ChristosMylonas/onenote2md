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
        TableInfo tableInfo;

        MarkdownContent lastContent;
        

        public MarkdownGeneratorContext(
            Dictionary<string, QuickStyleDef> quickStyleDefs, Dictionary<string, TagDef> tagDefs,
            MarkdownContent content)
        {
            this.quickStyleDefs = quickStyleDefs;
            this.tagDefs = tagDefs;
            lastContent = content;
            tableInfo = new TableInfo();
        }

        public MarkdownGeneratorContext(
            Dictionary<string, QuickStyleDef> quickStyleDefs, Dictionary<string, TagDef> tagDefs)
        {
            this.quickStyleDefs = quickStyleDefs;
            this.tagDefs = tagDefs;

            lastContent = null;
            tableInfo = new TableInfo();
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

        public TableInfo TableInfo { get { return tableInfo; } }
    }
}

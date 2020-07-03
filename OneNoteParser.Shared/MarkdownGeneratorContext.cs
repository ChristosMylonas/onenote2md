using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNoteParser.Shared
{
    public class MarkdownGeneratorContext
    {
        MarkdownContent lastContent;

        public MarkdownGeneratorContext(MarkdownContent content)
        {
            lastContent = content;
        }

        public MarkdownGeneratorContext()
        {
            lastContent = null;
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

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNoteParser.Shared
{
   public class MarkdownContent
    {
        public string Content { get; }
        public bool IsPair { get; }

        public bool WillAppendLine()
        {
            if (Content == null)
                return false;
            else
                return Content.Contains("\r\n");
        }

        public MarkdownContent(string content, bool isPair)
        {
            Content = content;
            IsPair = isPair;
        }

        public static MarkdownContent Empty()
        {
            return new MarkdownContent("", false);
        }

        public static MarkdownContent PairedContent(string content)
        {
            return new MarkdownContent(content, true);
        }

        public static MarkdownContent SingleContent(string content)
        {
            return new MarkdownContent(content, false);
        }
    }
}

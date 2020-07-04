using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNoteParser.Shared
{
    public class TagDef
    {
        public string Index { get; set; }
        public string Type { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }

        public MarkdownContent GetMD()
        {
            if (String.IsNullOrEmpty(Name))
                return MarkdownContent.Empty();
            else
            {
                switch (Name)
                {
                    case "To Do":
                        return MarkdownContent.SingleContent(" [ ] ");

                    case "Important":
                        return MarkdownContent.SingleContent(":star: ");

                    case "Question":
                        return MarkdownContent.SingleContent(":question: ");

                    case "Critical":
                        return MarkdownContent.SingleContent(":exclamation: ");
                        
                    

                    default:
                        return MarkdownContent.SingleContent(":red_circle: ");
                }
            }

        }
    }
}

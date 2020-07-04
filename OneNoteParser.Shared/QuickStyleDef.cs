using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneNoteParser.Shared
{
    public class QuickStyleDef
    {
        public string Index { get; set; }
        public string Name { get; set; }

        public MarkdownContent GetMD()
        {
            if (String.IsNullOrEmpty(Name))
                return MarkdownContent.Empty();
            else
            {
                switch (Name)
                {
                    case "PageTitle":
                        return MarkdownContent.SingleContent("# ");

                    case "h1":
                        return MarkdownContent.SingleContent("# ");

                    case "h2":
                        return MarkdownContent.SingleContent("## ");

                    case "h3":
                        return MarkdownContent.SingleContent("### ");

                    case "h4":
                        return MarkdownContent.SingleContent("#### ");

                    case "h5":
                        return MarkdownContent.SingleContent("##### ");

                    case "h6":
                        return MarkdownContent.SingleContent("###### ");

                    case "h7":
                        return MarkdownContent.SingleContent("####### ");

                    case "p":
                        return MarkdownContent.SingleContent("\r\n");

                    case "cite":
                        return MarkdownContent.PairedContent("*");

                    case "code":
                        return MarkdownContent.PairedContent("`");

                    default:
                        return MarkdownContent.Empty();
                }
            }

        }
    }
}

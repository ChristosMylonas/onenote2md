using Onenote2md.Shared.OneNoteObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onenote2md.Shared
{
    public interface IPageGenerator
    {
        MarkdownPage PreviewMD(Page page);

        void GeneratePageMD(Page page, IWriter writer);

        IPageLinkResolver LinkResolver { get; set; }
    }
}

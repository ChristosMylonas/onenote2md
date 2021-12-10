using Onenote2md.Shared.OneNoteObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onenote2md.Shared
{
    public interface IWriter
    {
        void WritePage(MarkdownPage page);

        string WriteAttachment(Page page, string preferredFileName, byte[] fileContent);

        string CopyAttachment(Page page, string originalAttachmentPath, string preferredFileName = null);

        string GetPageFullPath(Page page);
    }
}

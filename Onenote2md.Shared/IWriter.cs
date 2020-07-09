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
        void WritePageImage(string fullPath, byte[] image);
        void PushDirectory(string dir);
        void PopDirectory();
        string GetOutputDirectory();
    }
}

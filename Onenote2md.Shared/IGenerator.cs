using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onenote2md.Shared
{
    public interface IGenerator
    {
        MarkdownPage PreviewMD(string parentId);

        void GeneratePageMD(string parentId, IWriter writer);
        void GenerateSectionMD(string sectionName, IWriter writer);
        void GenerateSectionMD(string sectionId, string sectionName, IWriter writer);
        void GenerateSectionGroupMD(string sectionGroupId, string sectionGroupName, IWriter writer);
        void GenerateNotebookMD(string notebookName, IWriter writer);
    }
}

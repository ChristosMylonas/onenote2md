using Onenote2md.Shared.OneNoteObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onenote2md.Shared
{
    public interface INotebookGenerator
    {
        void GenerateSectionMD(Section section, IWriter writer);

        void GenerateSectionGroupMD(SectionGroup sectionGroup, IWriter writer);

        void GenerateNotebookMD(Notebook notebook, IWriter writer);
    }
}

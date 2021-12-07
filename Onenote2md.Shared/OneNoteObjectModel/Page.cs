namespace Onenote2md.Shared.OneNoteObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public partial class Page
    {
        public string MarkdownFileName { get; set; }

        public string MarkdownRelativePath { get; set; }

        public string SectionName { get; set; }
    }
}

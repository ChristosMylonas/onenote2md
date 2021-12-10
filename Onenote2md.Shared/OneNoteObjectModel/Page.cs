namespace Onenote2md.Shared.OneNoteObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public partial class Page
    {
        /// <summary>
        /// Gets or sets the output Markdown file name (without path).
        /// </summary>
        public string MarkdownFileName { get; set; }

        /// <summary>
        /// Gets or sets the output Markdown file path (with file name).
        /// </summary>
        public string MarkdownRelativePath { get; set; }

        /// <summary>
        /// Gets or sets the OneNote section name which the page belongs to.
        /// Initially this is used to resolve the OneNote internal page links.
        /// </summary>
        public string SectionName { get; set; }
    }
}

namespace Onenote2md.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Configures where to put the attachments of the page.
    /// Let's make an example:
    /// RootOutputDirectory=D:\Markdown
    /// Converted Markdown file=D:\Markdown\NotebookName\SectionName\PageName.md
    /// AttachmentSubDir=assets
    /// </summary>
    public enum AttachmentLocation
    {
        None = 0,

        /// <summary>
        /// Attachment directory path will be: D:\Markdown\assets\
        /// </summary>
        Root = 1,

        /// <summary>
        /// Attachment directory path will be: D:\Markdown\NotebookName\SectionName\
        /// </summary>
        BesidesPage = 2,

        /// <summary>
        /// Attachment directory path will be: D:\Markdown\NotebookName\SectionName\assets\
        /// </summary>
        SubDir = 3,

        /// <summary>
        /// Attachment directory path will be: D:\Markdown\NotebookName\SectionName\PageNameassets\
        /// </summary>
        PageNameDir = 4,
    }
}

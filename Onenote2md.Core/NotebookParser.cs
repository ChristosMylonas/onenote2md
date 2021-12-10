namespace Onenote2md.Core
{
    using Onenote2md.Shared;
    using Onenote2md.Shared.OneNoteObjectModel;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class NotebookParser : INotebookGenerator
    {
        #region Fields
        private readonly OneNoteApplication oneNoteApp;
        private readonly IPageGenerator pageGenerator;
        private Stack<string> subDirectories = new Stack<string>();
        private HashSet<string> targetMarkdownFiles = new HashSet<string>();
        #endregion

        public NotebookParser(OneNoteApplication oneNoteApplication, IPageGenerator pageGenerator)
        {
            this.oneNoteApp = oneNoteApplication;
            this.pageGenerator = pageGenerator;
        }

        public void GenerateSectionMD(Section section, IWriter writer)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            if (section.Page == null || !section.Page.Any())
            {
                return;
            }

            List<Page> pages = this.ResolvePages(section);
            this.GeneratePages(pages, writer);
        }

        public void GenerateSectionGroupMD(SectionGroup sectionGroup, IWriter writer)
        {
            if (sectionGroup == null)
            {
                throw new ArgumentNullException(nameof(sectionGroup));
            }

            List<Page> pages = this.ResolvePages(sectionGroup);
            this.GeneratePages(pages, writer);
        }

        public void GenerateNotebookMD(Notebook notebook, IWriter writer)
        {
            if (notebook == null)
            {
                throw new ArgumentNullException(nameof(notebook));
            }

            this.subDirectories.Push(FileHelper.MakeValidFileName(notebook.nickname));
            List<Page> pages = new List<Page>();
            IEnumerable<SectionGroup> sectionGroups = this.oneNoteApp.GetSectionGroups(notebook);
            if (sectionGroups != null)
            {
                foreach (var item in sectionGroups)
                {
                    if (item.isRecycleBin)
                    {
                        continue;
                    }

                    pages.AddRange(this.ResolvePages(item));
                }
            }

            IEnumerable<Section> sections = this.oneNoteApp.GetSections(notebook);
            if (sections != null)
            {
                foreach (var section in sections)
                {
                    pages.AddRange(this.ResolvePages(section));
                }
            }

            this.subDirectories.Pop();
            this.GeneratePages(pages, writer);
        }

        public void GeneratePages(IEnumerable<Page> pages, IWriter writer)
        {
            PageLinkResolver linkResolver = new PageLinkResolver();
            linkResolver.CachePages(pages);
            foreach (Page page in pages)
            {
                this.GeneratePage(page, writer, linkResolver);
            }
        }

        private void GeneratePage(Page page, IWriter writer, IPageLinkResolver linkResolver)
        {
            if (this.pageGenerator == null)
            {
                throw new ArgumentNullException(nameof(this.pageGenerator));
            }

            if (page == null)
            {
                return;
            }

            // The Page object retrieved from the Section objet doesn't contain page contents.
            // So we need to get the full page details by the following API.
            Page pageDetails = this.oneNoteApp.GetPage(page.ID);
            pageDetails.MarkdownFileName = page.MarkdownFileName;
            pageDetails.MarkdownRelativePath = page.MarkdownRelativePath;
            pageDetails.SectionName = page.SectionName;
            this.pageGenerator.LinkResolver = linkResolver;
            this.pageGenerator.GeneratePageMD(pageDetails, writer);
        }

        /// <summary>
        /// Get the pages in section and resolve the name confliction.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        private List<Page> ResolvePages(Section section)
        {
            List<Page> pages = new List<Page>();
            if (section == null || section.Page == null || !section.Page.Any())
            {
                return pages;
            }

            this.subDirectories.Push(FileHelper.MakeValidFileName(section.name));
            try
            {
                // parentPages stack stores pages with lower PageLevel than the current page.
                // parentPages stack is used to track the page level.
                Stack<Page> parentPages = new Stack<Page>();
                for (int i = 0; i < section.Page.Length; i++)
                {
                    section.Page[i].SectionName = section.name;
                    int.TryParse(section.Page[i].pageLevel, out int currentPageLevel);
                    while (parentPages.Any())
                    {
                        int.TryParse(parentPages.Peek().pageLevel, out int topPageLevel);
                        if (topPageLevel < currentPageLevel)
                        {
                            break;
                        }

                        parentPages.Pop();
                        this.subDirectories.Pop();
                    }

                    // If the current page has subpages, put the current page and its subpages
                    // into the subdir.
                    if (i + 1 < section.Page.Length)
                    {
                        int.TryParse(section.Page[i + 1].pageLevel, out int nextPageLevel);
                        if (nextPageLevel > currentPageLevel)
                        {
                            parentPages.Push(section.Page[i]);
                            this.subDirectories.Push(FileHelper.MakeValidFileName(section.Page[i].name));
                        }
                    }

                    // Resolve the path conflicts.
                    string pageName = FileHelper.MakeValidFileName(section.Page[i].name);
                    if (string.IsNullOrWhiteSpace(pageName))
                    {
                        pageName = "unnamed";
                    }

                    string outputDir = Path.Combine(this.subDirectories.Reverse().ToArray());
                    section.Page[i].MarkdownFileName = pageName + ".md";
                    section.Page[i].MarkdownRelativePath = Path.Combine(outputDir, section.Page[i].MarkdownFileName);
                    int sn = 1;
                    while (this.targetMarkdownFiles.Contains(section.Page[i].MarkdownRelativePath))
                    {
                        section.Page[i].MarkdownFileName = $"{pageName}_{sn++}.md";
                        section.Page[i].MarkdownRelativePath = Path.Combine(outputDir, section.Page[i].MarkdownFileName);
                    }

                    this.targetMarkdownFiles.Add(section.Page[i].MarkdownRelativePath);
                    pages.Add(section.Page[i]);
                }

                while (parentPages.Any())
                {
                    parentPages.Pop();
                    this.subDirectories.Pop();
                }

                return pages;
            }
            finally
            {
                this.subDirectories.Pop();
            }
        }

        /// <summary>
        /// Get the pages in section group and resolve the name confliction.
        /// </summary>
        /// <param name="sectionGroup"></param>
        /// <returns></returns>
        private List<Page> ResolvePages(SectionGroup sectionGroup)
        {
            this.subDirectories.Push(FileHelper.MakeValidFileName(sectionGroup.name));
            try
            {
                List<Page> pages = new List<Page>();
                IEnumerable<SectionGroup> subSectionGroups = this.oneNoteApp.GetSectionGroups(sectionGroup);
                if (subSectionGroups != null)
                {
                    foreach (var subSectionGroup in subSectionGroups)
                    {
                        pages.AddRange(ResolvePages(subSectionGroup));
                    }
                }

                IEnumerable<Section> subSections = this.oneNoteApp.GetSections(sectionGroup);
                if (subSections != null)
                {
                    foreach (Section section in subSections)
                    {
                        pages.AddRange(ResolvePages(section));
                    }
                }

                return pages;
            }
            finally
            {
                this.subDirectories.Pop();
            }
        }
    }
}

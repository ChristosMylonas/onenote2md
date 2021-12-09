namespace Onenote2md.Core
{
    using Onenote2md.Shared;
    using Onenote2md.Shared.OneNoteObjectModel;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    internal class PageLinkResolver : IPageLinkResolver
    {
        private List<Page> pageCache;

        #region IPageLinkResolver interface
        /// <inheritdoc/>
        public string ResolvePageLink(string href, Page relativeTo)
        {
            if (string.IsNullOrWhiteSpace(href))
            {
                return href;
            }

            if (!href.StartsWith("onenote:"))
            {
                return href;
            }

            Regex regex = new Regex(@"onenote:(?<section>[^\.]+\.one)?#(?<page>[^&]+)&", RegexOptions.Compiled);
            Match match = regex.Match(href);
            if (!match.Success)
            {
                return href;
            }

            string sectionName = match.Groups["section"].Value;
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = relativeTo.SectionName;
            }
            else
            {
                // Trim the ".one" extension.
                sectionName = sectionName.Substring(0, sectionName.Length - 4);
                sectionName = HttpUtility.UrlDecode(sectionName);
            }

            string pageName = match.Groups["page"].Value;
            pageName = HttpUtility.UrlDecode(pageName);
            Page targetPage = this.pageCache.FirstOrDefault(p => p.name == pageName && p.SectionName == sectionName);
            if (targetPage == null)
            {
                return href;
            }
            
            Uri targetUri = new Uri(Path.Combine(@"C:\", targetPage.MarkdownRelativePath));
            Uri containerUri = new Uri(Path.Combine(@"C:\", relativeTo.MarkdownRelativePath));
            Uri relativeUri = containerUri.MakeRelativeUri(targetUri);
            return relativeUri.ToString();
        }
        #endregion

        public void CachePages(IEnumerable<Page> pages)
        {
            this.pageCache = pages.ToList();
        }
    }
}

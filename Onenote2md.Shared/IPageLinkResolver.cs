namespace Onenote2md.Shared
{
    public interface IPageLinkResolver
    {
        /// <summary>
        /// Resolves the OneNote internal page link to relative Markdown link.
        /// If the input link is not a OneNote link or the target page is not in the current notebook,
        /// the input link will be returned as is.
        /// An internal link looks like:
        /// onenote:test.one#Format*Test*%3cFormatTest%3e&section-id={874D44D5-B8C8-4DBC-BF3B-CE3998A4769B}&page-id={85A97E23-19E2-4F5D-A9CD-F7C3557664BF}&base-path=https://d.docs.live.net/6a0225773735dfd7/OneNote/Study
        /// </summary>
        /// <param name="href"></param>
        /// <param name="relativeTo"></param>
        /// <returns></returns>
        string ResolvePageLink(string href, string relativeTo);
    }
}

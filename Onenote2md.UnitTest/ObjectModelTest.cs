namespace Onenote2md.UnitTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Onenote2md.Core;
    using Onenote2md.Shared.OneNoteObjectModel;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class ObjectModelTest
    {
        private readonly OneNoteApplication oneNote;

        public ObjectModelTest()
        {
            this.oneNote = OneNoteApplication.Instance;
        }

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void Test1()
        {
            // The following data contains test data specific to my environment.
            Notebook notebook = this.oneNote.GetNotebook("Study");
            Assert.IsNotNull(notebook, "Get Notebook");

            IEnumerable<Section>? sections = this.oneNote.GetSections(notebook);
            Assert.IsNotNull(sections, "Get sections");
            Assert.IsTrue(sections?.Count() > 5);

            Section section = this.oneNote.GetSection(notebook, "IT");
            Assert.IsNotNull(section, "Get single section");
            Assert.IsNotNull(section.Page, "Section pages");
            Assert.IsTrue(section.Page.Count() > 1, "Page count");

            Page pageDetails = this.oneNote.GetPage(section.Page[0].ID);
            Assert.IsNotNull(pageDetails, "Get page details");
            Assert.IsNotNull(pageDetails.Items);
        }
    }
}
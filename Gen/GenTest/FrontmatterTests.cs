using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenLibrary;

namespace GenTest
{
    [TestClass]
    public class FrontMatterTests
    {
        [TestMethod]
        public void CanSetFrontmatterToTemplateType()
        {
            var frontmatter = new Frontmatter("<!-- type:template -->");
            Assert.AreEqual(frontmatter.Type, ContentType.Template);
        }

        [TestMethod]
        public void CanSpecifyATemplate()
        {
            var frontMatter = new Frontmatter("<!-- template:tem -->");
            Assert.IsTrue(frontMatter.UsesTemplate);
        }

        [TestMethod]
        public void TemplateIsSetToFileName()
        {
            var frontMatter = new Frontmatter("<!-- template:tem -->");
            Assert.AreEqual("tem", frontMatter.Template);
        }

        [TestMethod]
        public void FrontmatterCanIncludeATitle()
        {
            var frontMatter = new Frontmatter("<!-- title:\"Test Title\" -->");
            Assert.AreEqual("Test Title", frontMatter.Title);
        }

        [TestMethod]
        public void FrontmatterCanSpecifyDraft()
        {
            var frontmatter = new Frontmatter("<!-- draft: true -->");
            Assert.IsTrue(frontmatter.IsDraft);
        }
    }
}
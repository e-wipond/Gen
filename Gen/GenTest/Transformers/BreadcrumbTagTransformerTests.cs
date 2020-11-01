using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenLibrary;
using GenLibrary.Transformers;

namespace GenTest.Transformers
{
    [TestClass]
    public class BreadcrumbTagTransformerTests
    {
        [TestMethod]
        public void BreadcrumbsForADepthOfOne()
        {
            var transformer = new BreadcrumbTagTransformer();
            var index = new FileNode
            {
                FullName = "index.html",
                Contents = "index",
                Title = "Home"
            };

            IFileNode file = new FileNode
            {
                FullName = "test.html",
                Contents = "{{breadcrumbs}}",
                Parent = index
            };

            file = transformer.Transform(file);

            Assert.AreEqual("<a href=\"./index.html\">Home</a>", file.Contents);
        }

        [TestMethod]
        public void BreadcrumbsForDepthOfTwo()
        {
            var transformer = new BreadcrumbTagTransformer();

            var grandparent = new FileNode
            {
                FullName = "gp.html",
                Contents = "gp",
                Title = "Grandparent",
            };

            var parent = new FileNode
            {
                FullName = "index.html",
                Contents = "index",
                Title = "Parent",
                Parent = grandparent
            };

            IFileNode file = new FileNode
            {
                FullName = "test.html",
                Contents = "{{breadcrumbs}}",
                Parent = parent,
            };

            file = transformer.Transform(file);

            Assert.AreEqual(
                "<a href=\"../gp.html\">Grandparent</a> <span>></span> <a href=\"./index.html\">Parent</a>",
                file.Contents);
        }

        public void BreadcrumbsOnAnIndexPage()
        {
            var transformer = new BreadcrumbTagTransformer();

            var file = new FileNode
            {
                FullName = "index.html",
                Contents = "Home",
                Title = "Home",
            };

            IFileNode child = new FileNode
            {
                FullName = "index.html",
                Contents = "{{breadcrumbs}}",
                Parent = file,
                FileDepth = 1,
            };

            child = transformer.Transform(child);

            Assert.AreEqual("<a href=\"../index.html\">Home</a>", file.Contents);
        }
    }
}
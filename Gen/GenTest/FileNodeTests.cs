using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenLibrary;

namespace GenTest
{
    [TestClass]
    public class FileNodeTests
    {
        [TestMethod]
        public void AFileHasANameAndContents()
        {
            var file = new FileNode
            {
                FullName = "test.txt",
                Contents = "Hello, World."
            };

            Assert.AreEqual("test", file.Name);
            Assert.AreEqual("Hello, World.", file.Contents);
        }

        [TestMethod]
        public void ANameCanHaveADotInIt()
        {
            var file = new FileNode
            {
                FullName = "test.stuff.html",
                Contents = "."
            };

            Assert.AreEqual("test.stuff", file.Name);
        }

        [TestMethod]
        public void AFileHasAnExtension()
        {
            var file = new FileNode
            {
                FullName = "test.txt",
                Contents = "."
            };

            Assert.AreEqual(".txt", file.Extension);
        }

        [TestMethod]
        public void AFileHasAFullName()
        {
            var file = new FileNode
            {
                FullName = "blah.html",
                Contents = "<h1>stuff</h1>"
            };

            Assert.AreEqual("blah.html", file.FullName);
        }

        [TestMethod]
        public void AFileCanHaveFrontMatter()
        {
            var file = new FileNode
            {
                FullName = "front.html",
                Contents = "<!-- type:template -->"
            };

            Assert.AreEqual(file.Frontmatter.Type, ContentType.Template);
        }

        [TestMethod]
        public void AFilesContentsExcludeTheFrontMatter()
        {
            var file = new FileNode
            {
                FullName = "front.html",
                Contents = "<!-- type:template -->\nrest of contents"
            };

            Assert.AreEqual("rest of contents", ((IFileNode)file).Contents);
        }

        [TestMethod]
        public void FileContentsCanBeEmpty()
        {
            var file = new FileNode
            {
                FullName = "empty.txt",
                Contents = string.Empty
            };

            Assert.AreEqual(string.Empty, ((IFileNode)file).Contents);
        }

        [TestMethod]
        public void NewlinesInFrontmatterDoNotBreakRegex()
        {
            var file = new FileNode
            {
                FullName = "_base.html",
                Contents = "<!--\ntype:template\n--><p>test</p>"
            };

            Assert.AreEqual(ContentType.Template, file.Frontmatter.Type);
        }

        [TestMethod]
        public void CanSetParentOfAFile()
        {
            var file = new FileNode();
            var parent = new FileNode();

            file.Parent = parent;

            Assert.IsNotNull(file.Parent);
        }

        [TestMethod]
        public void CanGetDepthOfNode()
        {
            var file = new FileNode();
            var parent = new FileNode();
            var grandparent = new FileNode();

            file.Parent = parent;
            parent.Parent = grandparent;

            Assert.AreEqual(2, file.GetTreeDepth());
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenLibrary;
using GenLibrary.Transformers;

namespace GenTest.Transformers
{
    [TestClass]
    public class RelativeRootTagTransformerTests
    {
        [TestMethod]
        public void CanSetRelativeRootBasedOnDepth()
        {
            var parent = new FileNode();

            IFileNode file = new FileNode
            {
                FullName = "relroot.html",
                Contents = "<link rel=\"stylesheet\" href=\"{{relative-root}}assets/style.css\">",
                Parent = parent
            };

            var tag = new RelativeRootTagTransformer();

            file = tag.Transform(file);

            Assert.AreEqual("<link rel=\"stylesheet\" href=\"./assets/style.css\">", file.Contents);
        }

        [TestMethod]
        public void CanSetRelativeRootAtZeroDepth()
        {
            IFileNode file = new FileNode
            {
                FullName = "relroot.html",
                Contents = "<link rel=\"stylesheet\" href=\"{{relative-root}}assets/style.css\">"
            };

            var tag = new RelativeRootTagTransformer();

            file = tag.Transform(file);

            Assert.AreEqual("<link rel=\"stylesheet\" href=\"./assets/style.css\">", file.Contents);
        }
    }
}
using GenTest.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenTest
{
    [TestClass]
    public class MockGenFSTests
    {
        [TestMethod]
        public void ItCanBeInstantiated()
        {
            var fs = new MockGenFS();
            Assert.IsNotNull(fs);
        }

        [TestMethod]
        public void ExistsIsFalseForANewDirectory()
        {
            var fs = new MockGenFS();
            Assert.IsFalse(fs.DirectoryExists("newDirectory"));
        }

        [TestMethod]
        public void ExistsIsTrueAfterADirectoryIsCreated()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("testDir");
            Assert.IsTrue(fs.DirectoryExists("testDir"));
        }

        [TestMethod]
        public void ExistsIsFalseAfterADirectoryIsRemoved()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("testDir");
            fs.DeleteDirectory("testDir");
            Assert.IsFalse(fs.DirectoryExists("testDir"));
        }

        [TestMethod]
        public void CanCreateAFile()
        {
            var fs = new MockGenFS();
            fs.CreateFile("test.txt");
            Assert.IsTrue(fs.FileExists("test.txt"));
        }

        [TestMethod]
        public void CanCreateAFileInAnExistingDirectory()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("test");
            fs.CreateFile("test/foo.txt");
            Assert.IsTrue(fs.FileExists("test/foo.txt"));
        }

        [TestMethod]
        public void CanCreateAFileInANewDirectory()
        {
            var fs = new MockGenFS();
            fs.CreateFile("new/test.txt");
            Assert.IsTrue(fs.DirectoryExists("new"));
        }

        [TestMethod]
        public void CanFindASubDirectory()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("foo/bar/baz");
            Assert.IsTrue(fs.DirectoryExists("foo/bar"));
        }

        [TestMethod]
        public void CanDeleteASubDirectory()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("foo/bar/baz");
            fs.DeleteDirectory("foo/bar/baz");
            Assert.IsFalse(fs.DirectoryExists("foo/bar/baz"));
        }

        [TestMethod]
        public void AnEmptyDirectoryIsEmpty()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("foo");
            Assert.IsTrue(fs.DirectoryIsEmpty("foo"));
        }

        [TestMethod]
        public void GetFilesReturnsTheFilesInADirectory()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("test");
            fs.CreateFile("test/foo.txt");
            fs.CreateFile("test/bar.txt");

            var files = fs.GetFiles("test");

            Assert.AreEqual(files.Count, 2);
            Assert.IsTrue(files.Contains("foo.txt"));
            Assert.IsTrue(files.Contains("bar.txt"));
        }

        [TestMethod]
        public void GetFilesWorksIfThereIsATrailingSlash()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("test");
            fs.CreateFile("test/foo.txt");
            fs.CreateFile("test/bar.txt");

            var files = fs.GetFiles("test/");

            Assert.AreEqual(files.Count, 2);
            Assert.IsTrue(files.Contains("foo.txt"));
            Assert.IsTrue(files.Contains("bar.txt"));
        }

        [TestMethod]
        public void GetDirectoriesReturnsTheSubDirectories()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("test");
            fs.CreateFile("test/foo.txt");
            fs.CreateDirectory("test/bar");

            var directories = fs.GetDirectories("test");

            Assert.AreEqual(directories.Count, 1);
            Assert.IsTrue(directories.Contains("bar"));
        }

        [TestMethod]
        public void GetFilesDoesNotReturnImplicitFolders()
        {
            var fs = new MockGenFS();
            fs.CreateFile("src/folder/test.txt");
            Assert.AreEqual(fs.GetFiles("src").Count, 0);
        }

        [TestMethod]
        public void GetDirectoriesDoesReturnImplicitFolders()
        {
            var fs = new MockGenFS();
            fs.CreateFile("src/folder/file.txt");
            Assert.AreEqual(fs.GetDirectories("src").Count, 1);

        }

        [TestMethod]
        public void CreateFileCanIncludeFileContents()
        {
            var fs = new MockGenFS();
            fs.CreateFile("test.txt", "hello, world");
            Assert.IsTrue(fs.FileExists("test.txt"));
        }

        [TestMethod]
        public void ReadFileCanReadFileContents()
        {
            var fs = new MockGenFS();
            fs.CreateFile("test.txt", "hello, world");
            Assert.AreEqual("hello, world", fs.ReadFile("test.txt"));
        }
    }
}
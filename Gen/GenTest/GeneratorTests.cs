using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenLibrary;
using GenTest.Mocks;

namespace GenTest
{
    [TestClass]
    public class GeneratorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AnExceptionIsThrownIfTheSrcIsNull()
        {
            var gen = new Generator(new MockGenFS(), null, "dist");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AnExceptionIsThrownIfTheSrcIsTheEmptyString()
        {
            var gen = new Generator(new MockGenFS(), string.Empty, "dist");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AnExceptionIsThrownIfTheSrcDoesNotExist()
        {
            var gen = new Generator(new MockGenFS(), "wrong", "dist");
        }

        [TestMethod]
        public void TheDistDirectoryIsCreatedIfItDoesNotExist()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("test");
            fs.CreateFile("test/index.html");
            var gen = new Generator(fs, "test", "dist");
            Assert.IsTrue(fs.DirectoryExists("dist"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AnExceptionIsThrownIfTheDistIsNotEmpty()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("test");
            fs.CreateDirectory("dist");
            fs.CreateFile("dist/foo.txt");
            var gen = new Generator(fs, "test", "dist");
        }

        [TestMethod]
        public void AnExceptionIsNotThrownIfThePathExists()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("test");
            var gen = new Generator(fs, "test", "dist");
            Assert.IsNotNull(gen);
        }

        [TestMethod]
        public void SourceFilesAreCopiedToTheDistDirectory()
        {
            // Arrange
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html");
            fs.CreateFile("src/one.txt");
            fs.CreateFile("src/two.txt");

            // Act
            var gen = new Generator(fs, "src", "dist");

            // Assert
            Assert.IsTrue(fs.DirectoryExists("dist"));
            Assert.IsTrue(fs.FileExists("dist/index.html"));
            Assert.IsTrue(fs.FileExists("dist/one.txt"));
            Assert.IsTrue(fs.FileExists("dist/two.txt"));
        }

        [TestMethod]
        public void SourceFilesAreCopiedRecursively()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html");
            fs.CreateFile("src/one.txt");
            fs.CreateFile("src/foo/index.html");
            fs.CreateFile("src/foo/two.txt");
            fs.CreateFile("src/foo/bar/index.html");
            fs.CreateFile("src/foo/bar/three.txt");

            var gen = new Generator(fs, "src", "dist");

            Assert.IsTrue(fs.FileExists("dist/index.html"));
            Assert.IsTrue(fs.FileExists("dist/one.txt"));
            Assert.IsTrue(fs.FileExists("dist/foo/index.html"));
            Assert.IsTrue(fs.FileExists("dist/foo/two.txt"));
            Assert.IsTrue(fs.FileExists("dist/foo/bar/index.html"));
            Assert.IsTrue(fs.FileExists("dist/foo/bar/three.txt"));
        }

        [TestMethod]
        public void FilesAreCreatedRelativeToTheirSourceDestination()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("root");
            fs.CreateDirectory("root/src");
            fs.CreateDirectory("root/dist");
            fs.CreateFile("root/src/index.html");

            var gen = new Generator(fs, "root/src", "root/dist");

            Assert.IsTrue(fs.FileExists("root/dist/index.html"));
        }

        [TestMethod]
        public void FileContentsAreCopiedToTheDestination()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html");
            fs.CreateFile("src/test.txt", "hello, world");

            var gen = new Generator(fs, "src", "dist");

            Assert.AreEqual("hello, world", fs.ReadFile("dist/test.txt"));
        }

        [TestMethod]
        public void TemplateFilesAreNotCopiedToTheOutputDirectory()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/test.html", "<!-- type:template -->");

            var gen = new Generator(fs, "src", "dist");

            Assert.IsFalse(fs.FileExists("dist/test.html"));
        }

        [TestMethod]
        public void AContentFileCanUseATemplateFile()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html");
            fs.CreateFile("src/tem.html", "<!-- type:template -->\n<b>{{content}}</b>");
            fs.CreateFile("src/con.html", "<!-- type:content template:tem --><em>Hello, world</em>");

            var gen = new Generator(fs, "src", "dist");

            Assert.AreEqual("<b><em>Hello, world</em></b>", fs.ReadFile("dist/con.html"));
        }

        [TestMethod]
        public void AFileCanUseRelativeRootPaths()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html", "index");
            fs.CreateFile("src/tem.html", "<!-- type:template -->\n<p>{{relative-root}}</p><p>{{content}}</p>");
            fs.CreateFile("src/sub/index.html", "index");
            fs.CreateFile("src/sub/con.html", "<!-- type:content template:tem --><em>Hello, world</em>");

            var gen = new Generator(fs, "src", "dist");

            Assert.AreEqual("<p>../</p><p><em>Hello, world</em></p>", fs.ReadFile("dist/sub/con.html"));
        }

        [TestMethod]
        public void ATemplateCanUseATitle()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html");
            fs.CreateFile("src/tem.html", "<!-- type:template -->\n<h1>{{title}}</h1><p>{{content}}</p>");
            fs.CreateFile("src/con.html", "<!-- template:tem title:\"Content Title\" -->Some content.");

            var gen = new Generator(fs, "src", "dist");

            Assert.AreEqual("<h1>Content Title</h1><p>Some content.</p>", fs.ReadFile("dist/con.html"));
        }

        [TestMethod]
        public void TemplatesCanBeNested()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html");
            fs.CreateFile("src/outerTemplate.html", "<!-- type:template --> <h1>{{content}}</h1>");
            fs.CreateFile("src/innerTemplate.html", "<!-- type:template template:outerTemplate --><em>{{content}}</em>");
            fs.CreateFile("src/content.html", "<!-- template:innerTemplate -->Hello, world.");

            var gen = new Generator(fs, "src", "dist");

            Assert.AreEqual("<h1><em>Hello, world.</em></h1>", fs.ReadFile("dist/content.html"));
        }

        [TestMethod]
        public void TemplateReadAfterContentShouldStillWork()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html");
            fs.CreateFile("src/content.html", "<!-- template:tem --> foobar.");
            fs.CreateFile("src/tem.html", "<!-- type:template --> <p>{{content}}</p>");

            var gen = new Generator(fs, "src", "dist");

            Assert.AreEqual("<p>foobar.</p>", fs.ReadFile("dist/content.html"));
        }

        [TestMethod]
        public void SrcAndDistCanStartWithADot()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html", "index");
            fs.CreateFile("src/about.html", "about");

            var gen = new Generator(fs, "./src", "./dist");

            Assert.IsTrue(fs.FileExists("dist/index.html"));
            Assert.IsTrue(fs.FileExists("dist/about.html"));
        }

        [TestMethod]
        public void MoreComplexNestedTemplatesWork()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html");
            fs.CreateFile("src/_base.html", "<!-- type:template --><h1>_Base</h1>{{content}}");
            fs.CreateFile("src/_page.html", "<!-- type:template template:_base --><h2>_Page</h2>{{content}}");
            fs.CreateFile("src/sicp/week-1.html", "<!-- template:_page --><p>Actual content</p>");

            var gen = new Generator(fs, "src", "dist");

            Assert.AreEqual(
                "<h1>_Base</h1><h2>_Page</h2><p>Actual content</p>", 
                fs.ReadFile("dist/sicp/week-1.html"));
        }

        [TestMethod]
        public void CanCreateABreadcrumbMenu()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html", "<!-- title: Home -->");
            fs.CreateFile("src/one/index.html", "<!-- title: One -->");
            fs.CreateFile("src/one/two/index.html", "<!-- title: Two -->");
            fs.CreateFile("src/one/two/content.html", "{{breadcrumbs}}");

            var gen = new Generator(fs, "src", "dist");

            Assert.AreEqual(
                "<a href=\"../../index.html\">Home</a> <span>></span> <a href=\"../index.html\">One</a> <span>></span> <a href=\"./index.html\">Two</a>", 
                fs.ReadFile("dist/one/two/content.html"));
        }

        [TestMethod]
        public void RelativeRootAtBaseLevel()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html");
            fs.CreateFile("src/about.html", "<!-- template:_base -->about");
            fs.CreateFile("src/_base.html", "<!-- type:template -->{{relative-root}}assets/style.css {{content}}");

            var gen = new Generator(fs, "src", "dist");

            Assert.AreEqual( "./assets/style.css about", fs.ReadFile("dist/about.html"));
        }

        [TestMethod]
        public void BreadcrumbsForIndex()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html", "<!-- title:Home -->Home");
            fs.CreateFile("src/foo/index.html", "<!-- title:Child -->{{breadcrumbs}} Child");

            var gen = new Generator(fs, "src", "dist");

            Assert.AreEqual( "<a href=\"../index.html\">Home</a> Child", fs.ReadFile("dist/foo/index.html"));
        }

        [TestMethod]
        public void ADraftPageIsExcludedFromOutput()
        {
            var fs = new MockGenFS();
            fs.CreateDirectory("src");
            fs.CreateFile("src/index.html");
            fs.CreateFile("src/draft.html", "<!-- draft: true -->");

            var gen = new Generator(fs, "src", "dist");

            Assert.IsFalse(fs.FileExists("dist/draft.html"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using GenLibrary.Transformers;

namespace GenLibrary
{
    public class Generator
    {
        private IGenFS fileIO;

        private string sourceDirectory;

        public string outputDirectory;

        private IList<IFileTransformer> transformers;

        private IList<IFileTransformer> postTransformers;

        private IList<IFileNode> templates = new List<IFileNode>();

        private IList<IFileNode> partials = new List<IFileNode>();

        private IList<IFileNode> pages = new List<IFileNode>();

        private IList<string> fixedFiles = new List<string> { ".jpg", ".pdf", ".png" };

        public Generator(IGenFS fileIO, string sourceDirectory, string outputDirectory)
        {
            this.fileIO = fileIO;

            this.ValidateDirectories(sourceDirectory, outputDirectory);

            this.sourceDirectory = sourceDirectory;
            this.outputDirectory = outputDirectory;

            var stats = new Dictionary<int, Dictionary<int, Statistic>>();

            this.transformers = new List<IFileTransformer>
            {
                new InlineFileTransformer(fileIO, sourceDirectory),
                new RenderPartialTransformer(this.partials),
                new NextPreviousTransformer(),
                new PostsIndexTransformer(this.pages),
                new MarkdownTransformer(),
                new TemplateTransformer(this.templates),
                new KeyTagTransformer(),
                new RelativeRootTagTransformer(),
                new BreadcrumbTagTransformer(),
                new CodeTagTransformer(),
                new SyntaxHighlightTransformer(),
                new ExerciseTagTransformer(stats),
            };

            this.postTransformers = new List<IFileTransformer>
            {
                new ProgressTagTransformer(stats),
                new CleanupTransformer(),
            };

            var root = this.ReadInput(relativePath: string.Empty, fileDepth: 0, parent: null);

            if (root != null)
            {
                this.ProcessTree(root);
                this.WriteOutput(root);
            }
        }

        private IFileNode ReadInput(string relativePath, int fileDepth, IFileNode parent)
        {
            var currentPath = Path.Join(this.sourceDirectory, relativePath);
            var localFiles = this.fileIO.GetFiles(currentPath);
            var subdirectories = this.fileIO.GetDirectories(Path.Join(this.sourceDirectory, relativePath));

            var localNodes = new List<IFileNode>();

            foreach (var filename in localFiles)
            {
                var filePath = Path.Join(currentPath, filename);

                var contents = this.fileIO.ReadFile(filePath);

                var fileNode = new FileNode
                {
                    FullName = filename,
                    Contents = contents,
                    FileDepth = fileDepth,
                    RelativePath = relativePath,
                };

                if (this.fixedFiles.Contains(fileNode.Extension))
                {
                    // ignore from our tree, 
                    // just copy it to the output directly     
                    Directory.CreateDirectory(Path.Join(this.outputDirectory, relativePath));
                    File.Copy(filePath, Path.Join(this.outputDirectory, relativePath, filename));
                }
                else if (fileNode.IsTemplate)
                {
                    this.templates.Add(fileNode);
                }
                else if (fileNode.IsPartial)
                {
                    this.partials.Add(fileNode);
                }
                else
                {
                    if (fileNode.Name == "index")
                    {
                        if (parent != null)
                        {
                            fileNode.Parent = parent;
                            parent.Children.Add(fileNode);
                        }

                        parent = fileNode;
                    }

                    localNodes.Add(fileNode);
                    this.pages.Add(fileNode);
                }
            }

            // Need this separately, in case a file is read before the index.
            foreach (var file in localNodes)
            {
                if (file.Name != "index")
                {
                    file.Parent = parent;
                    parent.Children.Add(file);
                }
            }

            foreach (var directory in subdirectories)
            {
                var path = Path.Join(relativePath, directory);
                this.ReadInput(path, fileDepth + 1, parent);
            }

            return parent;
        }

        private void ProcessTree(IFileNode root)
        {
            for (var i = 0; i < this.templates.Count; i++)
            {
                foreach (var transformer in this.transformers)
                {
                    this.templates[i] = transformer.Transform(this.templates[i]);
                }
            }

            for (var i = 0; i < this.partials.Count; i++)
            {
                foreach (var transformer in this.transformers)
                {
                    this.partials[i] = transformer.Transform(this.partials[i]);
                }
            }

            this.ProcessRecursively(root);
            this.PostProcessRecursively(root);
        }

        private void ProcessRecursively(IFileNode node)
        {
            foreach (var transformer in this.transformers)
            {
                node = transformer.Transform(node);
            }

            for (var i = 0; i < node.Children.Count; i++)
            {
                this.ProcessRecursively(node.Children[i]);
            }
        }

        private void PostProcessRecursively(IFileNode node)
        {
            foreach (var transformer in this.postTransformers)
            {
                node = transformer.Transform(node);
            }

            for (var i = 0; i < node.Children.Count; i++)
            {
                this.PostProcessRecursively(node.Children[i]);
            }
        }

        private void WriteOutput(IFileNode file)
        {
            if (!file.IsDraft)
            {
                var path = Path.Join(this.outputDirectory, file.RelativePath);
                Directory.CreateDirectory(path);
                this.fileIO.CreateFile(Path.Join(path, file.FullName), file.Contents);
            }

            foreach (var child in file.Children)
            {
                this.WriteOutput(child);
            }
        }

        private void ValidateDirectories(string source, string destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(this.sourceDirectory));
            }
            else if (!fileIO.DirectoryExists(source))
            {
                throw new ArgumentException("Source directory must exist", nameof(source));
            }
            else if (!fileIO.DirectoryExists(destination))
            {
                fileIO.CreateDirectory(destination);
            }
            else if (!fileIO.DirectoryIsEmpty(destination))
            {
                throw new ArgumentException("Dist directory must be empty", nameof(destination));
            }
        }
    }
}

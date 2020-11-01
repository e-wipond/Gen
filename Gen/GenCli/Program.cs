using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using GenLibrary;
using McMaster.Extensions.CommandLineUtils;
using System.IO;
using System.Diagnostics;

namespace GenCli
{
    public class Program
    {
        static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Argument(0)]
        [Required]
        public string Source { get; } = string.Empty;

        [Argument(1)]
        [Required]
        public string Destination { get; } = string.Empty;

        [Option(Description = "Overwrite output directory")]
        public bool Force { get; }

        private void OnExecute()
        {
            if (Directory.Exists(this.Source))
            {
                var srcPath = Path.GetFullPath(this.Source);
            }
            else
            {
                Console.WriteLine("Source directory does not exist");
                return;
            }

            if (Directory.Exists(this.Destination)
                && Directory.EnumerateFileSystemEntries(this.Destination).Any())
            {
                if (!this.Force)
                {
                    Console.WriteLine("Output directory is not empty, run again with --force or -f to overwrite");
                    return;
                }
                else
                {
                    Directory.Delete(this.Destination, true);
                }
            }

            var watch = Stopwatch.StartNew();

            var generator = new Generator(new GenFS(), this.Source, this.Destination);

            watch.Stop();

            Console.WriteLine($"Done in {watch.ElapsedMilliseconds} ms.");
        }
    }
}

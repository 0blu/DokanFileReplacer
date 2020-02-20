using System;
using System.IO;
using System.Threading.Tasks;
using DokanNet;
using DokanNet.Logging;

namespace DokanFileReplacer.Example
{
    class Program
    {
        private static int _openCount = 0;
        private const string InterestingFileName = "hello world.txt";
        private static string _mountpoint;

        static void Main(string[] args)
        {
            var proxy = new DokanProxy(Path.GetFullPath("Files/OriginalFolder"), GetReplacedFilename);

            // Mount as removable drive?
            if (true)
            {
                var mountLetter = "R:\\";
                _mountpoint = mountLetter;
                LogMountpointAndRead();
                proxy.Mount(mountLetter, DokanOptions.RemovableDrive, new NullLogger());
            }
            else
            {
                var mountPoint = Path.Combine(Path.GetTempPath(), "DokanFileReplacer");
                _mountpoint = mountPoint;
                Directory.CreateDirectory(mountPoint);

                LogMountpointAndRead();
                proxy.Mount(mountPoint, DokanOptions.OptimizeSingleNameSearch, new NullLogger());

                Directory.Delete(mountPoint);
            }
        }

        private static async void LogMountpointAndRead()
        {
            Console.WriteLine($"Mounted your temporary file system at: '{_mountpoint}'");
            await Task.Delay(500); // Well, we have to wait until the mountpoint is really mounted.

            string path = Path.Combine(_mountpoint, InterestingFileName);
            Console.WriteLine($"\n--- Reading files with File.ReadAllText(\"{path}\") ---");
            for (int i = 1; i <= 10; i++)
            {
                string data = File.ReadAllText(path);
                Console.ForegroundColor = i % 2 == 0 ? ConsoleColor.Red : ConsoleColor.Green;
                Console.WriteLine($"Output {i}: >> '{data}' <<");
                Console.ResetColor();
            }
            Console.WriteLine($"--- END ---\n");
        }

        static string GetReplacedFilename(string requestedFilePath)
        {
            string absPath = Path.Combine(_mountpoint, requestedFilePath);
            if (Directory.Exists(absPath))
            {
                return null; // Skipping directory requests
            }

            Console.WriteLine($"[PROXY] File '{requestedFilePath}' was requested");
            if (Path.GetFileName(requestedFilePath) != InterestingFileName)
            {
                return null; // Just return the original file
            }

            if (_openCount++ % 2 == 0)
            {
                return null; // Only every second request should be intercepted
            }

            Console.WriteLine("[PROXY] Replace file!");
            return  Path.GetFullPath("Files/replacement.txt");
        }
    }
}

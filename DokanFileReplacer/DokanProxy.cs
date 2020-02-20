using System;
using System.IO;
using DokanNet;
using FileAccess = DokanNet.FileAccess;

namespace DokanFileReplacer
{
    public class DokanProxy : Mirror
    {
        /// <summary>
        /// A delegate that is called for each file that is requested in the virtual file system.
        /// If the response is not null, the string is used to open this file instead of the real file.
        /// </summary>
        /// <param name="requestedFilePath">The local path that was requested</param>
        /// <returns>Null if the file should not be temporary replaced, otherwise the new !ABSOLUTE! filepath</returns>
        public delegate string FileReplacementProvider(string requestedFilePath);

        private readonly FileReplacementProvider _replacementProvider;

        public DokanProxy(string originalFilePath, FileReplacementProvider replacementProviderProvider) : base(originalFilePath)
        {
            _replacementProvider = replacementProviderProvider;
        }

        public override NtStatus CreateFile(string fileName, FileAccess access, FileShare share, FileMode mode, FileOptions options,
            FileAttributes attributes, IDokanFileInfo info)
        {
            string replacement = _replacementProvider(fileName);
            string filePath = replacement ?? GetPath(fileName);

            return CreateFileFromAbsolutePath(filePath, access, share, mode, options, attributes, info);
        }
    }
}

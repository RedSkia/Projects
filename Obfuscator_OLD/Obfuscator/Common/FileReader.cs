using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Obfuscator.Common
{
    public sealed class FileReader
    {
        /// <summary>
        /// Files and content to be analyzed Creating the base for obfuscation
        /// </summary>
        private readonly Dictionary<string, string> _inputFiles = new();
        public IReadOnlyDictionary<string, string> InputFiles => this._inputFiles;

        /// <summary>
        /// Files and content to be updated based on the base obfuscation data <see cref="InputFiles"/>
        /// </summary>
        private readonly Dictionary<string, string> _updateFiles = new();
        public IReadOnlyDictionary<string, string> UpdateFiles => this._updateFiles;

        ~FileReader()
        {
            this._inputFiles.Clear();
            this._updateFiles.Clear();
        }

        public string? GetContent(string filePath) => this._inputFiles.TryGetValue(filePath, out string? inputValue) ? inputValue : this._inputFiles.TryGetValue(filePath, out string? updateValue) ? updateValue : null;

        public async Task AddFiles(FileType fileType, params string[] filePaths)
        {
            foreach (string filePath in filePaths) { _ = await AddFile(filePath, fileType); }
        }
        public async Task<bool> AddFile(string filePath, FileType fileType)
        {
            if (!File.Exists(filePath) || !String.Equals(Path.GetExtension(filePath), ".cs", StringComparison.OrdinalIgnoreCase)) { return false; }
            filePath = Path.GetFullPath(filePath);

            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            using (StreamReader reader = new StreamReader(stream))
            {
                var fileContent = await reader.ReadToEndAsync();
                switch (fileType)
                {
                    case FileType.Input: {
                        this._inputFiles[filePath] = fileContent; 
                    } break;
                    case FileType.Update: {
                        if (!this._inputFiles.ContainsKey(filePath)) { this._updateFiles[filePath] = fileContent; } 
                    } break;
                }
            }
            return true;
        }
        public bool RemoveFile(string filePath) => this._inputFiles?.Remove(filePath) ?? this._updateFiles?.Remove(filePath) ?? false;
        public void ClearFiles(FileType fileType = FileType.NULL)
        {
            switch (fileType)
            {
                case FileType.NULL: { this._inputFiles.Clear(); this._updateFiles.Clear(); } break;
                case FileType.Input: { this._inputFiles.Clear(); } break;
                case FileType.Update: { this._updateFiles.Clear(); } break;
            }
        }

        public enum FileType
        {
            NULL,
            Input,
            Update
        }
    }
}
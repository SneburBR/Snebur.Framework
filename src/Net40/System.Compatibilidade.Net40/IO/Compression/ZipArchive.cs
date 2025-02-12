using Ionic.Zip;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace System.IO.Compression
{
    public class ZipArchive: IDisposable
    {
        private readonly Stream _stream;
        private bool _leaveOpen;
        private ZipFile _zipFile;

        public ReadOnlyCollection<ZipArchiveEntry> Entries
            => this._zipFile.Entries
                .Select(x=> new ZipArchiveEntry(x))
                .ToList()
                .AsReadOnly();

        public ZipArchive(Stream stream,
            ZipArchiveMode mode, 
            bool leaveOpen = false)
        {
            this._stream = stream;
            this._leaveOpen = leaveOpen;
            this._zipFile =  mode== ZipArchiveMode.Read 
                ? ZipFile.Read(stream) 
                : new ZipFile();
        }

        public ZipArchiveEntry CreateEntry(string nomeArquivo)
        {
            var entry = new ZipEntry
            {
                FileName = nomeArquivo
            };
            this._zipFile.Entries.Add(entry);
            return new ZipArchiveEntry(entry);
        }

        public void Dispose()
        {
            if (this._leaveOpen)
            {
                return;
            }
            this._stream?.Dispose();
        }
    }

    public class ZipArchiveEntry
    {
        public ZipEntry Entry { get; }
        public string Name 
            =>  this.Entry.FileName;

        public ZipArchiveEntry(ZipEntry entry)
        {
            this.Entry = entry;
        }
          
        public Stream Open()
        {
            return this.Entry.InputStream;
        }
    }
    public enum ZipArchiveMode
    {
        Read,
        Create,
        Update
    }
}

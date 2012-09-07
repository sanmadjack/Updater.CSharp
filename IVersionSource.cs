using System;
using System.IO;
namespace Updater {
    public interface IVersionSource {
        Version ProgramVersion { get; }
        Version GetFileVersion(string name);
        Nullable<DateTime> GetFileDate(string name);
        bool ValidateFile(FileInfo file, Uri url);
    }
}

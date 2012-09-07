using System;

namespace Updater {
    public interface IFileVersion {
        Version Version { get; }
        DateTime Date { get; }
    }
}

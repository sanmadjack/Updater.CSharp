using System;

namespace Updater {
    public class NoVersionInfoException : Exception {
        public IFileVersion File { get; protected set; }
        public NoVersionInfoException(IFileVersion file) {
            File = file;
        }
    }
}

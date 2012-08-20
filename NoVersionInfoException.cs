using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Updater {
    public class NoVersionInfoException: Exception {
        public IFileVersion File { get; protected set; }
        public NoVersionInfoException(IFileVersion file) {
            File = file;
        }
    }
}

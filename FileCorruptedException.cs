using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Updater {
    public class FileCorruptedException: Exception {
        public FileCorruptedException(string file_name, Exception inner) : base(file_name, inner) { }
        public FileCorruptedException(string file_name) : base(file_name) { }

    }
}

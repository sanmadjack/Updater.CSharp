using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Updater {
    public interface IFileVersion {
        Version Version { get; }
        DateTime Date { get; }
    }
}

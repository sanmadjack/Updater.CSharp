using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Updater {
    public class DataUpdates: AUpdates<DataUpdate> {
        private string DestinationFolder;
        public DataUpdates(string destination_folder) {
            DestinationFolder = destination_folder;
        }

        public new void Add(DataUpdate item) {
            item.DownloadFolder = DestinationFolder;
            base.Add(item);
        }

    }
}

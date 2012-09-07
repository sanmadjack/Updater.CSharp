using System.Collections.Generic;

namespace Updater {
    public abstract class AUpdates<T> : List<T> where T : AUpdate {

        public new void Add(T item) {
            int i = checkForFile(item);
            if (i != -1) {
                T existing = this[i];
                int result = existing.CompareTo(item);
                if (result == 0) {
                    existing.addURL(item);
                } else if (result < 0) {
                    this[i] = item;
                }
            } else {
                base.Add(item);
            }
        }

        private int checkForFile(T item) {
            for (int i = 0; i < this.Count; i++) {
                if (this[i].getName() == item.getName())
                    return i;
            }
            return -1;
        }

        public bool UpdateAvailable {
            get {
                return NextUpdate != null;
            }
        }

        public int UpdateCount {
            get {
                int i = 0;
                foreach (T item in this) {
                    if (item.UpdateAvailable)
                        i++;
                }
                return i;
            }
        }
        private AUpdate NextUpdate {
            get {
                if (this.Count == 0)
                    return null;
                foreach (T item in this) {
                    if (item.UpdateAvailable)
                        return item;
                }
                return null;
            }
        }

        public string NextUpdateName {
            get {
                if (NextUpdate == null)
                    return null;
                return NextUpdate.getName();
            }
        }

        public void DownloadNextUpdate() {
            if (UpdateAvailable) {
                NextUpdate.Update();
            }
        }


    }
}

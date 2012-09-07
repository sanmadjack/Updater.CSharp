using System.IO;
using System.Xml;
namespace Updater {
    public class DataUpdate : AUpdate {
        public string DownloadFolder;

        public DataUpdate(XmlElement xml, IVersionSource versions)
            : base(xml, versions) {
        }

        protected override bool performUpdate() {
            //            GameXmlFile file = Games.xml.getFile(this.Name);
            return this.downloadHelper(Path.Combine(DownloadFolder, this.Name));
        }

        public override int CompareTo(AUpdate update) {
            return this.Date.CompareTo(update.Date);
        }

        public override string getName() {
            return Name;
        }

        public override bool UpdateAvailable {
            get {
                if (HasBeenUpdated)
                    return false;


                if (versions.GetFileVersion(this.Name) != null) {
                    return this.Version > versions.GetFileVersion(this.Name);
                }
                if (versions.GetFileDate(this.Name) != null) {
                    return this.Date > versions.GetFileDate(this.Name);
                }
                return true;
                //                throw new NoVersionInfoException(file);
            }
        }

    }
}

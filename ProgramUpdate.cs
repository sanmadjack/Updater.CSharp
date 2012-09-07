using System;
using System.Xml;
namespace Updater {
    public class ProgramUpdate : AUpdate {
        public override bool UpdateAvailable {
            get {
                if (HasBeenUpdated)
                    return false;


                if (OS != "windows")
                    return false;

                if (Version <= versions.ProgramVersion)
                    return false;

                return true;
            }
        }

        public override int CompareTo(AUpdate update) {
            ProgramUpdate prog = update as ProgramUpdate;
            return this.Version.CompareTo(prog.Version);
        }

        public override string getName() {
            return OS;
        }


        public string OS { get; protected set; }

        public ProgramUpdate(XmlElement xml, IVersionSource source)
            : base(xml, source) {
            this.OS = xml.Attributes["os"].Value;

        }

        protected override bool performUpdate() {
            foreach (Uri url in URLs) {
                try {
                    System.Diagnostics.Process.Start(url.ToString());
                    return true;
                } catch (Exception e) {
                    Logger.Logger.log(e);
                    continue;
                }
            }
            return false;
        }

    }
}

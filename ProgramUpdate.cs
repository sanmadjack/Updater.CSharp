﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace Updater {
    class ProgramUpdate: AUpdate {
        public override bool UpdateAvailable {
            get {
                if (OS != "windows")
                    return false;

                if (Version <= Core.ProgramVersion)
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

        public Version Version { get; protected set; }

        public string OS { get; protected set; }

        public ProgramUpdate(XmlElement xml): base(xml) {
            this.Date = DateTime.Parse(xml.Attributes["date"].Value);


            this.Version = new Version(Int32.Parse(xml.Attributes["majorVersion"].Value),Int32.Parse(xml.Attributes["minorVersion"].Value),Int32.Parse(xml.Attributes["revision"].Value));

            this.OS = xml.Attributes["os"].Value;
            //this.Stable = Boolean.Parse(xml.Attributes["stable"].Value);

        }

        public override bool Update() {
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

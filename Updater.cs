using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
namespace Updater {

    public class Updater {
        // The update urls
        private List<string> update_sources;

        public DataUpdates Data;
        public ProgramUpdates Program;
        private IVersionSource VersionSource;

        public Updater(IVersionSource version_source, string data_folder) {
            this.Data = new DataUpdates(data_folder);
            this.Program = new ProgramUpdates();
            this.VersionSource = version_source;
        }

        public UpdateAvailability checkUpdates() {
            if (!checkConnection())
                return UpdateAvailability.None;

            string folder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string updates_file = Path.Combine(folder, "updates.xml");

            if (!File.Exists(updates_file)) {
                throw new FileNotFoundException("updates.xml");
            }

            XmlReaderSettings xml_settings = new XmlReaderSettings();
            xml_settings.ConformanceLevel = ConformanceLevel.Document;
            xml_settings.IgnoreComments = true;
            xml_settings.IgnoreWhitespace = true;
            XmlDocument document = new XmlDocument();

            FileStream stream = new FileStream(updates_file, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            XmlReader reader = XmlReader.Create(stream, xml_settings);
            try {
                document.Load(reader);
            } catch (XmlException e) {
                throw new FileCorruptedException(updates_file, e);
            } finally {
                reader.Close();
                stream.Close();
            }

            XmlElement updates_node = null;
            foreach (XmlNode node in document.ChildNodes) {
                if (node.Name == "updates") {
                    updates_node = node as XmlElement;
                }
            }

            if (updates_node == null) {
                throw new FileCorruptedException(updates_file);
            }

            //this.Add(new UpdateHandler(updates_node,"updates.xml",updates_file));

            update_sources = new List<string>();

            foreach (XmlElement element in updates_node.ChildNodes) {
                if (element.Name != "source")
                    continue;

                update_sources.Add(element.InnerText);
            }


            WebClient Client = new WebClient();



            foreach (string update_source in update_sources) {
                document = new XmlDocument();
                //stream = new FileStream(updates_file, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                Stream updates;
                try {
                    updates = Client.OpenRead(update_source);
                    reader = XmlReader.Create(updates, xml_settings);
                    document.Load(reader);
                    updates.Close();
                } catch (Exception e) {
                    Logger.Logger.log(e);
                    continue;
                } finally {
                    reader.Close();
                }

                XmlElement files_node = null;
                foreach (XmlNode node in document.ChildNodes) {
                    if (node.Name == "files") {
                        files_node = node as XmlElement;
                    }
                }

                if (files_node == null) {
                    Logger.Logger.log(update_source + "seems to be missing the files tag");
                    //                    TranslatingMessageHandler.SendWarning("XMLErrorMissingTag","files", update_source);
                    continue;
                }

                foreach (XmlElement element in files_node.ChildNodes) {
                    try {
                        switch (element.Name) {
                            case "program":
                                Program.Add(new ProgramUpdate(element, VersionSource));
                                break;
                            case "data":
                                Data.Add(new DataUpdate(element, VersionSource));
                                break;
                        }
                    } catch (Exception e) {
                        Logger.Logger.log(e);
                        continue;
                    }
                }
            }

            if (Program.UpdateAvailable && Data.UpdateAvailable) {
                return UpdateAvailability.DataAndProgram;
            } else if (Program.UpdateAvailable) {
                return UpdateAvailability.Program;
            } else if (Data.UpdateAvailable) {
                return UpdateAvailability.Data;
            }
            return UpdateAvailability.None;

        }

        private bool checkConnection() {
            try {
                System.Net.Sockets.TcpClient clnt = new System.Net.Sockets.TcpClient("www.google.com", 80);
                clnt.Close();
                return true;
            } catch {
                return false;
            }
        }

    }
}

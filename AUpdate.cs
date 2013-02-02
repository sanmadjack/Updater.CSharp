using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;

namespace Updater {
    public abstract class AUpdate : IComparable<AUpdate> {
        protected bool HasBeenUpdated = false;

        public Version Version { get; protected set; }


        public string Type { get; protected set; }
        public string Name { get; protected set; }


        public DateTime Date { get; protected set; }
        public List<Uri> URLs { get; protected set; }
        protected IVersionSource versions;
        protected AUpdate(XmlElement xml, IVersionSource versions) {
            URLs = new List<Uri>();
            this.versions = versions;
            this.Date = DateTime.Parse(xml.Attributes["date"].Value);

            Name = xml.Attributes["name"].Value;

            if (xml.HasAttribute("date"))
                this.Date = DateTime.Parse(xml.Attributes["date"].Value);

            if (xml.HasAttribute("majorVersion") && xml.HasAttribute("minorVersion") && xml.HasAttribute("revision"))
                this.Version = new Version(Int32.Parse(xml.Attributes["majorVersion"].Value), Int32.Parse(xml.Attributes["minorVersion"].Value), Int32.Parse(xml.Attributes["revision"].Value));


            addURL(xml);
        }

        public void addURL(XmlElement xml) {

            this.URLs.Add(new Uri(xml.Attributes["url"].Value));





        }
        public void addURL(AUpdate update) {
            this.URLs.AddRange(update.URLs);
        }

        public abstract bool UpdateAvailable { get; }
        public abstract int CompareTo(AUpdate update);
        public abstract string getName();
        public abstract string getPath();


        public bool Update() {
            bool result = performUpdate();
            HasBeenUpdated = true;
            return result;
        }

        protected abstract bool performUpdate();

        protected string downloadFile(Uri url) {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = WebRequestMethods.Http.Get;
            webRequest.KeepAlive = true;
            webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None;

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            Stream remote_file = response.GetResponseStream();
            //remote_file.ReadTimeout = 10000;

            string tmp_name = System.IO.Path.GetTempFileName();

            FileStream local_file = new FileStream(tmp_name, FileMode.Create, FileAccess.Write);

            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = remote_file.Read(buffer, 0, Length);
            while (bytesRead > 0) {
                local_file.Write(buffer, 0, bytesRead);
                bytesRead = remote_file.Read(buffer, 0, Length);
            }

            local_file.Close();
            remote_file.Close();



            return tmp_name;
        }

        protected bool downloadHelper(string target) {
            FileInfo tmp_file = null;
            foreach (Uri url in URLs) {
                try {
                    tmp_file = new FileInfo(downloadFile(url));
                    if (!versions.ValidateFile(tmp_file, url)) {
                        Logger.Logger.log("Error while downloading " + url.ToString());
                        tmp_file.Delete();
                        continue;
                    }



                    if (File.Exists(target))
                        File.Delete(target);

                    tmp_file.MoveTo(target);
                    break;
                } catch (Exception exception) {
                    Logger.Logger.log(exception);
                    if (tmp_file != null && tmp_file.Exists)
                        tmp_file.Delete();
                    return false;
                }
            }

            return true;
        }

    }
}

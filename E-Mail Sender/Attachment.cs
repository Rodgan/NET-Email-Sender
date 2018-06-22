using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Email_Sender
{
    public class Attachment
    {
        public Attachment() { }
        public Attachment(string filePath)
        {
            _FilePath = filePath;
        }
        private string  _FilePath;
        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; }
        }

        public bool FileExists
        {
            get { return File.Exists(FilePath); }
        }
        public string FileName
        {
            get
            {
                if (!FileExists)
                    return null;

                return new FileInfo(FilePath).Name;
            }
        }
        public string MimeType
        {
            get
            {
                if (!FileExists)
                    return null;

                return MimeTypes.GetMimeTypeFromFile(FilePath);
            }
        }
        public string Base64
        {
            get
            {
                if (!FileExists)
                    return null;

                return Convert.ToBase64String(File.ReadAllBytes(FilePath));
            }
        }
    }
}

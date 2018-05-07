using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NET_Email_Sender
{
    public class MimeTypes
    {
        private static Dictionary<string, string> _MimeTypesList;
        public static Dictionary<string, string> MimeTypesList
        {
            get
            {
                if (_MimeTypesList == null)
                {
                    _MimeTypesList = new Dictionary<string, string>();

                    var assembly = Assembly.GetExecutingAssembly();
                    var reader = new StreamReader(assembly.GetManifestResourceStream("NET_Email_Sender.mime_types.txt"));

                    while(reader.Peek() != -1)
                    {
                        var line = reader.ReadLine().Split(';');

                        if (line.Length < 2)
                            continue;

                        var extensions = line[0].Split(',');
                        var mime_type = line[1];

                        for ( var i = 0; i < extensions.Length; i++)
                        {
                            var extension = extensions[i].Trim();

                            if (!_MimeTypesList.ContainsKey(extension))
                                _MimeTypesList.Add(extension.ToLower(), mime_type);
                        }

                    }
                }

                return _MimeTypesList;
            }
        }

        public static string GetMimeTypeFromExtension(string extension, string defaultValue = "text/plain")
        {
            if (!extension.StartsWith("."))
                extension = "." + extension;

            extension = extension.ToLower().Trim();

            var typeFound = MimeTypesList.Where(x => x.Key == extension).FirstOrDefault();

            return typeFound.Value ?? defaultValue;
        }

        public static string GetMimeTypeFromFile(string filePath, string defaultValue = "text/plain")
        {
            if (!File.Exists(filePath))
                throw new Exception($"File {filePath} Not Found");

            FileInfo file = new FileInfo(filePath);

            return GetMimeTypeFromExtension(file.Extension, defaultValue);
        }
    }
}

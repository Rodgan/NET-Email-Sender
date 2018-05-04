using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NET_Email_Sender
{
    public class Email
    {

        // ([<]?[^ !"£\$%&/\(\)='\?\^<>]+@[^ !"£\$%&/\(\)='\?\^<>]+[\.]+[^ !"£\$%&/\(\)='\?\^<>]+[>]?)

        public string Sender
        {
            get { return $"MAIL FROM: {ExtractEmailFromString(_From)}"; }
        }
        public string[] Recipients
        {
            get
            {
                var rcptToContainer = ($"{To};{Cc};{Bcc}").Split(';');

                for ( var i = 0; i < rcptToContainer.Length; i++)
                {
                    var email = ExtractEmailFromString(rcptToContainer[i]);

                    if (email != null)
                        rcptToContainer[i] = $"RCPT TO: {email}";
                }

                return rcptToContainer;
            }
        }

        public static string ExtractEmailFromString(string str)
        {
            var emailPattern = @"([<]?[^ ""!£\$%&/\(\)='\?\^<>]+@[^ ""!£\$%&/\(\)='\?\^<>]+[\.]+[^ ""!£\$%&/\(\)='\?\^<>]+[>]?)";

            var match = Regex.Match(str, emailPattern);

            if (match.Success)
                return match.Value;
            else
                return null;
        }

        private string _From;
        public string From
        {
            get { return _From; }
            set { _From = value; Headers.From = value; }
        }

        private string _To;
        public string To
        {
            get { return _To; }
            set { _To = value; Headers.To = value; }
        }

        private string _Cc;
        public string Cc
        {
            get { return _Cc; }
            set { _Cc = value; Headers.Cc = value; }
        }

        private string _Bcc;
        public string Bcc
        {
            get { return _Bcc; }
            set { _Bcc = value; Headers.Bcc = value; }
        }

        private string _ReplyTo;
        public string ReplyTo
        {
            get { return _ReplyTo; }
            set { _ReplyTo = value; Headers.ReplyTo = value; }
        }

        private string _Subject;
        public string Subject
        {
            get { return _Subject; }
            set { _Subject = value; Headers.Subject = value; }
        }
        public string Message { get; set; }

        public Headers Headers { get; set; } = new Headers();
    }

    public class Headers
    {
        public string[] GetAllHeaders(Headers obj)
        {
            return GetType().
                    GetProperties().
                    Where(x => x.GetValue(obj) != null).
                    Select(x => x.GetValue(obj).ToString()).
                    ToArray();
        }

        private string _From;
        public string From
        {
            get { return (_From != null) ? $"From: {_From}" : null; }
            set { _From = value; }
        }

        private string _To;
        public string To
        {
            get { return (_To != null) ? $"To: {_To}" : null; }
            set { _To = value; }
        }

        private string _Subject;
        public string Subject
        {
            get { return (_Subject != null) ? $"Subject: {_Subject}" : null; }
            set { _Subject = value; }
        }

        private string _ReplyTo;
        public string ReplyTo
        {
            get { return (_ReplyTo != null) ? $"Reply-To: {_ReplyTo}" : null; }
            set { _ReplyTo = value; }
        }

        private string _Cc;
        public string Cc
        {
            get { return (_Cc != null) ? $"CC: {_Cc}" : null; }
            set { _Cc = value; }
        }

        private string _Bcc;
        public string Bcc
        {
            get { return (_Bcc != null) ? $"BCC: {_Bcc}" : null; }
            set { _Bcc = value; }
        }

        private string _MimeVersion;
        public string MimeVersion
        {
            get { return (_MimeVersion != null) ? $"MIME-Version: {_MimeVersion}" : null; }
            set { _MimeVersion = value; }
        }

        private string _ContentTransferEncoding;
        public string ContentTransferEncoding
        {
            get { return (_ContentTransferEncoding != null) ? $"Content-Transfer-Encoding: {_ContentTransferEncoding}" : null; }
            set { _ContentTransferEncoding = value; }
        }

        private string _ContentType;
        public string ContentType
        {
            get { return (_ContentType != null) ? $"Content-Type: {_ContentType}" : null; }
            set { _ContentType = value; }
        }

    }
}

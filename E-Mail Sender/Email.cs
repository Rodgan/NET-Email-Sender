using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Email_Sender
{
    public class Email
    {
        private string _From;
        /// <summary>
        /// Sender
        /// </summary>
        public string From
        {
            get { return $"MAIL FROM: {_From}"; }
            set { _From = value; }
        }

        private string _To;
        /// <summary>
        /// Recipient
        /// </summary>
        public string To
        {
            get { return $"RCPT TO: {_To}"; }
            set { _To = value; }
        }

        /// <summary>
        /// Subject
        /// </summary>
        public string Subject
        {
            get { return Headers.Subject; }
            set { Headers.Subject = value; }
        }

        /// <summary>
        /// Message
        /// </summary>
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

        private string _CC;
        public string CC
        {
            get { return (_CC != null) ? $"CC: {_ReplyTo}" : null; }
            set { _CC = value; }
        }

        private string _BCC;
        public string BCC
        {
            get { return (_BCC != null) ? $"BCC: {_BCC}" : null; }
            set { _BCC = value; }
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

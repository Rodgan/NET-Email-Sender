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
        public enum EmailContentType
        {
            Text,
            HTML
        }
        public Email(string from, string to, string subject, string message, EmailContentType contentType = EmailContentType.Text)
        {
            NewEmail(from, to, null, null, null, subject, message, contentType);
        }
        public Email(string from, string to, string replyTo, string subject, string message, EmailContentType contentType = EmailContentType.Text)
        {
            NewEmail(from, to, null, null, replyTo, subject, message, contentType);
        }
        public Email(string from, string to, string cc, string bcc, string replyTo, string subject, string message, EmailContentType contentType = EmailContentType.Text)
        {
            NewEmail(from, to, cc, bcc, replyTo, subject, message, contentType);
        }

        private void NewEmail(string from, string to, string cc, string bcc, string replyTo, string subject, string message, EmailContentType contentType)
        {
            From = from;
            To = to;
            Cc = cc;
            Bcc = bcc;
            ReplyTo = replyTo;
            Subject = subject;
            Message = message;
            UseHTML = (contentType == EmailContentType.HTML);
        }

        public Email() { }

        public string EmailSender
        {
            get { return $"MAIL FROM: {ExtractEmailFromString(_From)}"; }
        }
        public string[] EmailRecipients
        {
            get
            {
                var rcptToContainer = ($"{To};{Cc};{Bcc}").Split(';');

                for (var i = 0; i < rcptToContainer.Length; i++)
                {
                    var email = ExtractEmailFromString(rcptToContainer[i]);

                    if (email != null)
                        rcptToContainer[i] = $"RCPT TO: {email}";
                }

                return rcptToContainer;
            }
        }
        public List<Attachment> EmailAttachments { get; set; } = new List<Attachment>();
        public void AddAttachment(ICollection<Attachment> attachments)
        {
            var attachmentsToAdd = attachments.Where(x => !EmailAttachments.Contains(x)).ToList();
            EmailAttachments.AddRange(attachmentsToAdd);
        }
        public void AddAttachment(Attachment attachment)
        {
            if (!EmailAttachments.Contains(attachment))
                EmailAttachments.Add(attachment);
        }
        public void RemoveAttachemnt(ICollection<Attachment> attachments)
        {
            var attachmentsToRemove = attachments.Where(x => EmailAttachments.Contains(x)).ToList();

            foreach (var attachment in attachmentsToRemove)
            {
                EmailAttachments.Remove(attachment);
            }
        }
        public void RemoveAttachment(Attachment attachment)
        {
            if (EmailAttachments.Contains(attachment))
                EmailAttachments.Remove(attachment);
        }
        
        public string EmailBody
        {
            get
            {
                return string.Join(Environment.NewLine, EmailBodyBoundaryDeclaration, EmailBodyTextMessage, EmailBodyAttachments, EmailBodyEndingBoundary);
            }
        }

        private string EmailBodyBoundaryDeclaration
        {
            get
            {
                var level_1_boundary = GenerateBoundary(1);
                var boundaryDeclaration = new string[] {
                     $"Content-Type: multipart/mixed; boundary=\"{level_1_boundary}\"",
                     "",
                     "This is a multipart message in MIME format.",
                     ""
                };

                return string.Join(Environment.NewLine, boundaryDeclaration);
            }
        }
        private string EmailBodyTextMessage
        {
            get
            {
                var level_1_boundary = GenerateBoundary(1);
                var contentType = UseHTML ? "text/html" : "text/plain";

                var textMessage = new string[] {
                    $"--{level_1_boundary}",
                    $"Content-Type: {contentType}",
                    "",
                    Message,
                    ""
                };

                return string.Join(Environment.NewLine, textMessage);
            }
        }
        private string EmailBodyAttachments
        {
            get
            {
                if (EmailAttachments.Count == 0)
                    return null;

                var stringCollection = new List<string>();
                var level_1_boundary = GenerateBoundary(1);

                for (var i = 0; i < EmailAttachments.Count; i++)
                {
                    var currentAttachment = EmailAttachments[i];

                    var attachmentString = new string[] {
                        $"--{level_1_boundary}",
                        $"Content-Type: {currentAttachment.MimeType};",
                        $"Content-Disposition: attachment; filename=\"{currentAttachment.FileName}\"",
                        $"Content-Transfer-Encoding: base64",
                        "",
                        currentAttachment.Base64,
                        ""
                    };

                    stringCollection.Add(string.Join(Environment.NewLine, attachmentString));
                }

                return string.Join(Environment.NewLine, stringCollection);
            }
        }
        private string EmailBodyEndingBoundary
        {
            get
            {
                var level_1_boundary = GenerateBoundary(1);
                // Ending boundary ends with "--"
                var endingBoundary = new string[] {
                    $"--{level_1_boundary}--",
                    ""
                };

                return string.Join(Environment.NewLine, endingBoundary);
            }
        }

        private List<string> Boundaries = new List<string>();

        /// <summary>
        /// Generate and/or get boundary string
        /// </summary>
        /// <param name="boundaryLevel">Level of boundary. Starts from 1</param>
        /// <returns></returns>
        private string GenerateBoundary(int boundaryLevel)
        {
            if (Boundaries.Count < boundaryLevel)
            {
                var newBoundary = $"--_Boundary_lv_{boundaryLevel}={Converter.StringToBase64(DateTime.Now.ToString())}";
                Boundaries.Add(newBoundary);
            }

            return Boundaries[boundaryLevel - 1];
        }

        public static string ExtractEmailFromString(string str)
        {
            var emailPattern = @"([<]?[^ ""!£\$%&/\(\)='\?\^<>]+@[^ ""!£\$%&/\(\)='\?\^<>]+[\.]+[^ ""!£\$%&/\(\)='\?\^<>]+[>]?)";

            var match = Regex.Match(str, emailPattern);

            if (match.Success)
            {
                var email = match.Value;

                if (email.StartsWith("<") && !email.EndsWith(">"))
                    email = email + ">";
                else if (!email.StartsWith("<") && email.EndsWith(">"))
                    email = "<" + email;

                return email;
            }
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

        public bool UseHTML { get; set; }
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

        private string _MimeVersion = "1.0";
        public string MimeVersion
        {
            get { return (_MimeVersion != null) ? $"MIME-Version: {_MimeVersion}" : null; }
            set { _MimeVersion = value; }
        }

    }
}

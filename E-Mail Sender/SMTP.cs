using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace NET_Email_Sender
{
    public class SMTP
    {
        public enum AuthType
        {
            Login,
            Plain,
            CramMD5
        }

        #region Stream
        private NetworkStream Stream;
        private SslStream SSLStream;
        private Socket Client;
        private StreamReader Reader;
        private StreamWriter Writer;
        #endregion

        private IPAddress _IPAddress;
        /// <summary>
        /// SMTP IP Address
        /// </summary>
        public IPAddress IPAddress
        {
            get
            {
                return _IPAddress;
            }
        }

        private string _Hostname;
        /// <summary>
        /// SMTP Hostname
        /// </summary>
        public string Hostname
        {
            get
            {
                return _Hostname;
            }
        }

        private int _Port;
        /// <summary>
        /// SMTP Port
        /// </summary>
        public int Port
        {
            get
            {
                return _Port;
            }
        }

        /// <summary>
        /// Time (ms) to wait before stopping reading from Stream
        /// </summary>
        public int ReadTimeout { get; set; } = 2000;

        /// <summary>
        /// If FALSE, login will be skipped
        /// </summary>
        public bool DoLogin { get; set; } = false;
        /// <summary>
        /// If TRUE, use SSL Connection 
        /// </summary>
        public bool UseSSL { get; set; } = false;
        /// <summary>
        /// If TRUE, SSL Certificate will always be marked as secure even if not
        /// </summary>
        public bool SSLCertificateIsSecure { get; set; } = false;
        private SslPolicyErrors SSLError { get; set; } = SslPolicyErrors.None;

        /// <summary>
        /// Set the Type of Authentication
        /// </summary>
        public AuthType Authentication = AuthType.Login;

        /// <summary>
        /// Get or Set Username
        /// </summary>
        public string Username { get; set; } = "";
        /// <summary>
        /// Get or Set Password
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// Get Username in Base64
        /// </summary>
        private string UsernameBase64
        {
            get
            {
                return Converter.StringToBase64(Username);
            }
        }
        /// <summary>
        /// Get Password in Base64
        /// </summary>
        private string PasswordBase64
        {
            get
            {
                return Converter.StringToBase64(Password);
            }
        }

        /// <summary>
        /// Get Logs
        /// </summary>
        public List<string> Log = new List<string>();

        /// <summary>
        /// Set Server Settings
        /// </summary>
        /// <param name="ipAddressOrHostname">IP Address or Hostname</param>
        /// <param name="port">Port</param>
        public void SetServerSettings(string ipAddressOrHostname, int port, bool useSSL = false, bool sslCertificateIsAlwaysSecure = false)
        {
            IPHostEntry DNS = Dns.GetHostEntry(ipAddressOrHostname);

            if (DNS.AddressList.Length == 0)
                return;

            _IPAddress = DNS.AddressList[0];
            _Hostname = DNS.HostName;
            _Port = port;
            UseSSL = useSSL;
            SSLCertificateIsSecure = sslCertificateIsAlwaysSecure;
        }

        /// <summary>
        /// Create a new instance of SMTP
        /// </summary>
        /// <param name="ipAddress">IP Address or Hostname</param>
        /// <param name="port">Port</param>
        public SMTP(string ipAddress, int port, bool useSSL = false, bool sslCertificateIsAlwaysSecure = false)
        {
            ConfigureSMTP(ipAddress, port, useSSL, sslCertificateIsAlwaysSecure);
        }
        /// <summary>
        /// Create a new instance of SMTP
        /// </summary>
        /// <param name="ipAddress">IP Address or Hostname</param>
        /// <param name="port">Port</param>
        public SMTP(string ipAddress, string port, bool useSSL = false, bool sslCertificateIsAlwaysSecure = false)
        {
            ConfigureSMTP(ipAddress, port, useSSL, sslCertificateIsAlwaysSecure);
        }
        /// <summary>
        /// Create a new instance of SMTP
        /// </summary>
        /// <param name="ipAddress">IP Address or Hostname</param>
        /// <param name="port">Port</param>
        /// <param name="username">SMTP Username</param>
        /// <param name="password">SMTP Password</param>
        /// <param name="authType">Authentication Type</param>
        /// <param name="useSSL">If TRUE, use SSL Connection</param>
        /// <param name="sslCertificateIsAlwaysSecure">If TRUE, SSL Certificate will always be marked as secure even if not</param>
        public SMTP(string ipAddress, int port, string username, string password, AuthType authType = AuthType.Login, bool useSSL = false, bool sslCertificateIsAlwaysSecure = false)
        {
            ConfigureSMTP(ipAddress, port, useSSL, sslCertificateIsAlwaysSecure);
            ConfigureLogin(username, password, authType);
        }
        /// <summary>
        /// Create a new instance of SMTP
        /// </summary>
        /// <param name="ipAddress">IP Address or Hostname</param>
        /// <param name="port">Port</param>
        /// <param name="username">SMTP Username</param>
        /// <param name="password">SMTP Password</param>
        /// <param name="authType">Authentication Type</param>
        /// <param name="useSSL">If TRUE, use SSL Connection</param>
        /// <param name="sslCertificateIsAlwaysSecure">If TRUE, SSL Certificate will always be marked as secure even if not</param>
        public SMTP(string ipAddress, string port, string username, string password, AuthType authType = AuthType.Login, bool useSSL = false, bool sslCertificateIsAlwaysSecure = false)
        {
            ConfigureSMTP(ipAddress, port, useSSL, sslCertificateIsAlwaysSecure);
            ConfigureLogin(username, password, authType);
        }
        /// <summary>
        /// Create a new instance of SMTP
        /// </summary>
        public SMTP() { }

        private void ConfigureSMTP(string ipAddress, int port, bool useSSL = false, bool sslCertificateIsAlwaysSecure = false)
        {
            SetServerSettings(ipAddress, port, useSSL, sslCertificateIsAlwaysSecure);
        }
        private void ConfigureSMTP(string ipAddress, string port, bool useSSL = false, bool sslCertificateIsAlwaysSecure = false)
        {
            int smtpPort = 0;

            if (int.TryParse(port, out smtpPort))
                SetServerSettings(ipAddress, smtpPort, useSSL, sslCertificateIsAlwaysSecure);
        }
        private void ConfigureLogin(string username, string password, AuthType authType)
        {
            Username = username;
            Password = password;
            Authentication = authType;
            DoLogin = true;
        }

        /// <summary>
        /// Validate SSL Certificate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="policyErrors"></param>
        /// <returns></returns>
        private bool ValidateSSLCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            SSLError = policyErrors;
            return policyErrors == SslPolicyErrors.None || SSLCertificateIsSecure;
        }

        /// <summary>
        /// Send Email. Returns TRUE if succeeds. Returns FALSE if fails.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool SendEmail(Email email)
        {

            Client = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Client.Connect(Hostname, Port);

            Stream = new NetworkStream(Client);

            if (UseSSL)
            {
                Log.Add($"<Connecting via SSL to {Hostname}:{Port}>");

                SSLStream = new SslStream(Stream, true, new RemoteCertificateValidationCallback(ValidateSSLCertificate), null);

                try
                {
                    SSLStream.AuthenticateAsClient(Hostname);
                }
                catch(Exception excp)
                {
                    
                    Log.Add($"<Invalid SSL Certificate {Hostname}:{Port} ({Enum.GetName(typeof(SslPolicyErrors), SSLError)})>");
                    return false;
                }

                Reader = new StreamReader(SSLStream);
                Writer = new StreamWriter(SSLStream) { AutoFlush = true };
            }
            else
            {
                Log.Add($"<Connecting to {Hostname}:{Port}>");

                Reader = new StreamReader(Stream);
                Writer = new StreamWriter(Stream) { AutoFlush = true };
            }


            // Connection Message
            // Expecting 220
            if (!SMTPResponse.GetResponseCode(ReadLine(), SMTPResponse.ResponseCode.ServiceReady))
            {
                Log.Add($"<Cannot establish connection to {Hostname}:{Port}>");
                return false;
            }

            // Send greetings
            // Expecting 250
            if (!SendAndReadAll("EHLO " + Hostname, SMTPResponse.ResponseCode.OK))
            {
                Log.Add($"<Error after greetings {Hostname}:{Port}>");
                return false;
            }


            // Do Login if required
            if (DoLogin)
            {
                switch (Authentication)
                {
                    case AuthType.Login:
                        if (!AuthLogin())
                        {
                            Log.Add($"<Authentication Error {Hostname}:{Port}>");
                            return false;
                        }
                        break;
                }
            }

            // Send MAIL FROM
            // Send RCPT TO
            if (!SendSenderAndRecipientAddresses(email))
            {
                Log.Add($"<Error after MAIL FROM & RCPT TO {Hostname}:{Port}>");
                return false;
            }


            // Send DATA
            // Expecting 354
            if (!SendAndReadLine("DATA", SMTPResponse.ResponseCode.StartMailInput))
            {
                Log.Add($"<Error after DATA {Hostname}:{Port}>");
                return false;
            }

            // Send Headers
            // Expecting nothing
            var headers = email.Headers.GetAllHeaders(email.Headers);

            foreach (var header in headers)
                Send(header);


            // Send Email body
            // Expecting nothing
            Send(email.EmailBody);

            // Send the end of message
            // Expecting 250
            var emailSent = SendEndOfMessage();

            if (emailSent)
                Log.Add("<E-Mail sent>");
            else
                Log.Add("<E-Mail not sent>");

            // Send QUIT
            // Expecting nothing
            Send("QUIT");

            return emailSent;
        }

  
        /// <summary>
        /// Read to end
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private string Read()
        {
            string line;
            string strToReturn = "";

            while ((line = ReadLine()) != null)
            {
                strToReturn += line + "\r\n";
            }

            return strToReturn;
        }
        /// <summary>
        /// Read Next Line
        /// </summary>
        /// <returns></returns>
        private string ReadLine()
        {
            try
            {
                Reader.BaseStream.ReadTimeout = ReadTimeout;
                var line = "[S]: " + SMTPResponse.FixMessage(Reader.ReadLine());
                Log.Add(line);

                return line;
            }
            catch (Exception excp)
            {
                return null;
            }
        }

        /// <summary>
        /// Send Data and read next line. Returns TRUE if expected code is equals to the actual response code.
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <param name="expectedResponse">Expected Response Code</param>
        /// <returns></returns>
        private bool SendAndReadLine(string data, SMTPResponse.ResponseCode expectedResponse)
        {
            Send(data);
            return SMTPResponse.GetResponseCode(ReadLine(), expectedResponse);
        }
        /// <summary>
        /// Send Data and read to end. Returns TRUE if expected code is equals to the actual response code.
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <param name="expectedResponse">Expected Response Code</param>
        /// <returns></returns>
        private bool SendAndReadAll(string data, SMTPResponse.ResponseCode expectedResponse)
        {
            Send(data);
            return SMTPResponse.GetResponseCode(Read(), expectedResponse);
        }
        /// <summary>
        /// Send Data without reading. Returns nothing.
        /// </summary>
        /// <param name="data">Data to send</param>
        private void Send(string data)
        {
            Log.Add("[C]: " + data);
            Writer.WriteLine(data);
        }

        private bool AuthLogin()
        {

            // Send AUTH LOGIN
            // Expecting 334
            if (!SendAndReadLine("AUTH LOGIN", SMTPResponse.ResponseCode.AskForCredentials))
                return false;

            // Send Username in Base64
            // Expecting 334
            if (!SendAndReadLine(UsernameBase64, SMTPResponse.ResponseCode.AskForCredentials))
                return false;

            // Send Password in Base64
            // Expecting 235
            return SendAndReadLine(PasswordBase64, SMTPResponse.ResponseCode.AuthenticationSuccessful);

        }

        private bool SendSenderAndRecipientAddresses(Email email)
        {
            // Send MAIL FROM: ...
            // Expecting 250
            if (!SendAndReadLine(email.EmailSender, SMTPResponse.ResponseCode.OK))
                return false;

            bool allRecipientsAccepted = false;

            // Send RCPT TO:
            // Expecting 250
            foreach (var rcpt in email.EmailRecipients)
            {
                if (string.IsNullOrEmpty(rcpt))
                    continue;

                if (!SendAndReadLine(rcpt, SMTPResponse.ResponseCode.OK))
                    return false;
                else
                    allRecipientsAccepted = true;
            }
            
            return allRecipientsAccepted;
        }

        private bool SendEndOfMessage()
        {
            Send("");
            Send("");
            return SendAndReadLine(".", SMTPResponse.ResponseCode.OK);
        }
    }
}


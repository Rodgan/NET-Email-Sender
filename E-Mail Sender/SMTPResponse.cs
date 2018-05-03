using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NET_Email_Sender
{
    class SMTPResponse
    {
        public enum ResponseCode
        {
            CodeNotRecognized,
            Success = 200,
            SystemStatus = 211,
            Help = 214,
            ServiceReady = 220,
            ServiceClosing = 221,
            AuthenticationSuccessful = 235,
            OK = 250,
            UserNotLocal_1 = 251,
            CannotVerifyUser = 252,
            AskForCredentials = 334,
            StartMailInput = 354,
            ServiceNotAvailable = 421,
            MailboxUnavailable_1 = 450,
            LocalErrorInProcessing = 451,
            InsufficientSystemStorage = 452,
            CommandUnrecognized = 500,
            SyntaxErrorParametersOrArguments = 501,
            CommandNotImplemented = 502,
            BadSequenceOfCommands = 503,
            CommandParameterNotImplemented = 504,
            DomainDoesNotAcceptMail = 521,
            AccessDenied = 530,
            MailboxUnavailable_2 = 550,
            UserNotLocal_2 = 551,
            ExceededStorageAllocation = 552,
            MailboxNameNotAllowed = 553,
            TransactionFailed = 554,
            SyntaxError = 555
        }

        public static string FixMessage(string message)
        {
            var match = Regex.Match(message,  @"([\d]{3}[ |\-]){1}(.)+$");

            if (match.Success)
                return match.Value;
            else
                return null;
        }
        public static bool GetResponseCode(string message, ResponseCode expectedResponse)
        {
            if (message == null || message.Length < 3)
                return false;

            message = FixMessage(message);

            var code = message.Substring(0,3);

            ResponseCode response = ResponseCode.CodeNotRecognized;

            Enum.TryParse(code, out response);

            return response == expectedResponse;
        }
    }
}

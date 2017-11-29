/*
Data_Capture_and_Extraction_with_C_Sharp_Succinctly.
Eduardo Freitas - 2016 - http://edfreitas.me
*/

#region "using"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MailKit.Net.Pop3;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MimeKit;
using System.IO;
using MailKit.Net.Smtp;
#endregion

#region "namespace"
namespace EmailProcessing
{
    #region "EmailParser"
    public class EmailParser : IDisposable
    {
        #region "consts"
        private const string cStrInvoice = "Invoice";
        private const string cStrMarketing = "Marketing";
        private const string cStrSupport = "Support";

        private const string cStrDftMsg = @"Hi,

        We've received your message but we are unable to classify it properly.

        -- Cheers.";

        private const string cStrMktMsg = @"Hi,

        We've received your message and we've relayed it to the Marketing department.

        -- Cheers.";

        private const string cStrAptMsg = @"Hi,

        We've received your message and we've relayed it to the Account Payable department.

        -- Cheers.";

        private const string cStrSupportMsg = @"Hi,

        We've received your message and we've relayed it to the Support department.

        -- Cheers.";
        #endregion

        #region "properties"
        public string User { get; set; }
        public string Pwd { get; set; }
        public string MailServer { get; set; }
        public int Port { get; set; }
        public Pop3Client Pop3 { get; set; }
        public ImapClient Imap { get; set; }
        #endregion

        #region "private"
        /// <summary>
        /// When set to true indicates that the instance of the class has been disposed.
        /// </summary>
        protected bool disposed;
        #endregion

        #region "constructors-destructors"
        public EmailParser()
        {
            try
            {
                User = String.Empty;
                Pwd = String.Empty;
                MailServer = String.Empty;
                Port = 0;
                Pop3 = null;
                Imap = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public EmailParser(string user, string pwd, string mailserver, int port)
        {
            try
            {
                User = user;
                Pwd = pwd;
                MailServer = mailserver;
                Port = port;
                Pop3 = null;
                Imap = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        ~EmailParser()
        {
            // Our finalizer should call our Dispose(bool) method with false.
            this.Dispose(false);
        }
        #endregion

        #region "public"
        public void DisplayPop3Subjects()
        {
            try
            {
                for (int i = 0; i < Pop3?.Count; i++)
                {
                    MimeMessage message = Pop3.GetMessage(i);
                    Console.WriteLine("Subject: {0}", message?.Subject);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void DisplayImapSubjects()
        {
            try
            {
                var folder = Imap?.Inbox.Open(FolderAccess.ReadOnly);

                for (int i = 0; i < Imap?.Inbox.Count; i++)
                {
                    MimeMessage message = Imap.Inbox.GetMessage(i);
                    Console.WriteLine("Subject: {0}", message?.Subject);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void DisplayPop3HeaderInfo()
        {
            try
            {
                for (int i = 0; i < Pop3?.Count; i++)
                {
                    MimeMessage message = Pop3.GetMessage(i);
                    Console.WriteLine("Subject: {0}", message?.Subject);

                    foreach (Header h in message?.Headers)
                    {
                        Console.WriteLine("Header Field: {0} = {1}", h.Field, h.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void DisplayImapHeaderInfo()
        {
            try
            {
                var folder = Imap?.Inbox.Open(FolderAccess.ReadOnly);

                for (int i = 0; i < Imap.Inbox?.Count; i++)
                {
                    MimeMessage message = Imap.Inbox.GetMessage(i);
                    Console.WriteLine("Subject: {0}", message?.Subject);

                    foreach (Header h in message?.Headers)
                    {
                        Console.WriteLine("Header Field: {0} = {1}", h.Field, h.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected string[] GetPop3EmailWords(ref MimeMessage m)
        {
            List<string> w = new List<string>();

            string b = String.Empty, s = String.Empty, c = String.Empty;

            try
            {
                b = m.GetTextBody(MimeKit.Text.TextFormat.Text);
                s = m.Subject;

                if (m.Attachments != null)
                {
                    foreach (MimeEntity att in m.Attachments)
                    {
                        if (att.IsAttachment)
                        {
                            if (att.ContentType.MediaType.Contains("text"))
                            {
                                c = ((MimeKit.TextPart)att).Text;
                            }
                        }
                    }

                    w = CleanMergeEmailWords(w, b, s, c);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return w.ToArray();
        }

        private static List<string> CleanMergeEmailWords(List<string> w, string b, string s, string c)
        {
            try {
                if (b != String.Empty || s != String.Empty || c != String.Empty)
                {
                    List<string> bl = new List<string>();
                    List<string> sl = new List<string>();
                    List<string> cl = new List<string>();

                    if (b != String.Empty)
                        bl = b.Split(new string[] { " ", "\r", "\n" },
                        StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (s != String.Empty)
                        sl = s.Split(new string[] { " ", "\r", "\n" },
                        StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (c != String.Empty)
                        cl = c.Split(new string[] { " ", "\r", "\n" },
                        StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (bl.Count > 0 || sl.Count > 0 || cl.Count > 0)
                    {
                        bl = bl.Union(sl).ToList();
                        w = bl.Union(cl).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return w;
        }

        public void SendSmtpResponse(string smtp, string user, string pwd, int port, string toAddress, string toName, string msgTxt)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(
                    "Your Name (Automated Email Bot)", 
                    "you@your-domain.com"));

                message.To.Add(new MailboxAddress(toName, toAddress));
                message.Subject = "Thanks for reaching out";

                message.Body = new TextPart("plain")
                {
                    Text = msgTxt
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(smtp, port, true);
                    client.Authenticate(user, pwd);

                    client.Send(message);

                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected int DetermineResponseType(string[] w)
        {
            int res = -1;

            try
            {
                foreach (string ww in w)
                {
                    if (ww.ToUpper().Contains(cStrInvoice.ToUpper()))
                    {
                        res = 1;
                        break;
                    }
                    else if (ww.ToUpper().Contains(cStrMarketing.ToUpper()))
                    {
                        res = 0;
                        break;
                    }
                    else if (ww.ToUpper().Contains(cStrSupport.ToUpper()))
                    {
                        res = 2;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return res;
        }

        protected void SendResponses(string[] w, string smtp, string user, string pwd, int port, string toAddress, string toName)
        {
            try
            {
                switch (DetermineResponseType(w))
                {
                    case 0: // Marketing
                        SendSmtpResponse(smtp, user, pwd, port, toAddress, toName, cStrMktMsg);
                        break;

                    case 1: // Accounts Payable
                        SendSmtpResponse(smtp, user, pwd, port, toAddress, toName, cStrAptMsg);
                        break;

                    case 2: // Support
                        SendSmtpResponse(smtp, user, pwd, port, toAddress, toName, cStrSupportMsg);
                        break;

                    default: // Anything else
                        SendSmtpResponse(smtp, user, pwd, port, toAddress, toName, cStrDftMsg);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void AutomatedSmtpResponses(string smtp, string user, string pwd, int port, string toAddress, string toName)
        {
            try
            {
                for (int i = 0; i < Pop3?.Count; i++)
                {
                    MimeMessage message = Pop3.GetMessage(i);

                    if (message != null)
                    {
                        string[] words = GetPop3EmailWords(ref message);

                        if (words?.Length > 0)
                            SendResponses(words, smtp, user, pwd, port, toAddress, toName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void AutomatedSmtpResponsesImap(string smtp, string user, string pwd, int port, string toAddress, string toName)
        {
            try
            {
                var folder = Imap?.Inbox.Open(FolderAccess.ReadOnly);

                for (int i = 0; i < Imap?.Inbox.Count; i++)
                {
                    MimeMessage message = Imap.Inbox.GetMessage(i);

                    if (message != null)
                    {
                        string[] words = GetPop3EmailWords(ref message);

                        if (words?.Length > 0)
                            SendResponses(words, smtp, user, pwd, port, toAddress, toName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SavePop3BodyAndAttachments(string path)
        {
            try
            {
                for (int i = 0; i < Pop3?.Count; i++)
                {
                    MimeMessage msg = Pop3.GetMessage(i);

                    if (msg != null)
                    {
                        string b = msg.GetTextBody(MimeKit.Text.TextFormat.Text);

                        Console.WriteLine("Body: {0}", b);

                        if (msg.Attachments != null)
                        {
                            foreach (MimeEntity att in msg.Attachments)
                            {
                                if (att.IsAttachment)
                                {
                                    if (!Directory.Exists(path))
                                        Directory.CreateDirectory(path);

                                    string fn = Path.Combine(path,
                                        att.ContentType.Name);

                                    if (File.Exists(fn))
                                        File.Delete(fn);

                                    using (var stream = File.Create(fn))
                                    {
                                        var mp = ((MimePart)att);
                                        mp.ContentObject.DecodeTo(stream);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SavePop3BodyAndAttachments_Old(string path)
        {
            try
            {
                for (int i = 0; i < Pop3?.Count; i++)
                {
                    MimeMessage message = Pop3.GetMessage(i);

                    if (message != null)
                    {
                        string b = message.GetTextBody(MimeKit.Text.TextFormat.Text);

                        Console.WriteLine("Body: {0}", b);

                        if (message.Attachments != null)
                        {
                            foreach (MimeEntity att in message.Attachments)
                            {
                                if (att.IsAttachment)
                                {
                                    if (!Directory.Exists(path))
                                        Directory.CreateDirectory(path);

                                    string fn = Path.Combine(path, att.ContentType.Name);

                                    if (File.Exists(fn))
                                        File.Delete(fn);

                                    att.WriteTo(fn);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SaveImapBodyAndAttachments_Old(string path)
        {
            try
            {
                var folder = Imap?.Inbox.Open(FolderAccess.ReadOnly);

                for (int i = 0; i < Imap?.Inbox.Count; i++)
                {
                    MimeMessage message = Imap.Inbox.GetMessage(i);

                    if (message != null)
                    {
                        string b = message.GetTextBody(MimeKit.Text.TextFormat.Text);

                        Console.WriteLine("Body: {0}", b);

                        if (message.Attachments != null)
                        {
                            foreach (MimeEntity att in message.Attachments)
                            {
                                if (att.IsAttachment)
                                {
                                    if (!Directory.Exists(path))
                                        Directory.CreateDirectory(path);

                                    string fn = Path.Combine(path, att.ContentType.Name);

                                    if (File.Exists(fn))
                                        File.Delete(fn);

                                    att.WriteTo(fn);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SaveImapBodyAndAttachments(string path)
        {
            try
            {
                var folder = Imap?.Inbox.Open(FolderAccess.ReadOnly);

                for (int i = 0; i < Imap?.Inbox.Count; i++)
                {
                    MimeMessage message = Imap.Inbox.GetMessage(i);

                    if (message != null)
                    {
                        string b = message.GetTextBody(MimeKit.Text.TextFormat.Text);

                        Console.WriteLine("Body: {0}", b);

                        if (message.Attachments != null)
                        {
                            foreach (MimeEntity att in message.Attachments)
                            {
                                if (att.IsAttachment)
                                {
                                    if (!Directory.Exists(path))
                                        Directory.CreateDirectory(path);

                                    string fn = Path.Combine(path,
                                        att.ContentType.Name);

                                    if (File.Exists(fn))
                                        File.Delete(fn);

                                    using (var stream = File.Create(fn))
                                    {
                                        var mp = ((MimePart)att);
                                        mp.ContentObject.DecodeTo(stream);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void OpenPop3()
        {
            try
            {
                if (Pop3 == null)
                {
                    Pop3 = new Pop3Client();

                    Pop3.Connect(this.MailServer, this.Port, false);
                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    Pop3.AuthenticationMechanisms.Remove("XOAUTH2");
                    Pop3.Authenticate(this.User, this.Pwd);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void OpenImap()
        {
            try
            {
                if (Imap == null)
                {
                    Imap = new ImapClient();

                    Imap.Connect(this.MailServer, this.Port, true);
                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    Imap.AuthenticationMechanisms.Remove("XOAUTH2");
                    Imap.Authenticate(this.User, this.Pwd);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ClosePop3()
        {
            try
            {
                if (Pop3 != null)
                {
                    Pop3.Disconnect(true);
                    Pop3.Dispose();
                    Pop3 = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void CloseImap()
        {
            try
            {
                if (Imap != null)
                {
                    Imap.Disconnect(true);
                    Imap.Dispose();
                    Imap = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion

        #region "Protected Dispose"
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        /// <remarks>
        /// If the main class was marked as sealed, we could just make this a private void Dispose(bool).  Alternatively, we could (in this case) put
        /// all of our logic directly in Dispose().
        /// </remarks>
        public virtual void Dispose(bool disposing)
        {
            // Use our disposed flag to allow us to call this method multiple times safely.
            // This is a requirement when implementing IDisposable.
            if (!this.disposed)
            {
                if (disposing)
                {
                    // If we have any managed, IDisposable resources, Dispose of them here.
                    // In this case, we don't, so this was unneeded.
                    // Later, we will subclass this class, and use this section.

                    if (Pop3 != null)
                    {
                        Pop3.Disconnect(true);
                        Pop3.Dispose();
                        Pop3 = null;
                    }

                    if (Imap != null)
                    {
                        Imap.Disconnect(true);
                        Imap.Dispose();
                        Imap = null;
                    }
                }

                // Always dispose of undisposed unmanaged resources in Dispose(bool).
            }
            // Mark us as disposed, to prevent multiple calls to dispose from having an effect, 
            // and to allow us to handle ObjectDisposedException.
            this.disposed = true;
        }
        #endregion

        #region "Dispose"
        /// <summary>
        /// Dispose() --> Performs Internals defined tasks associated with freeing, releasing, or resetting managed and unmanaged resources.
        /// </summary>
        /// <example><code>s.Dispose();</code></example>
        public void Dispose()
        {
            // We start by calling Dispose(bool) with true.
            this.Dispose(true);

            // Now suppress finalization for this object, since we've already handled our resource cleanup tasks.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    #endregion
}
#endregion
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
#endregion

#region "namespace"
namespace EmailProcessing
{
    #region "EmailExample"
    public class EmailExample
    {
        #region "private vars & consts"
        private const string cPopUserName = "pop@your-domain.com";
        private const string cPopPwd = "your-password";
        private const string cPopMailServer = "mail.your-domain.com";
        private const int cPopPort = 110;

        private const string cImapUserName = "imap@your-domain.com";
        private const string cImapPwd = "your-password";
        private const string cImapMailServer = "mail.your-domain.com";
        private const int cImapPort = 993;

        private const string cSmtpUserName = "smtp@your-domain.com";
        private const string cSmtpPwd = "your-password";
        private const string cSmtpMailServer = "mail.your-domain.com";
        private const int cSmptPort = 465;
        #endregion

        #region "public"
        public static void ShowPop3Subjects()
        {
            try
            {
                using (EmailParser ep = new EmailParser(cPopUserName, cPopPwd, cPopMailServer, cPopPort))
                {
                    ep.OpenPop3();

                    ep.DisplayPop3Subjects();

                    ep.ClosePop3();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void ShowImapSubjects()
        {
            try
            {
                using (EmailParser ep = new EmailParser(cImapUserName, cImapPwd, cImapMailServer, cImapPort))
                {
                    ep.OpenImap();

                    ep.DisplayImapSubjects();

                    ep.CloseImap();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void DisplayHeaderInfo()
        {
            try
            {
                using (EmailParser ep = new EmailParser(cPopUserName, cPopPwd, cPopMailServer, cPopPort))
                {
                    ep.OpenPop3();
                    
                    ep.DisplayPop3HeaderInfo();

                    ep.ClosePop3();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void DisplayHeaderInfoImap()
        {
            try
            {
                using (EmailParser ep = new EmailParser(cImapUserName, cImapPwd, cImapMailServer, cImapPort))
                {
                    ep.OpenImap();

                    ep.DisplayImapHeaderInfo();

                    ep.CloseImap();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void AutomatedSmtpResponses()
        {
            try
            {
                using (EmailParser ep = new EmailParser(cPopUserName, cPopPwd, cPopMailServer, cPopPort))
                {
                    ep.OpenPop3();

                    ep.AutomatedSmtpResponses(cSmtpMailServer, cSmtpUserName, cSmtpPwd, cSmptPort, "to@another-domain.com", "Your customer");

                    ep.ClosePop3();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void AutomatedSmtpResponsesImap()
        {
            try
            {
                using (EmailParser ep = new EmailParser(cImapUserName, cImapPwd, cImapMailServer, cImapPort))
                {
                    ep.OpenImap();

                    ep.AutomatedSmtpResponsesImap(cSmtpMailServer, cSmtpUserName, cSmtpPwd, cSmptPort, "to@another-domain.com", "Your customer");

                    ep.CloseImap();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void SavePop3BodyAndAttachments(string path)
        {
            try
            {
                using (EmailParser ep = new EmailParser(cPopUserName, cPopPwd, cPopMailServer, cPopPort))
                {
                    ep.OpenPop3();

                    ep.SavePop3BodyAndAttachments(path);

                    ep.ClosePop3();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void SaveImapBodyandAttachments(string path)
        {
            try
            {
                using (EmailParser ep = new EmailParser(cImapUserName, cImapPwd, cImapMailServer, cImapPort))
                {
                    ep.OpenImap();

                    ep.SaveImapBodyAndAttachments(path);

                    ep.CloseImap();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void OpenClosePop3()
        {
            try
            {
                using (EmailParser ep = new EmailParser(cPopUserName, cPopPwd, cPopMailServer, cPopPort))
                {
                    ep.OpenPop3();
                    ep.ClosePop3();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void OpenCloseImap()
        {
            try
            {
                using (EmailParser ep = new EmailParser(cImapUserName, cImapPwd, cImapMailServer, cImapPort))
                {
                    ep.OpenImap();
                    ep.CloseImap();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion
    }
    #endregion
}
#endregion
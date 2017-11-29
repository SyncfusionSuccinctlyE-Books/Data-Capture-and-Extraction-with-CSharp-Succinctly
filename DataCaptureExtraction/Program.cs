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

using EmailProcessing;
using ImageProcessing;
using WebProcessing;
using TextProcessing;
using System.Text.RegularExpressions;
#endregion

#region "namespace"
namespace DataCaptureExtraction
{
    #region "program"
    class Program
    {
        public static void RegExExample()
        {
            // First we see the input string.
            string input = "256.58.125.121";

            // Here we call Regex.Match.
            Match match = Regex.Match(input, @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
                RegexOptions.IgnoreCase);

            // Here we check the Match instance.
            if (match.Success)
            {
                // Finally, we get the Group value and display it.
                Console.WriteLine(match.Value);
            }
            else
                Console.WriteLine("No match");
        }

        static void Main(string[] args)
        {
            //EmailExample.ShowPop3Subjects();
            //EmailExample.DisplayPop3HeaderInfo();

            EmailExample.SavePop3BodyAndAttachments(
              @"C:\Attach");

            //EmailExample.AutomatedSmtpResponses();

            //EmailExample.ShowImapSubjects();
            //EmailExample.DisplayHeaderInfoImap();
            EmailExample.SaveImapBodyandAttachments(
                @"C:\Attach");

            //EmailExample.AutomatedSmtpResponsesImap();

            //ImageExample.GetImageWords();

            //WebExample.DisplayNYTBestSellersListNames();

            //BayesExample.BayesExample2();

            //RegExExample();

            //NerExample.nerExample();
            //Console.ReadLine();
        }
    }
    #endregion
}
#endregion
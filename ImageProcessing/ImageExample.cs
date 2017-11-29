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
namespace ImageProcessing
{
    #region "ImageExample"
    public class ImageExample
    {
        public static string[] GetImageWords()
        {
            List<string> w = new List<string>();

            try
            {
                using (ImageParser ip = new ImageParser(@"C:\Emgu\emgucv-windows-universal 3.0.0.2157\bin\tessdata", "eng"))
                {
                    if (ip.OcrImage(
                        @"C:\Images\Figure1-c.png")
                        != string.Empty)
                        w.AddRange(ip?.Words.ToList<string>());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return w.ToArray();
        }
    }
    #endregion
}
#endregion
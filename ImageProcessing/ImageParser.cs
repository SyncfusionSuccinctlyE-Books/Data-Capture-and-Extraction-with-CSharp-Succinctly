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

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.OCR;
using System.IO;
#endregion

#region "namespace"
namespace ImageProcessing
{
    #region "ImageParser"
    public class ImageParser: IDisposable
    {
        #region "private"
        /// <summary>
        /// When set to true indicates that the instance of the class has been disposed.
        /// </summary>
        protected bool disposed;
        #endregion

        #region "properties"
        public Tesseract OcrEngine { get; set; }
        public string[] Words { get; set; }
        #endregion

        #region "constructors-destructors"
        public ImageParser()
        {
            OcrEngine = null;
        }

        public ImageParser(string dataPath, string lang)
        {
            try
            {
                OcrEngine = new Tesseract(dataPath, lang, OcrEngineMode.TesseractCubeCombined);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        ~ImageParser()
        {
            // Our finalizer should call our Dispose(bool) method with false.
            this.Dispose(false);
        }
        #endregion

        #region "public"
        public string OcrImage(string img)
        {
            string res = String.Empty;
            List<string> wrds = new List<string>();

            try
            {
                if (File.Exists(img))
                {
                    using (Image<Bgr, byte> i = new Image<Bgr, byte>(img))
                    {
                        if (OcrEngine != null)
                        {
                            OcrEngine.Recognize(i);
                            res = OcrEngine.GetText().TrimEnd();

                            wrds.AddRange(res.Split(new string[] { " ", "\r", "\n" },
                                StringSplitOptions.RemoveEmptyEntries).ToList());

                            this.Words = wrds?.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return res;
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
                    if (OcrEngine != null)
                    {
                        OcrEngine.Dispose();
                        OcrEngine = null;
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
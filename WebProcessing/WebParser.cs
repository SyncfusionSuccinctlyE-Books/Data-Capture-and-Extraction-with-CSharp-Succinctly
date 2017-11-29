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

using RestSharp;
using Newtonsoft.Json;
#endregion

#region "namespace"
namespace WebProcessing
{
    #region "WebParser"
    public class WebParser : IDisposable
    {
        #region "protected"
        protected RestClient WebClient = null;
        protected string response = String.Empty;
        #endregion

        #region "public"
        public string Response
        {
            get { return response; }
        }
        #endregion

        #region "methods"
        public string Request(string resource, Method type, string param, string value)
        {
            string res = String.Empty;

            try
            {
                if (WebClient != null)
                {
                    var request = new RestRequest(resource, type);
                    request.AddParameter(param, value);

                    IRestResponse htmlRes = WebClient.Execute(request);
                    res = htmlRes.Content;

                    if (res != String.Empty)
                        response = res;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return res;
        }

        public T DeserializeJson<T>(string res)
        {
            return JsonConvert.DeserializeObject<T>(res);
        }
        #endregion

        #region "private"
        /// <summary>
        /// When set to true indicates that the instance of the class has been disposed.
        /// </summary>
        protected bool disposed;
        #endregion

        #region "constructors-destructors"
        public WebParser()
        {
            WebClient = null;
            response = String.Empty;
        }

        public WebParser(string baseUrl)
        {
            try
            {
                WebClient = new RestClient(baseUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        ~WebParser()
        {
            this.Dispose(false);
        }
        #endregion

        #region "Protected Dispose"
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        /// <remarks>
        /// If the main class was marked as sealed, we could just make this a private void Dispose(bool).  Alternatively, we could (in this case) put
        /// all of our logic directly in Dispose().
        /// </remarks>
        public virtual void Dispose(bool disposing)
        {
            // Use our disposed flag to allow us to call this method multiple times safely.
            // This is a requirement when implementing IDisposable
            if (!this.disposed)
            {
                if (disposing)
                {
                    // If we have any managed, IDisposable resources, Dispose of them here.
                    // In this case, we don't, so this was unneeded.
                    // Later, we will subclass this class, and use this section.
                    WebClient = null;
                }

                // Always dispose of undisposed unmanaged resources in Dispose(bool)
            }
            // Mark us as disposed, to prevent multiple calls to dispose from having an effect, 
            // and to allow us to handle ObjectDisposedException
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
            // We start by calling Dispose(bool) with true
            this.Dispose(true);

            // Now suppress finalization for this object, since we've already handled our resource cleanup tasks
            GC.SuppressFinalize(this);
        }
        #endregion "Dispose"

        #endregion
    }
}
#endregion
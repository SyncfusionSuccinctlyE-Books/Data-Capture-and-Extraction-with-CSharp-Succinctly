/*
Data_Capture_and_Extraction_with_C_Sharp_Succinctly.
Eduardo Freitas - 2016 - http://edfreitas.me
*/

#region "using"
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

#region "namespace"
namespace WebProcessing
{
    #region "BestSellerListNames"
    public class BestSellersListNames
    {
        public string status;
        public string copyright;
        public int num_results;
        public BestSellerListNamesItems[] results;
    }

    public class BestSellerListNamesItems
    {
        public string list_name;
        public string display_name;
        public string list_name_encoded;
        public DateTime oldest_published_date;
        public DateTime newest_published_date;
        public string updated;
    }
    #endregion

    #region "WebExample"
    public class WebExample
    {
        public const string cStrNYTBooksBaseUrl = "http://api.nytimes.com/svc/books/";
        public const string cStrNYTBooksResource = "v3/lists/names.json";
        public const string cStrNYTApiKeyStr = "api-key";
        public const string cStrNYTApiKeyVal = "<< YOUR NYT API KEY VALUE >>";

        public static string GetRawNYTBestSellersListNames()
        {
            string res = String.Empty;

            try
            {
                using (WebParser wp = new WebParser(cStrNYTBooksBaseUrl))
                {
                    res = wp.Request(cStrNYTBooksResource, Method.GET, cStrNYTApiKeyStr, cStrNYTApiKeyVal);

                    Console.WriteLine(res);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return res;
        }

        public static void DisplayNYTBestSellersListNames()
        {
            try
            {
                BestSellersListNames b = GetNYTBestSellersListNames();

                if (b != null && b.results.Length > 0)
                {
                    foreach (BestSellerListNamesItems i in b.results)
                    {
                        Console.WriteLine("Name: " + i.display_name);
                        Console.WriteLine("Oldest Published Date: " + i.oldest_published_date.ToString());
                        Console.WriteLine("Newest Published Date: " + i.newest_published_date.ToString());
                        Console.WriteLine(Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static BestSellersListNames GetNYTBestSellersListNames()
        {
            BestSellersListNames bs = null;

            try
            {
                using (WebParser wp = new WebParser(cStrNYTBooksBaseUrl))
                {
                    string res = wp.Request(cStrNYTBooksResource, Method.GET, cStrNYTApiKeyStr, cStrNYTApiKeyVal);

                    bs = wp.DeserializeJson<BestSellersListNames>(res);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return bs;
        }
    }
    #endregion
}
#endregion
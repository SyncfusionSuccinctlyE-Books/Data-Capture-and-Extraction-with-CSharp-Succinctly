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
using TextProcessing;
#endregion

#region "namespace"
namespace TextProcessing
{
    #region NerExample
    public class NerExample
    {
        public static void nerExample()
        {
            using (NER n = new NER())
            {
                string[] res = n.Recognize("I went to Stanford, which is located in California.");

                if (res != null && res.Length > 0)
                {
                    foreach (string r in res)
                    {
                        Console.WriteLine(r);
                    }
                }
            }
        }
    }
    #endregion

    #region "BayesExample"
    public class BayesExample
    {
        public static void BayesExample1()
        {
            using (BayesEngine b = new BayesEngine())
            {
                b.dataset.Add(new KeyValuePair<string, string[]>("Department", new string[] { "Finance", "Finance", "Finance", "IT", "Finance" }));
                b.dataset.Add(new KeyValuePair<string, string[]>("Sex", new string[] { "Female", "Female", "Male", "Male", "Male" }));
                b.dataset.Add(new KeyValuePair<string, string[]>("Age", new string[] { "32", "36", "46", "40", "30" }));
                b.dataset.Add(new KeyValuePair<string, string[]>("Role", new string[] { "Assistant Controller", "Senior Controller",
                "Finance Director", "IT Manager", "Financial Lead" }));

                // P(female | X) = [P(finance | female) * P(senior | female) * P(female)] / [PP(female | X) + PP(male | X)]
                double r = b.BayesAX("Female", new string[] { "Female", "Male" }, new string[] { "Sex", "Sex" },
                    new string[] { "Finance", "Controller" },
                    new string[] { "Department", "Role" }, "Sex");

                Console.WriteLine("P(female|(finance|senior)) with smoothing: " + r);
            }
        }

        public static void BayesExample2()
        {
            using (BayesEngine b = new BayesEngine())
            {
                b.dataset.Add(new KeyValuePair<string, string[]>("Sex", new string[] 
                { "male", "male", "male", "male", "male", "male", "male", "male", "male", "male", "male", "male",
                  "male", "male", "male", "male", "male", "male", "male", "male", "male", "male", "male", "male",
                  "female", "female", "female", "female", "female", "female", "female", "female", "female", "female",
                  "female", "female", "female", "female", "female", "female" }));

                b.dataset.Add(new KeyValuePair<string, string[]>("Prof", new string[] 
                { "tech", "tech", "tech", "tech", "tech", "tech", "tech", "tech", "tech", "tech", "tech", "tech",
                  "tech", "tech", "tech", "const", "const", "const", "const", "const", "admin", "admin", "edu", "edu",
                  "admin", "admin", "admin", "admin", "admin", "admin", "admin", "edu", "edu", "edu", "edu", "tech",
                  "tech", "tech", "tech", "tech" }));

                b.dataset.Add(new KeyValuePair<string, string[]>("Hand", new string[] 
                { "left", "left", "left", "left", "left", "left", "left", "right", "right", "right", "right", "right",
                  "right", "right", "right", "right", "right", "right", "right", "right", "right", "right", "right", "right",
                  "left", "left", "right", "right", "right", "right", "right", "right", "right", "right", "right", "right",
                 "right", "right", "right", "right" }));

                b.dataset.Add(new KeyValuePair<string, string[]>("Height", new string[] 
                { "short", "tall", "tall", "tall", "tall", "medium", "medium", "medium", "medium", "medium", "medium",
                  "medium", "medium", "medium", "medium", "medium", "medium", "medium", "medium", "medium", "medium",
                  "medium", "medium", "medium", "short", "short", "short", "short", "short", "short", "tall", "tall",
                  "medium", "medium", "medium", "medium", "medium", "medium", "medium", "medium" }));

                // P(male|(edu|right|tall)) with smoothing
                double r1 = b.BayesAX("male", new string[] { "male", "female" }, new string[] { "Sex", "Sex" },
                    new string[] { "edu", "right", "tall" }, new string[] { "Prof", "Hand", "Height" },
                    "Sex");

                // P(male|(edu|right|tall)) without smoothing
                double r2 = b.BayesAX("male", new string[] { "male", "female" }, new string[] { "Sex", "Sex" },
                    new string[] { "edu", "right", "tall" }, new string[] { "Prof", "Hand", "Height" },
                    "Sex", false);

                // P(female|(edu|right|tall)) with smoothing
                double r3 = b.BayesAX("female", new string[] { "male", "female" }, new string[] { "Sex", "Sex" },
                    new string[] { "edu", "right", "tall" }, new string[] { "Prof", "Hand", "Height" },
                    "Sex");

                // P(female|(edu|right|tall)) without smoothing
                double r4 = b.BayesAX("female", new string[] { "male", "female" }, new string[] { "Sex", "Sex" },
                    new string[] { "edu", "right", "tall" }, new string[] { "Prof", "Hand", "Height" },
                    "Sex" ,false);

                Console.WriteLine("P(male|(edu|right|tall)) with smoothing: " + r1);
                Console.WriteLine("P(male|(edu|right|tall)) without smoothing: " + r2);

                Console.WriteLine("P(female|(edu|right|tall)) with smoothing: " + r3);
                Console.WriteLine("P(female|(edu|right|tall)) without smoothing: " + r4);
            }
        }
    }
    #endregion
}
#endregion

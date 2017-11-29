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

using edu.stanford.nlp.ie.crf;
using edu.stanford.nlp.pipeline;
using edu.stanford.nlp.util;
#endregion

#region "namespace"
namespace TextProcessing
{
    #region "BayesEngine"
    public class BayesEngine : IDisposable
    {
        #region "protected"
        /// <summary>
        /// When set to true indicates that the instance of the class has been disposed.
        /// </summary>
        protected bool disposed;
        #endregion

        #region "constructors-destructors"
        public BayesEngine()
        {
            dataset = new List<KeyValuePair<string, string[]>>();
            StrictCounting = false;
        }

        ~BayesEngine()
        {
            this.Dispose(false);
        }
        #endregion

        #region "protected methods"
        protected bool ExistsAinXRow(int xRow, string A, string aCol = "")
        {
            bool res = false;

            try
            {
                if (dataset != null)
                {
                    foreach (KeyValuePair<string, string[]> column in dataset)
                    {
                        if (aCol == String.Empty || column.Key.ToUpper().Contains(aCol.ToUpper()))
                        {
                            if (StrictCounting)
                                res = (column.Value[xRow].ToUpper() == A.ToUpper()) ? true : false;
                            else
                            {
                                if (column.Value[xRow].ToUpper().Contains(" "))
                                    res = (column.Value[xRow].ToUpper().Contains(A.ToUpper())) ? true : false;
                                else
                                    res = (column.Value[xRow].ToUpper() == A.ToUpper()) ? true : false;
                            }

                            if (res) break;
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

        #region "public methods"
        public List<KeyValuePair<string, string[]>> dataset = null;
        public bool StrictCounting { get; set; }

        // count(finance & male)
        public double CountXA(string xAttribute, string A, string xCol = "", string aCol = "")
        {
            double res = 0;

            try
            {
                if (dataset != null)
                {
                    foreach (KeyValuePair<string, string[]> xColumn in dataset)
                    {
                        if (xCol == String.Empty || xColumn.Key.ToUpper().Contains(xCol.ToUpper()))
                        {
                            int xRow = 0;
                            foreach (string x in xColumn.Value)
                            {
                                if (StrictCounting)
                                {
                                    if (x.ToUpper() == xAttribute.ToUpper() && ExistsAinXRow(xRow, A, aCol))
                                        res++;
                                }
                                else
                                {
                                    if (x.ToUpper().Contains(" "))
                                    {
                                        if (x.ToUpper().Contains(xAttribute.ToUpper()) && ExistsAinXRow(xRow, A, aCol))
                                            res++;
                                    }
                                    else
                                        if (x.ToUpper() == xAttribute.ToUpper() && ExistsAinXRow(xRow, A, aCol))
                                            res++;
                                }

                                xRow++;
                            }
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

        // count(female) -- where female is a group
        public double CountA(string A, string col = "")
        {
            double res = 0;

            try
            {
                if (dataset != null)
                {
                    foreach (KeyValuePair<string, string[]> column in dataset)
                    {
                        if (col == String.Empty || column.Key.ToUpper().Contains(col.ToUpper()))
                        {
                            foreach (string wrd in column.Value)
                            {
                                if (StrictCounting)
                                {
                                    if (wrd.ToUpper() == A.ToUpper())
                                        res++;
                                }
                                else
                                {
                                    if (wrd.ToUpper().Contains(" "))
                                    {
                                        if (wrd.ToUpper().Contains(A.ToUpper()))
                                            res++;
                                    }
                                    else
                                        if (wrd.ToUpper() == A.ToUpper())
                                            res++;
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

            return res;
        } 

        // count(sex) -- where sex is a column (all groups)
        public double CountCol(string col)
        {
            double res = 0;

            try
            {
                if (dataset != null)
                {
                    foreach (KeyValuePair<string, string[]> column in dataset)
                    {
                        if (col != String.Empty && column.Key.ToUpper().Contains(col.ToUpper()))
                        {
                            res = column.Value.Length;
                            break;
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

        // P(male) = count (male) / count (sex)
        public double ProbA(string A, string aCol)
        {
            double res = 0;

            try
            {
                res = CountA(A, aCol) / CountCol(aCol);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return res;
        }

        // P(finance | male) = count(finance & male) / count(male)
        public double ProbXA(string xAttribute, string A, string xCol = "", string aCol = "")
        {
            double res = 0;

            try
            {
                res = CountXA(xAttribute, A, xCol, aCol) / CountA(A, aCol);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return res;
        }

        // P(finance | male) = count(finance & male) + 1 / count(male) + 3 (Add-one smoothing)
        public double SmoothingProbXA(string xAttribute, string A, int numAttributes, string xCol = "", string aCol = "")
        {
            double res = 0;

            try
            {
                res = (CountXA(xAttribute, A, xCol, aCol) + 1) / (CountA(A, aCol) + numAttributes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return res;
        }

        // Decides whether to use ProbXA or SmoothingProbXA
        public double CalcProbXA(bool smoothing, string xAttribute, string A, int numAttributes = 0, string xCol = "", string aCol = "")
        {
            double res = 0;

            try
            {
                res = ProbXA(xAttribute, A, xCol, aCol);

                res = (res == 0 || smoothing) ? SmoothingProbXA(xAttribute, A, numAttributes, xCol, aCol) : res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return res;
        }

        // PP(male | X) = P(finance | male) * P(<40 | male) * P(senior | male) * P(male)
        public double PProbAX(bool smoothing, string A, string[] xAttributes, string[] xColls, string aCol = "")
        {
            double res = 0;

            try
            {
                if (xAttributes != null && xAttributes.Length > 0)
                {
                    int i = 0;

                    List<double> rlts = new List<double>();

                    foreach (string xAtrrib in xAttributes)
                    {
                        string xCol = (xColls != null && xColls.Length > 0 &&
                            xColls.Length == xAttributes.Length) ? xColls[i] : String.Empty;

                        rlts.Add(CalcProbXA(smoothing, xAtrrib, A, xAttributes.Length, xCol, aCol));

                        i++;
                    }

                    rlts.Add(ProbA(A, aCol));

                    double tmp = 0;
                    int cnt = 0;

                    foreach (double r in rlts)
                    {
                        tmp = (cnt == 0) ? r : tmp *= r;
                        cnt++;
                    }

                    res = tmp;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return res;
        }

        // P(female | X) = [P(finance | female) * P(<40 | female) * P(senior | female) * P(female)] / [PP(female | X) + PP(male | X)]
        public double BayesAX(string A, string[] G, string[] gColls, string[] xAttributes, string[] xColls, string aCol = "", bool smoothing = true)
        {
            double res = 0;

            try
            {
                double nonimator = PProbAX(smoothing, A, xAttributes, xColls, aCol);
                double denominator = 0;

                if (G != null && G.Length > 0 && gColls != null && gColls.Length > 0)
                {
                    if (G.Length == gColls.Length)
                    {
                        int i = 0;
                        foreach (string group in G)
                        {
                            denominator += PProbAX(smoothing, group, xAttributes, xColls, gColls[i]);
                            i++;
                        }
                    }
                }

                if (denominator > 0)
                    res = nonimator / denominator;
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
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    dataset = null;
                }

                this.disposed = true;
            }
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
    }
    #endregion

    #region "NER"
    public class NER : IDisposable
    {
        #region "protected"
        /// <summary>
        /// When set to true indicates that the instance of the class has been disposed.
        /// </summary>
        protected bool disposed;

        protected CRFClassifier Classifier = null;

        protected string[] ParseResult(string txt)
        {
            List<string> res = new List<string>();

            try
            {
                string[] tmp = txt.Split(' ');

                if (tmp != null && tmp.Length > 0)
                {
                    foreach (string t in tmp)
                    {
                        if (t.Count(x => x == '/') == 2)
                        {
                            res.Add(t.Substring(0, t.LastIndexOf("/") - 1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return res.ToArray();
        }
        #endregion

        #region "constructors-destructors"
        public NER()
        {
            try
            {
                string root = @"D:\Temp\NER\classifiers";
                Classifier = CRFClassifier.getClassifierNoExceptions(root + @"\english.all.3class.distsim.crf.ser.gz");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        ~NER()
        {
            this.Dispose(false);
        }
        #endregion

        #region "public"
        public string[] Recognize(string txt)
        {
            return ParseResult(Classifier.classifyToString(txt));
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

                    Classifier = null;
                }
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
    }
    #endregion
}
#endregion
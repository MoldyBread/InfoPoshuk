using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slovnyk
{
    [Serializable]
    public class MyDictionary
    {
        public SortedDictionary<string, Counter> Terms { get; }

        private int docsCount;
        private int docNow = 0;

        public MyDictionary(int n)
        {
            docsCount = n;
            Terms = new SortedDictionary<string, Counter>();
        }

        public void add(string term, ref Dictionary<string, bool[]> matrix)
        {

            if (!Terms.ContainsKey(term))
            {
                Terms.Add(term, new Counter());
                Terms[term].Add(docNow);

                matrix.Add(term, new bool[docsCount]);
                matrix[term][docNow] = true;
            }
            else
            {
                Terms[term].Add(docNow);
                matrix[term][docNow] = true;
            }
        }

        //public void sort()
        //{
        //    Terms.
        //}

        public void nextDoc()
        {
            docNow++;
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slovnyk
{
    [Serializable]
    public class Counter
    {
        private int[] counter;
        public Counter()
        {
            counter = new int[0];
        }

        public void Add(int n)
        {
            if(!counter.Contains(n))
            {
                Array.Resize(ref counter, counter.Length + 1);
                counter[counter.Length - 1] = n;
            }
        }

        public override string ToString()
        {
            string s="{";
            for (int i = 0; i < counter.Length; i++)
            {
                s += i < counter.Length - 1 ? counter[i].ToString() + ", " : counter[i].ToString()+"}";
            }
            return s;
        }

        public int[] ThisCounter
        {
            get { return counter; }
        }

    }
}

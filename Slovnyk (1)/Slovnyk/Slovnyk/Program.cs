using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections;

namespace Slovnyk
{
    
    class Program
    {
        private static MyDictionary myDictionary;
        private static Dictionary<string, bool[]> matrix;
        private static string[] dirs;
        private static string[] lines = null;

        static void Main(string[] args)
        {
            long firstCounter = 0;
            long secondCounter=0;

            matrix = new Dictionary<string, bool[]>();
            
            dirs = Directory.GetFiles(@"toRead");
            
            Regex regex = new Regex(@"\W");

            myDictionary = new MyDictionary(dirs.Length);

            for (int i = 0; i < dirs.Length; i++)
            {
                Console.WriteLine("Processing "+ dirs[i].Split('\\')[1]+ " file. ");
                secondCounter += new FileInfo(dirs[i]).Length;
                
                if (dirs[i].Contains(".fb2"))//FOR FB2
                {
                    XElement el = XElement.Load(dirs[i]);
                    lines = el.Value.Split(' ');
                    firstCounter += lines.Length;
                    foreach (string term in lines)
                    {
                        
                        myDictionary.add(regex.Replace(term,""),ref matrix);
                    }
                }
                else
                using (StreamReader sr = new StreamReader(File.Open(dirs[i], FileMode.Open))) //FOR TXT
                {
                    while (!sr.EndOfStream)
                    {
                        lines= sr.ReadLine().Split(' ');
                        firstCounter += lines.Length;
                        foreach (string term in lines)
                        {
                            
                            myDictionary.add(regex.Replace(term,""),ref matrix);
                        }
                    }   
                }

                dirs[i] = dirs[i].Split('\\')[1];
                myDictionary.nextDoc();
                
            }

            Console.WriteLine("Processed.\n");

            Console.WriteLine("Size of collection: " + secondCounter / 1024 + "Kb");
            Console.WriteLine("Count of words in readed files: " + firstCounter);
            using (Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, myDictionary.Terms);
                firstCounter = stream.Length;
            }

            Console.WriteLine("Size of dictionary: "+firstCounter/1024+"Kb");
            Console.WriteLine("Terms in dictionary: " + myDictionary.Terms.Count);
            

            if (File.Exists("SavedDict.txt"))
            {
                File.Delete("SavedDict.txt");
            }
            using (FileStream fs = File.Create("SavedDict.txt"))
                new BinaryFormatter().Serialize(fs, myDictionary);

           

            new System.Threading.Thread(NewForm).Start();

            while (true)
            {
                Console.WriteLine("\nBool search(AND, OR, NOT - upper letters, with spaces): ");
                boolSearch();
            }
            

        }

        private static void NewForm()
        {
            FormMatrix f = new FormMatrix(myDictionary.Terms, matrix, dirs);
            f.ShowDialog();
            f.SendToBack();
        }

        private static void boolSearch()
        {
            string[] helpAND=new string[0], helpOR=new string[0];
            string s = Console.ReadLine();
            int[] intersection=new int[0];



            lines = Regex.Split(s, " AND ");

            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].Contains(" OR ") )
                {
                    Array.Resize(ref helpAND, helpAND.Length + 1);
                    helpAND[helpAND.Length - 1] = lines[i];
                }
                else
                {
                    Array.Resize(ref helpOR, helpOR.Length + 1);
                    helpOR[helpOR.Length - 1] = lines[i];
                }

            }


            for (int i = 0; i < helpOR.Length; i++)
            {
                lines = Regex.Split(helpOR[i], " OR ");
                intersection = intersection.Union(invertedList(ref lines[0]).Union(invertedList(ref lines[1])).ToArray()).ToArray();
            }


            if (intersection.Length == 0)
            {
                if (helpAND.Length > 1)
                {
                    intersection = invertedList(ref helpAND[0]).Intersect(invertedList(ref helpAND[1])).ToArray();

                    for (int i = 2; i < helpAND.Length; i++)
                        intersection = intersection.Intersect(invertedList(ref helpAND[i])).ToArray();
                }
                else if (helpAND.Length == 1)
                    intersection = invertedList(ref helpAND[0]);
            }
            else
            {
                for (int i = 0; i < helpAND.Length; i++)
                    intersection = intersection.Intersect(invertedList(ref helpAND[i])).ToArray();
            }
            
            showIntersection(intersection);
            

            
        }

        private static int[] invertedList(ref string term)
        {
            try
            {
                if (term.Contains("NOT "))
                {
                    int[] fullList = new int[dirs.Length];
                    for (int i = 0; i < fullList.Length; i++)
                    {
                        fullList[i] = i;
                    }

                    return fullList.Except(myDictionary.Terms[term.Replace("NOT ", "")].ThisCounter).ToArray();
                }

                return myDictionary.Terms[term.Replace("NOT ", "")].ThisCounter;
            }
            catch
            {
                return new int[0];
            }
            
        }

        private static void showIntersection(int[] intersection)
        {
            if (intersection.Length > 0)
            {
                string s="";

                for (int i = 0; i < intersection.Length; i++)
                {
                    s+=intersection[i] + (i < intersection.Length - 1 ? ", " : "");
                }

                if(s!="")
                    Console.WriteLine(s);
                else
                    Console.WriteLine("Not found.");
            }
            
            else
            {
                Console.WriteLine("Not found.");
            }
            
        }
    }
}

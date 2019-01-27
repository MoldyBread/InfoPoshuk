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
        static void Main(string[] args)
        {
            long firstCounter = 0;
            long secondCounter=0;

            Dictionary<string,bool[]> matrix = new Dictionary<string, bool[]>();
            
            string[] dirs = Directory.GetFiles(@"toRead");
            string[] lines=null;
            Regex regex = new Regex(@"\W");

            MyDictionary myDictionary = new MyDictionary(dirs.Length);

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


            FormMatrix f = new FormMatrix(myDictionary.Terms,matrix, dirs);

            Application.Run(f);

        }

        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Slovnyk
{
    public partial class FormMatrix : Form
    {
        private Dictionary<string, bool[]> matrix; //Matrix
        private SortedDictionary<string, Counter> myDictionary; //Here we can get index values
        private string[] dirs; //To show file names for matrix


        //Showed first 100 words !!!

        public FormMatrix(SortedDictionary<string, Counter> myDictionary, Dictionary<string,bool[]> matrix, string[] dirs)
        {
            InitializeComponent();
            this.matrix = matrix;
            this.dirs = dirs;
            this.myDictionary = myDictionary;

            var column = new DataGridViewColumn();
            column.HeaderText = "WORD";
            column.CellTemplate = new DataGridViewTextBoxCell();
            dataGridView1.Columns.Add(column);


            for (int i = 0; i < dirs.Length; i++)
            {
                column = new DataGridViewColumn();
                column.HeaderText = dirs[i];
                column.CellTemplate = new DataGridViewTextBoxCell();
                dataGridView1.Columns.Add(column);
            }

            showMatrix();
        }


        private void showMatrix()
        {
            Clear();
            for (int i = 1; i <= dirs.Length; i++)
            {
                dataGridView1.Columns[i].HeaderText = dirs[i-1];
            }

            for (int i = 0; i < 100; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1[0, i].Value = matrix.ElementAt(i).Key;
                for (int j = 1; j <= dirs.Length; j++)
                {
                    dataGridView1[j, i].Value = (matrix.ElementAt(i).Value[j - 1] ? 1 : 0);
                }
            }
        }

        private void showIndex()
        {
            Clear();
            for (int i = 1; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].HeaderText = "";
            }

            for (int i = 0; i < 100; i++)  
            {
                dataGridView1.Rows.Add();
                dataGridView1[0, i].Value = myDictionary.ElementAt(i+275).Key; //Here and a bit lower +275 is used to show normal words, not thrash like image links or smth

                for (int j = 1; j <= myDictionary.ElementAt(i+275).Value.ThisCounter.Length; j++)
                {
                    dataGridView1[j, i].Value = myDictionary.ElementAt(i+275).Value.ThisCounter[j - 1];
                }
            }

        }


        private bool swch=true;

        private void button1_Click(object sender, EventArgs e)
        {
            if(swch)
            {
                showIndex();
                label1.Text = "Index";
                swch = false;
            }
            else
            {
                showMatrix();
                label1.Text = "Matrix";
                swch = true;
            }
        }

        private void Clear()
        {
            while (dataGridView1.Rows.Count > 1)
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    dataGridView1.Rows.Remove(dataGridView1.Rows[i]);
        }

    }
}

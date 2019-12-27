using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Hidato_Solver_Gui_
{
    public partial class Form1 : Form
    {

        //OptionData od;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int nRows, nCols;
            try
            {
                nRows = int.Parse(textBox1.Text);
                nCols = int.Parse(textBox2.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Invlaid number of rows or columns specified.");
                return;
            }
            catch (OverflowException)
            {
                MessageBox.Show("Invlaid number of rows or columns specified.");
                return;
            }

            Hidato_Board board = new Hidato_Board(nCols, nRows);
            board.Show();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Hidato board files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.CheckFileExists = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {

            Stream stream = null;
            try
            {
                stream = File.OpenRead(textBox3.Text);
            }
            catch(System.Exception)
            {
                textBox3.Text = "";
                return;
            }
            

            StreamReader sr = new StreamReader(stream);

            Hidato_Board board = new Hidato_Board(sr);

            sr.Close();
            stream.Close();

            board.Show();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OptionDig dlg = new OptionDig();

            dlg.ShowDialog();
            dlg.Dispose();
        }
        
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}

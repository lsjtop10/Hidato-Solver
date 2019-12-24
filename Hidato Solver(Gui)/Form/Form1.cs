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
            int horizontalLn, verticalLn;
            try
            {
                horizontalLn = int.Parse(Horizontal.Text);
                verticalLn = int.Parse(Vertical.Text);
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

            Hidato_Board board = new Hidato_Board(horizontalLn, verticalLn);
#if DEBUG
            board.Show();
#endif
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}

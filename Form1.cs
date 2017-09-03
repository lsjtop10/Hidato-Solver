using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hidato_Solver_Gui_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using hidato_solver;
using System.IO;
namespace Hidato_Solver_Gui_
{

    public partial class Hidato_Board : Form
    { 
        private HidatoSolver hidato_solver;
        private TextBox[,] displayed;

        //텍스트 박스 상수
        private const int ColumnSpacing = 40; //열 간격
        private const int RowSpacing = 30; //행 간격
        private const int ColumnWidth = 35; //열 너비
        private const int RowHeight = 25; //행 높이
        private const int LeftMargin = 12; //왼쪽 여백
        private const int TopMargin = 12; //위쪽 여잭
        private const int RightMargin = 12; //오른쪽 여백
        private const int BottomMargin = 60; //아랫쪽 여백

        public Hidato_Board(int nCols, int nRows)
        {
            InitializeComponent();
            InitializeArrays(nCols, nRows);
            //ClearArrays();
        }


        private void InitializeArrays(int nCols, int nRows)
        {
            hidato_solver = new HidatoSolver(nCols, nRows);
            displayed = new TextBox[nCols, nRows];

            for (int i = 0; i < nCols; i++)
            {
                for (int j = 0; j < nRows; j++)
                {
                    displayed[i, j] = new TextBox();
                    displayed[i, j].Parent = this;
                    displayed[i, j].Location = new Point(j * ColumnSpacing + LeftMargin, i * RowSpacing + TopMargin); 
                    displayed[i, j].Size = new Size(ColumnWidth, RowHeight);
                    displayed[i, j].Visible = true;
                    displayed[i, j].TextChanged += new EventHandler(HidatoBoard_TextChanged);
                }
            }
            
            this.Size = new Size(nRows * ColumnSpacing + LeftMargin + RightMargin, nCols * RowSpacing + TopMargin + BottomMargin);

            int xPosSolve = (this.Size.Width - buttonSolve.Size.Width /*- button1.Size.Width*/ - ColumnSpacing) / 2;
            buttonSolve.Location = new Point(xPosSolve, nCols * RowSpacing + TopMargin);
            //button1.Location = new Point(xPosSolve + buttonSolve.Size.Width + ColumnSpacing, nRows * RowSpacing + TopMargin);
        }

        private void HidatoBoard_TextChanged(object sender, EventArgs e)
       {
            TextBox tb = (TextBox)sender;
            string s = tb.Text;
            // determine which row,col we are in
            int nCol = (tb.Location.Y - TopMargin) / RowSpacing;
            int nRow = (tb.Location.X - LeftMargin) / ColumnSpacing;
            
            int data = 0;
            try
            {
                data = int.Parse(s);
            }
            catch (FormatException) { }
            catch (OverflowException) { }

            if (data > hidato_solver.GridClength * hidato_solver.GridRlength)
            {
                tb.BackColor = Color.Red;
                tb.ForeColor = Color.Black;
                hidato_solver.SetDataAt(nCol, nRow, data);
            }
            else if (data == -1)
            {
                tb.BackColor = Color.Black;
                tb.ForeColor = Color.White;
                hidato_solver.SetDataAt(nCol, nRow, data);
            }
            else if (data > 0)
            {
                tb.BackColor = Color.LightGreen;
                tb.ForeColor = Color.Black;
                hidato_solver.SetDataAt(nCol, nRow, data);
            }
            else if (tb.TextLength == 0 || data == 0)
            {
                tb.BackColor = Color.White;
                tb.ForeColor = Color.Black;
                hidato_solver.SetDataAt(nCol, nRow, 0);
            }
            hidato_solver.show();
        }
    


        private void Hidato_Bord_Load(object sender, EventArgs e)
        {

        }

        public void UpdateTextBoxes()
        {
            for (int i = 0; i < hidato_solver.GridClength; i++)
            {
                for (int j = 0; j < hidato_solver.GridRlength; j++)
                {
                    //해당 칸에 들어있는 숫자가 0이면
                    int data = hidato_solver.GetDataAt(i, j);
                    if ( data == 0)
                    {   //공백 출력
                        displayed[i, j].Text = " ";
                    }
                    else
                    {
                        //아니면
                        //해당 칸에 있는 숫자를 문자열으로 바꾼 뒤, 해당 칸에 해당하는 텍스트 박스에 출력
                        displayed[i, j].Text = data.ToString();
                    }
                }
            }

            this.Refresh();
        }

        private void ClearArrays()
        {
            for(int i = 0; i < hidato_solver.GridClength; i++)
            {
                for(int j = 0; j < hidato_solver.GridRlength; j++)
                {
                    hidato_solver.SetDataAt(i, j, 0);
                }
            }
        }

        private void buttonSolve_Click(object sender, EventArgs e)
        {
           
            //문제를 푸는데 성공하면
            DateTime dtStart = DateTime.Now;
            bool success = hidato_solver.startsolve(this, 5);
            DateTime dtEnd = DateTime.Now;

            UpdateTextBoxes();

            if (success == true)
            {
                //성공했다는 메세지 박스를 출력함

                MessageBox.Show("Completed successfully. Time elapsed: " + (dtEnd - dtStart).ToString());
            }
            else
            {
                MessageBox.Show("Failed to solve.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateTextBoxes();
        }
    }
}

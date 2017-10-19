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
using System.Threading;

namespace Hidato_Solver_Gui_
{


    public partial class Hidato_Board : Form
    {
        private HidatoSolver hidato_solver;
        private HidatoGrid hidatoGrid;
        private TextBox[,] displayed;
        public delegate void VoidDelegate();
        public delegate void VoidDelgate2VII(int a, int b);

        /// <summary>
        /// 정적 속성 값
        /// </summary>
        //static public bool ShowAllProssace = false;
        //static public int NextUpdateSoconds = 5;
        //static public int processWaitTime = 0;

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

        public Hidato_Board(TextReader sr)
        {
            int col, row = 0;

            InitializeComponent();

            col = int.Parse(sr.ReadLine());
            row = int.Parse(sr.ReadLine());

            InitializeArrays(col, row);

            string s;

            for (int i = 0; i < col; i++)
            {
                s = sr.ReadLine();
                string[] sa = s.Split(' ');
                for (int j = 0; j < row; j++)
                {
                    hidatoGrid.InputAt(i, j, int.Parse(sa[j]));
                }
            }

            UpdateTextBoxes();
        }

        //public Hidato_Board(OptionData option)
        //{
            
        //}

        private void InitializeArrays(int nCols, int nRows)
        {
            hidatoGrid = new HidatoGrid(nCols, nRows);
            hidato_solver = new HidatoSolver(hidatoGrid);
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

            int xPosSolve = (this.Size.Width - buttonSolve.Size.Width - button1.Size.Width - ColumnSpacing) / 2;
            buttonSolve.Location = new Point(xPosSolve, nCols * RowSpacing + TopMargin);
            button1.Location = new Point(xPosSolve + buttonSolve.Size.Width + ColumnSpacing, nRows * RowSpacing + TopMargin);
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

            if (data == -1)
            {
                hidatoGrid.Disable++;
            }
            else if(hidatoGrid.GetDataAt(nCol, nRow) == -1 && data != -1)
            {
                hidatoGrid.Disable--;
            }

            if (data > hidatoGrid.GridClength * hidatoGrid.GridRlength - hidatoGrid.Disable)
            {
                tb.BackColor = Color.Red;
                tb.ForeColor = Color.Black;

                ///Node의 Input은 풀기 전 초기 값만 들어갸야 함으로 풀이가 진행중인 동안에 TaxtBox안에 Taxt가 바뀌면 Input에 들어가면 안 되기 때문에 
                ///풀이가 진행중인 경우에는 InputAt함수를 호출하지 않습니다.
                if (!hidato_solver.IsProcess)
                {
                    hidatoGrid.InputAt(nCol, nRow, data);
                }
            }
            else if (data == -1)
            {
                tb.BackColor = Color.Black;
                tb.ForeColor = Color.White;

                hidatoGrid.InputAt(nCol, nRow, data);
            }
            else if (data > 0)
            {
                tb.BackColor = Color.LightGreen;
                tb.ForeColor = Color.Black;

                hidatoGrid.InputAt(nCol, nRow, data);
                
            }
            else if (tb.TextLength == 0 || data == 0)
            {
                tb.BackColor = Color.White;
                tb.ForeColor = Color.Black;

                hidatoGrid.InputAt(nCol, nRow, data);
            }
            //hidatoGrid.show();
        }



        private void Hidato_Bord_Load(object sender, EventArgs e)
        {

        }

        public void UpdateTextBoxes()
        {

            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new VoidDelegate(UpdateTextBoxes));
                }
                catch { }
            }
            else
            {
                for (int i = 0; i < hidatoGrid.GridClength; i++)
                {
                    for (int j = 0; j < hidatoGrid.GridRlength; j++)
                    {
                        //해당 칸에 들어있는 숫자가 0이면
                        int data = hidatoGrid.GetDataAt(i, j);
                        if (data == 0)
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
        }

        private void ClearArrays()
        {
            for (int i = 0; i < hidatoGrid.GridClength; i++)
            {
                for (int j = 0; j < hidatoGrid.GridRlength; j++)
                {
                    hidatoGrid.InputAt(i, j, 0);
                }
            }
        }

        
        public bool success = false;
        private void buttonSolve_Click(object sender, EventArgs e)
        {
            //문제를 푸는데 성공하면
            Thread task1 = new Thread(new ThreadStart(() => hidato_solver.startsolve(this)));

            task1.Start();
        }

        /// <summary>
        /// 사용자에게 해당 작업을 성공했는지(t),실패(f) 여부를 매시지 박스로 출력해 줍니다.
        /// 걸린 시간을 표시하기 위해 해당 작업이 시작되기 시작한 시각을 매개 변수로 받습니다.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void ShowSFdialog(bool success, DateTime start)
        {
            UpdateTextBoxes();

            if (success == true)
            {
                //성공했다는 메세지 박스를 출력함C:\Users\석진 이\source\repos\Hidato Solver(Gui)\Hidato Solver(Gui)\Form1.cs

                MessageBox.Show("Completed successfully. Time elapsed: " + (DateTime.Now - start).ToString());
            }
            else
            {
                MessageBox.Show("Failed to solve.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Hidato board files (*.txt)|*.txt|All files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                DateTime dtStart = DateTime.Now;

                Stream stream = dlg.OpenFile();
                StreamWriter sw = new StreamWriter(stream);

                sw.WriteLine(hidatoGrid.GridClength.ToString());
                sw.WriteLine(hidatoGrid.GridRlength.ToString());

                for (int i = 0; i < hidatoGrid.GridClength; i++)
                {
                    string str = null;
                    for (int j = 0; j < hidatoGrid.GridRlength; j++)
                    {
                        str = str + hidatoGrid.GetDataAt(i, j).ToString() + " ";
                    }

                    sw.WriteLine(str);
                }

                sw.Close();
                stream.Close();

                ShowSFdialog(true, dtStart);
            }
        }

        private void Hidato_Board_FormClosing(object sender, FormClosingEventArgs e)
        {
            hidato_solver.cancel();

        }
    }
}


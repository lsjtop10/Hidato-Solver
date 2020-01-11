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
        private HidatoSolverInterface hidato_solver;
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
        private const int ColumnSpacing = 40;
        private const int RowSpacing = 30;
        private const int ColumnWidth = 35;
        private const int RowHeight = 25;
        private const int LeftMargin = 12;
        private const int TopMargin = 12;
        private const int RightMargin = 12;
        private const int BottomMargin = 60;

        public Hidato_Board(int nCols, int nRows)
        {
            InitializeComponent();
            InitializeArrays(nCols, nRows);
            //ClearArrays();
        }

        public Hidato_Board(TextReader sr)
        {
            int col, row;

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

        private void InitializeArrays(int nRows, int nCols)
        {
            hidatoGrid = new HidatoGrid(nRows, nCols);
            displayed = new TextBox[nRows, nCols];
            hidato_solver = new HidatoSolverInterface(hidatoGrid, this);

            for (int i = 0; i < nCols; i++)
            {
                for (int j = 0; j < nRows; j++)
                {
                    displayed[j, i] = new TextBox();
                    displayed[j, i].Parent = this;
                    displayed[j, i].Location = new Point(i * ColumnSpacing + LeftMargin, j * RowSpacing + TopMargin);
                    displayed[j, i].Size = new Size(ColumnWidth, RowHeight);
                    displayed[j, i].Visible = true;
                    displayed[j, i].TextChanged += new EventHandler(HidatoBoard_TextChanged);
                }
            }

            this.Size = new Size(nCols * ColumnSpacing + LeftMargin + RightMargin, nRows * RowSpacing + TopMargin + BottomMargin + button1.Height);

            int xPosSolve = (this.Size.Width - buttonSolve.Size.Width - button1.Size.Width - ColumnSpacing) / 2;
            buttonSolve.Location = new Point(xPosSolve, nRows * RowSpacing + TopMargin);
            button1.Location = new Point(xPosSolve + buttonSolve.Size.Width + ColumnSpacing, nRows * RowSpacing + TopMargin);
        }

        private void HidatoBoard_TextChanged(object sender, EventArgs e)
        {

            TextBox tb = (TextBox)sender;
            string s = tb.Text;
            // determine which row,col we are in

            int nRow = (tb.Location.X - LeftMargin) / ColumnSpacing;
            int nCol = (tb.Location.Y - TopMargin + 2) / RowSpacing;

            int data = 0;
            try
            {
                data = int.Parse(s);
              
            }
            catch (FormatException) {}
            catch (OverflowException) {}

            if (data > hidatoGrid.GridCols * hidatoGrid.GridRows - hidatoGrid.Disable)
            {
                tb.BackColor = Color.Red;
                tb.ForeColor = Color.Black;

                //Node의 Input은 풀기 전 초기 값만 들어갸야 하므로 풀이가 진행중인 동안에 TaxtBox안에 Taxt가 바뀌면 Input에 들어가면 안 되기 때문에 
                //풀이가 진행중인 경우에는 InputAt함수를 호출하지 않습니다.
                if (!Option.IsProcessing)
                {
                    hidatoGrid.InputAt(nRow, nCol, data);
                }
            }
            else if (data == -1)
            {
                tb.BackColor = Color.Black;
                tb.ForeColor = Color.White;

                if (Option.IsProcessing == false)
                {
                    hidatoGrid.InputAt(nRow, nCol, data);
                }

            }
            else if (data > 0)
            {
                tb.BackColor = Color.LightGreen;
                tb.ForeColor = Color.Black;

                if (Option.IsProcessing == false)
                {
                    hidatoGrid.InputAt(nRow, nCol, data);
                }

            }
            else if (tb.TextLength == 0 || data == 0)
            {
                tb.BackColor = Color.White;
                tb.ForeColor = Color.Black;

                if (Option.IsProcessing == false)
                {
                    hidatoGrid.InputAt(nRow,nCol, data);
                }

            }
            //hidatoGrid.show();
        }

        private void Hidato_Bord_Load(object sender, EventArgs e)
        {

        }

        public void UpdateTextBoxes()
        {
#if DEBUG
            hidatoGrid.show();

#endif
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
                for (int i = 0; i < hidatoGrid.GridCols; i++)
                {
                    for (int j = 0; j < hidatoGrid.GridRows; j++)
                    {
                        //해당 칸에 들어있는 숫자가 0이면
                        int data = hidatoGrid.GetDataAt(j, i);
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
            for (int i = 0; i < hidatoGrid.GridCols; i++)
            {
                for (int j = 0; j < hidatoGrid.GridRows; j++)
                {
                    hidatoGrid.InputAt(j, i, 0);
                }
            }
        }

        
        public bool success = false;
        private void buttonSolve_Click(object sender, EventArgs e)
        {
            SetTaxtBoxesIsEditble(false);
            //문제를 푸는데 성공하면
            hidato_solver.Startsolve();

            SetTaxtBoxesIsEditble(true);

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
                //성공했다는 메세지 박스를 출력함 C:\Users\석진 이\source\repos\Hidato Solver(Gui)\Hidato Solver(Gui)\Form1.cs

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

                sw.WriteLine(hidatoGrid.GridCols.ToString());
                sw.WriteLine(hidatoGrid.GridRows.ToString());

                for (int i = 0; i < hidatoGrid.GridCols; i++)
                {
                    string str = null;
                    for (int j = 0; j < hidatoGrid.GridRows; j++)
                    {
                        str = str + hidatoGrid.GetDataAt(j, i).ToString() + " ";
                    }

                    sw.WriteLine(str);
                }

                sw.Close();
                stream.Close();

                ShowSFdialog(true, dtStart);
            }
        }

        /// <summary>
        /// 보드의 텍스트 박스의 수정 가능/불가능 여부를 받아 활성/비활성화함
        /// </summary>
        /// <param name="editble">텍스트 박스들을 수정 가능한 상태로 만들지(True면 수정 가능)</param>
        private void SetTaxtBoxesIsEditble(bool editble)
        {
            for(int i = 0; i < hidatoGrid.GridCols; i++)
            {
                for(int j = 0; j < hidatoGrid.GridRows; j++)
                {
                    displayed[j, i].Enabled = editble;
                }
            }
        }

        private void Hidato_Board_FormClosing(object sender, FormClosingEventArgs e)
        {
            Option.SolveCancel = true;
        }
    }
}


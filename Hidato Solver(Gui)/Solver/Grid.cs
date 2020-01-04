using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hidato_solver
{
    // 누가 수업시간에 처묵처묵 해? -by tan kim
    enum Side { N = 0, NE, E, SE, S, SW, W, NW }
    class HidatoGrid
    {
        private Node head = null;

        private int m_Rows;
        private int m_Cols;

        //값이 -1인 칸의 개수 즉 비활성화 된 칸의 개수
        //private int m_Disable;
        private int m_Disable;

        /// <summary>
        /// 그리드 생성자 주어진 행과 열(이건 레거시)
        /// </summary>
        /// <param name="Cols"></param>
        /// <param name="Rows"></param>
        public HidatoGrid(int Rows, int Cols)
        {
            m_Cols = Cols;
            m_Rows = Rows;
            GenerateGrid(Rows, Cols);
        }

        public int GridRows
        {
            get { return m_Rows; }
        }

        public int GridCols
        {
            get { return m_Cols; }
        }

        public int Disable { get => DisableBoxesCount(); }

        public class Node
        {
            //-1을 unsed라는 이름으로 비활성화된 칸을 나타내는 상수로 정의합니다.
            public const int unused = -1;

            public Node[] Abutter = new Node[8];

            public SurchMarking marker;

            public int Input = 0;
            public int WorkSapce = 0;

            public int Data
            {
                get { return Input + WorkSapce; }
            }

            public bool[] insertMarking = new bool[8];

            public Node()
            {
                for(int i = 0; i < Abutter.Length; i++)
                {
                    Abutter[i] = null;
                }

                for (int i = 0; i < insertMarking.Length; i++)
                {
                    insertMarking[i] = false;
                }

            }


        }

        public void GenerateGrid(int Rows, int Cols)
        {
            int Rcount = 0; int Ccount = 0;

            //머리의 주소를 받는데 머리가 없으면 머리를 생성합니다.
            if (head == null)
            {
                head = new Node();
            }

            Node horse = head;

            //가로 폭 만큼 생성
            while (Rows - 1 > Rcount)
            {
                if (horse.Abutter[(int)Side.E] != null)
                {
                    horse = horse.Abutter[(int)Side.E];
                }
                else
                {
                    Node temp = new Node();
                    horse.Abutter[(int)Side.E] = temp;
                    horse.Abutter[(int)Side.W] = horse;
                    Rcount++;
                    temp = null;
                }
            }

            while (Cols - 1 > Ccount)
            {
                horse = head;

                while (horse.Abutter[(int)Side.S] != null)
                {
                    horse = horse.Abutter[(int)Side.S];
                }

                Rcount = 0;
                while (Rows > Rcount)
                {

                    Node temp1 = new Node();
                    horse.Abutter[(int)Side.S] = temp1;
                    temp1.Abutter[(int)Side.N] = horse;

                    //horse.S.W
                    if (horse.Abutter[(int)Side.S].Abutter[(int)Side.W] == null)
                    {
                        
                        if (horse.Abutter[(int)Side.W] == null)
                        {
                            //s.w
                            horse.Abutter[(int)Side.S].Abutter[(int)Side.W] = null;
                        }
                        else
                        {
                            //s.w , w.s
                            horse.Abutter[(int)Side.S].Abutter[(int)Side.W]= horse.Abutter[(int)Side.W].Abutter[(int)Side.S];
                            horse.Abutter[(int)Side.W].Abutter[(int)Side.S].Abutter[(int)Side.E] = horse.Abutter[(int)Side.S];
                        }
                    }

                    if (horse.Abutter[(int)Side.S].Abutter[(int)Side.NW] == null)
                    {
                        if (horse.Abutter[(int)Side.W] == null)
                        {
                            horse.Abutter[(int)Side.S].Abutter[(int)Side.NW] = null;
                        }
                        else
                        {
                            horse.Abutter[(int)Side.S].Abutter[(int)Side.NW] = horse.Abutter[(int)Side.W];
                            horse.Abutter[(int)Side.W].Abutter[(int)Side.SE] = horse.Abutter[(int)Side.S];
                        }
                    }

                    if (horse.Abutter[(int)Side.S].Abutter[(int)Side.NE] == null)
                    {
                        if (horse.Abutter[(int)Side.E] == null)
                        {
                            horse.Abutter[(int)Side.S].Abutter[(int)Side.NE] = null;
                        }
                        else
                        {
                            horse.Abutter[(int)Side.S].Abutter[(int)Side.NE] = horse.Abutter[(int)Side.E];
                            horse.Abutter[(int)Side.E].Abutter[(int)Side.SW]= horse.Abutter[(int)Side.S];
                        }
                    }
                    horse = horse.Abutter[(int)Side.E];
                    Rcount++;
                    temp1 = null;

                }

                Ccount++;

            }
        }

        /// <summary>
        /// 주어진 열과 행의 노드를 반환
        /// </summary>
        /// <param name="Row">열</param>
        /// <param name="Col">행</param>
        /// <returns></returns>
        public Node GetNodeAt(int Row, int Col)
        {
            int count = 0;
            Node horse = head;

            while (Col != count)
            {
                horse = horse.Abutter[(int)Side.S];
                count++;
            }

            count = 0;
            while (Row != count)
            {
                horse = horse.Abutter[(int)Side.E];
                count++;
            }

            return horse;
        }

        public void InputAt(int Row, int Col, int data)
        {
            HidatoGrid.Node taget = GetNodeAt(Row, Col);
            taget.Input = data;
        }

        public int GetDataAt(int Row, int Col)
        {
            Node taget = GetNodeAt(Row, Col);
            int TagetData = taget.Input + taget.WorkSapce;
            return TagetData;
        }

        /// <summary>
        /// 모든 노드를 1차원 리스트로 묶어 반환
        /// </summary>
        /// <returns></returns>

        public int DisableBoxesCount()
        {
            m_Disable = 0;
            int disableCount = 0;
            for (int i = 0; i < GridCols; i++)
            {
                for (int j = 0; j < GridRows; j++)
                {
                    if (GetDataAt(j, i) == -1)
                    {
                        disableCount++;
                    }
                }
            }

            m_Disable = disableCount;
            return disableCount;
        }

        public void show()
        {
            for (int i = 0; i < GridCols; i++)
            {
                for (int j = 0; j < GridRows; j++)
                {
                    Console.Write(GetDataAt(j, i));
                    Console.Write(" ");

                }
                Console.WriteLine("");
            }

        }
    }


}
    


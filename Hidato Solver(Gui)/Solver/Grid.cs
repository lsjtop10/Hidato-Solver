using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hidato_solver
{
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

        public int HintCount { get => m_HintCount(); }
        public class Node
        {
            //-1을 unsed라는 이름으로 비활성화된 칸을 나타내는 상수로 정의합니다.
            public const int unused = -1;

            public Node[] Neighbor = new Node[8];

            public Node N { get { return this.Neighbor[(int)Side.N]; } set { this.Neighbor[(int)Side.N] = value; } }
            public Node NE { get { return this.Neighbor[(int)Side.NE]; } set { this.Neighbor[(int)Side.NE] = value; } }
            public Node E { get { return this.Neighbor[(int)Side.E]; } set { this.Neighbor[(int)Side.E] = value; } }
            public Node SE { get { return this.Neighbor[(int)Side.SE]; } set { this.Neighbor[(int)Side.SE] = value; } }
            public Node S { get { return this.Neighbor[(int)Side.S]; } set { this.Neighbor[(int)Side.S] = value; } }
            public Node SW { get { return this.Neighbor[(int)Side.SW]; } set { this.Neighbor[(int)Side.SW] = value; } }
            public Node W { get { return this.Neighbor[(int)Side.W]; } set { this.Neighbor[(int)Side.W] = value; } }
            public Node NW{ get { return this.Neighbor[(int)Side.NW]; } set { this.Neighbor[(int)Side.NW] = value; } }

            public SurchMarking marker;

            public int Input = 0;
            public int WorkSapce = 0;

            public int Data
            {
                get { return Input + WorkSapce; }
            }

            public Node()
            {
                this.NE = null;
                this.E = null;
                this.SE = null;
                this.S = null;
                this.SE = null;
                this.W = null;
                this.NW = null;
            }

            /// <summary>
            /// 이웃한 노드의 참조를 1차원 배열로 반환
            /// </summary>
            /// <returns></returns>
            public Node[] GetNeighborNods()
            {
                Node[] nodes = new Node[8];

                nodes[(int)Side.N] = this.N;
                nodes[(int)Side.NE] = this.NE;
                nodes[(int)Side.E] = this.E;
                nodes[(int)Side.SE] = this.SE;
                nodes[(int)Side.S] = this.S;
                nodes[(int)Side.SW] = this.SW;
                nodes[(int)Side.W] = this.W;
                nodes[(int)Side.NW] = this.NW;

                return nodes;
            }
        }

        public void GenerateGrid(int CLength, int RLength)
        {
            int Rcount = 0; int Ccount = 0;

            //머리의 주소를 받는데 머리가 없으면 머리를 생성합니다.
            if (head == null)
            {
                head = new Node();
            }

            Node horse = head;

            //가로 폭 만큼 생성
            while (RLength - 1 > Rcount)
            {
                if (horse.E != null)
                {
                    horse = horse.E;
                }
                else
                {
                    Node temp = new Node();
                    horse.E = temp;
                    temp.W = horse;
                    Rcount++;
                    temp = null;
                }
            }

            while (CLength - 1 > Ccount)
            {
                horse = head;

                while (horse.S != null)
                {
                    horse = horse.S;
                }

                Rcount = 0;
                while (RLength > Rcount)
                {

                    Node temp1 = new Node();
                    horse.S = temp1;
                    temp1.N = horse;

                    if (horse.S.W == null)
                    {
                        if (horse.W == null)
                        {
                            horse.S.W = null;
                        }
                        else
                        {
                            horse.S.W = horse.W.S;
                            horse.W.S.E = horse.S;
                        }
                    }

                    if (horse.S.NW == null)
                    {
                        if (horse.W == null)
                        {
                            horse.S.NW = null;
                        }
                        else
                        {
                            horse.S.NW = horse.W;
                            horse.W.SE = horse.S;
                        }
                    }

                    if (horse.S.NE == null)
                    {
                        if (horse.E == null)
                        {
                            horse.S.NE = null;
                        }
                        else
                        {
                            horse.S.NE = horse.E;
                            horse.E.SW = horse.S;
                        }
                    }
                    horse = horse.E;
                    Rcount++;
                    temp1 = null;

                }

                Ccount++;

            }
        }

        public Node GetNodeAt(int Row, int Col)
        {
            int count = 0;
            Node horse = head;

            while (Col!= count)
            {
                horse = horse.S;
                count++;
            }

            count = 0;
            while (Row != count)
            {
                horse = horse.E;
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
        private int DisableBoxesCount()
        {
            m_Disable = 0;
            int disableCount = 0;
            Node[] nodes = SerializeGrid();
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].Data == -1)
                {
                    disableCount++;
                }

            }

            m_Disable = disableCount;
            return disableCount;
        }

        public Node[] SerializeGrid()
        {
            Node[] nodesArr= new Node[GridCols * GridRows];

            int indexCount = 0;

            //가로 방향으로 운동
            Node RowHorse = head; 
            //세로 방향으로 운동
            Node ColHorse = head;

            for (int i = 0; i < GridCols; i++)
            {

                for (int j = 0; j < GridRows; j++)
                {
                    nodesArr[indexCount] = RowHorse;
                    indexCount++;
                    RowHorse = RowHorse.E;
                }

                ColHorse = ColHorse.S;
                RowHorse = ColHorse;
            }

            return nodesArr;
        }
        private int m_HintCount()
        {
            int hintcount = 0;
            Node[] nodes = SerializeGrid();
            for(int i = 0; i < nodes.Length; i++)
            {
                if(nodes[i].Input > 0)
                {
                    hintcount++;
                }
            }
            return hintcount;
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
    


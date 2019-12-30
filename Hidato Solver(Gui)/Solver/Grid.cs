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
        private Node hard = null;

        private int m_Rlength;
        private int m_Clength;

        //값이 -1인 칸의 개수 즉 비활성화 된 칸의 개수
        //private int m_Disable;
        public int Disable;

        public HidatoGrid(int Cols, int Rows)
        {
            m_Clength = Cols;
            m_Rlength = Rows;
            GenerateGrid(Cols, Rows);
        }

        public int GridRlength
        {
            get { return m_Rlength; }
        }

        public int GridClength
        {
            get { return m_Clength; }
        }

        public class Node
        {
            //-1을 unsed라는 이름으로 비활성화된 칸을 나타내는 상수로 정의합니다.
            public const int unused = -1;
            public Node N;
            public Node NE;
            public Node E;
            public Node SE;
            public Node S;
            public Node SW;
            public Node W;
            public Node NW;

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
                this.NE = null;
                this.E = null;
                this.SE = null;
                this.S = null;
                this.SE = null;
                this.W = null;
                this.NW = null;

                for (int i = 0; i < insertMarking.Length; i++)
                {
                    insertMarking[i] = false;
                }

            }

            public void EmptyMarker()
            {
                for (int i = 0; i < this.marker.SurchMarker.Length; i++)
                {
                    this.marker.SurchMarker[i] = false;
                }
            }

        }

        public void GenerateGrid(int CLength, int RLength)
        {
            int Rcount = 0; int Ccount = 0;

            //머리의 주소를 받는데 머리가 없으면 머리를 생성합니다.
            if (hard == null)
            {
                hard = new Node();
            }

            Node horse = hard;

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
                horse = hard;

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
            Node horse = hard;

            while (Col != count)
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

        public void InputAt(int Yindex, int Xindex, int data)
        {
            HidatoGrid.Node taget = GetNodeAt(Yindex, Xindex);
            taget.Input = data;
        }

        public int GetDataAt(int Yindex, int Xindex)
        {
            Node taget = GetNodeAt(Yindex, Xindex);
            int TagetData = taget.Input + taget.WorkSapce;
            return TagetData;
        }

        public int DisableBoxesCount()
        {
            Disable = 0;
            int disableCount = 0;
            for (int i = 0; i < GridClength; i++)
            {
                for (int j = 0; j < GridRlength; j++)
                {
                    if (GetDataAt(i, j) == 0)
                    {
                        disableCount++;
                    }
                }
            }

            Disable = disableCount;
            return disableCount;
        }

    }


}
    


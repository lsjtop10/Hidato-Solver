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
        /// <summary>
        /// 그리드는 Row Col 순으로 한다.
        /// </summary>
        private Node[,] Grid;
        
        private int m_Rows;
        private int m_Cols;

        ///로 되어 있는 칸의 갯수
        public int m_Disable;
        public int Disable;

        public HidatoGrid(int Rows , int Cols)
        {
            m_Rows = Rows;
            m_Cols = Cols;
            Grid = new Node[Rows, Cols];
        }

        public class Node { public int Data;}
        public int GridRows
        {
            get { return m_Rows; }
        }

        public int GridCols
        {
            get { return m_Cols; }
        }


        public int HintCount()
        {
            int HintCount = 0;
            for (int i = 0; i < GridCols; i++)
            {
                for (int j = 0; j < GridRows; j++)
                {
                    if (GetDataAt(j, i) > 0)
                    {
                        HintCount++;
                    }
                }
            }

            return HintCount;
        }

        public int GetDataAt(int Row, int Col)
        {
            return Grid[Row, Col].Data;

        }
        public int GetDataAt(Coordinate coordinate)
        {
            return Grid[coordinate.Row, coordinate.Col].Data;

        }

        public Node GetNodeAt(int a, int b)
        {
            return new Node();
        }

        public void InputAt(int Row, int Col, int data)
        {
            Grid[Row, Col].Data = data;
        }

        

        public int DisableBoxesCount()
        {
            Disable = 0;
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

            Disable = disableCount;
            return disableCount;
        }



    }

    class Coordinate
    {
        public int Row, Col;

        public Coordinate(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public Coordinate()
        {
            Row = 0;
            Col = 0;
        }

        public Coordinate GetSideDelta(Side side)
        {
            //현재 좌표를 임시로 저장
            Coordinate tmp = new Coordinate(Row, Col);
            if (side == Side.N)
            {
                tmp.Row += 0;
                tmp.Col += 1;
            }
            else if (side == Side.NE)
            {
                tmp.Row += 1;
                tmp.Col += 1;

            }
            else if (side == Side.E)
            {
                tmp.Row += 1;
                tmp.Col += 0;
            }
            else if (side == Side.SE)
            {
                tmp.Row += 1;
                tmp.Col += -1;

            }
            else if (side == Side.S)
            {
                tmp.Row += 0;
                tmp.Col += -1;
            }
            else if (side == Side.SW)
            {
                tmp.Row += -1;
                tmp.Col += -1;
            }
            else if (side == Side.W)
            {
                tmp.Row += -1;
                tmp.Col += 0;
            }
            else if (side == Side.NW)
            {
                tmp.Row += -1;
                tmp.Col += 1;
            }

            // 렬, 행 둘 중에 하나라도 음수가 있으면 칸을 벗어났으므로 null 반환
            if (tmp.Row < 0 || tmp.Col < 0) 
            { 
                return null; 
            }
            else
            { 
                return tmp; 
            }
        }

    }
    
}

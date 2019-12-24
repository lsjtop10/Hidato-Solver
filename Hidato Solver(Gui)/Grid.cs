using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hidato_solver
{
    class HidatoGrid
    {
        int[,] Grid;
        private Node hard = null;
        private int m_Rlength;
        private int m_Clength;

        //-1로 되어 있는 칸의 개수
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
            

        }

        public int HintCount()
        {
            int HintCount = 0;
            for (int i = 0; i < GridClength; i++)
            {
                for (int j = 0; j < GridRlength; j++)
                {
                    if (GetNodeAt(i, j).data > 0)
                    {
                        HintCount++;
                    }
                }
            }

            return HintCount;
        }


        public void GenerateGrid(int CLength, int RLength)
        {
            Grid = new int[CLength, RLength];
            m_Clength = CLength;
            m_Rlength = RLength;
        }

        public int GetDataAt(int Yindex, int Xindex)
        {
            return Grid[Xindex, Yindex];
        }

        public int GetDataAt(Coordinate coordinate)
        {
            return Grid[coordinate.X, coordinate.Y];
        }

        public void InputAt(int Yindex, int Xindex, int data)
        {
            Grid[Yindex, Xindex] = data;
        }

        public void InputAt(Coordinate coordinate, int data)
        {
            Grid[coordinate.X, coordinate.Y] = data;

        }

        public int DisableBoxesCount()
        {
            Disable = 0;
            int disableCount = 0;
            for (int i = 0; i < GridClength; i++)
            {
                for (int j = 0; j < GridRlength; j++)
                {
                    if (GetDataAt(i, j) == -1)
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
        public int X; public int Y; 

        public Coordinate GetSideCoordinate(Side side, Coordinate current)
        {
            if (side == Side.N)
            {
                current.X += 0;
                current.Y += 1;
            }
            else if (side == Side.NE)
            {
                current.X += 1;
                current.Y += 1;
            }
            else if (side == Side.E)
            {
                current.X += 1;
                current.Y += 0;
            }
            else if (side == Side.SW)
            {
                current.X += 1;
                current.Y += -1;
            }
            else if (side == Side.S)
            {
                current.X += 0;
                current.Y += -1;
            }
            else if (side == Side.SW)
            {
                current.X += -1;
                current.Y += -1;
            }
            else if (side == Side.W)
            {
                current.X += -1;
                current.Y += 0;
            }
            else if (side == Side.NW)
            {
                current.X += -1;
                current.Y += 1;
            }

            return current;
        }
    }
}

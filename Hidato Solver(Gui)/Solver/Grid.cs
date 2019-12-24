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


        private int m_verticalLn; 
        private int m_horizontalLn;


        //-1로 되어 있는 칸의 개수
        //private int m_Disable;
        public int Disable;

        public HidatoGrid(int HorizontalLn, int VerticalLn) 
        {
            Grid = new int[HorizontalLn, VerticalLn];
            m_horizontalLn = HorizontalLn;
            m_verticalLn = VerticalLn;
        }

        /// <summary>
        /// 세로 길이 
        /// </summary>
        public int GridVerticalLn
        {
            get { return m_verticalLn; }

        }

        /// <summary>
        /// 가로 길이
        /// </summary>
        public int GridHorizontalLn
        {
            get { return m_horizontalLn; }
        }

        public int HintCount()
        {
            int HintCount = 0;
            for (int i = 0; i < GridHorizontalLn; i++)
            {
                for (int j = 0; j < GridVerticalLn; j++)
                {
                    if (GetDataAt(i, j) > 0)
                    {
                        HintCount++;
                    }
                }
            }

            return HintCount;
        }

        public int GetDataAt(int X, int Y)
        {
            return Grid[X, Y];
        }

        public int GetDataAt(Coordinate coordinate)
        {
            return Grid[coordinate.X, coordinate.Y];
        }

        public void InputAt(int X, int Y, int data)
        {
            Grid[X, Y] = data;
        }

        public void InputAt(Coordinate coordinate, int data)
        {
            Grid[coordinate.X, coordinate.Y] = data;

        }

        public int DisableBoxesCount()
        {
            Disable = 0;
            int disableCount = 0;
            for (int i = 0; i < GridHorizontalLn; i++)
            {
                for (int j = 0; j < GridVerticalLn; j++)
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

        public void Show()
        {
            int data = 0;
            for(int i = 0; i <  GridVerticalLn; i++)
            {
                for(int j = 0; j < m_horizontalLn; j++)
                {
                    data = Grid[j ,i];
                    Console.Write(data);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
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

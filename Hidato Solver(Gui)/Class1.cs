using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hidato_solver
{
    class HidatoGrid
    {
        private Node hard = null;

        private int m_Rlength;
        private int m_Clength;

        //-1로 되어 있는 칸의 갯수
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
           
        }

        public int GetDataAt(int Yindex, int Xindex)
        {

        }

        public void InputAt(int Yindex, int Xindex, int data)
        {
        }

        public int GetDataAt(int Yindex, int Xindex)
        {
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
}

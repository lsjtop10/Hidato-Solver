using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hidato_Solver_Gui_;
using System.Threading;
using static System.Math;


namespace hidato_solver
{

    //이 열거형 변수는 인접해 있는 칸을 8방위로 표현합니다. 
    //즉 위쪽에 있는 면을 N이라고 부르고(북쪽) 위쪽과 오른쪽의 사이에 있는 면을
    //NE(북동)쪽 이라 표현할 것입니다.
    public enum Side { N = 0, NE, E, SE, S, SW, W, NW }
    //이 클래스는 어떠한 방향에 대하여 다음 숫자가 들어갈수 있는지 없는지 표시합니다.
    //true면 다음 숫자가 들어갈수 있다는 것을 의미하고, false면 다음 숫자가 들어가지 못한다는 것을 의미합니다.
    class HidatoSolver
    {
        HidatoGrid m_hidatoGrid;
        static public bool IsProcess;
        static public bool ShowAllProcess;
        static public int NextUpdateSoconds;
        static public int ProcessWaitTime;

        public void cancel()
        {
            
        }

        public bool Startsolve(HidatoGrid grid)
        {
            return true;
        }

        public HidatoSolver(HidatoGrid hidatoGrid)
        {
            
        }
    }
}
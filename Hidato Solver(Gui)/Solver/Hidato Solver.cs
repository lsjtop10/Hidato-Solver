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
    //이 클래스는 어떠한 방향에 대하여 다음 숫자가 들어갈수 있는지 없는지 표시합니다.
    //true면 다음 숫자가 들어갈수 있다는 것을 의미하고, false면 다음 숫자가 들어가지 못한다는 것을 의미합니다.

     /// <summary>
     /// 가능한 경우의 수 저장
     /// </summary>
    class SurchMarking
    {

    }
   
    /// <summary>
    /// DFS탐색에서 풀이 이력 저장
    /// </summary>
    class History
    {
     
    }

    /// <summary>
    /// 풀이에 필요한 일반적 메서드와 값u
    /// </summary>
    class Solver
    {
        HidatoGrid hidatoGrid;
        Coordinate currentPtr;
        History history;
    }
    
    interface ISolve
    {
        bool Solve();
    }
        
    class DFSSolver : Solver, ISolve
    {
        /// <summary>
        /// 재귀 탐색
        /// </summary>
        public bool Solve()
        {
            return false;
        }
    }

    /// <summary>
    /// 신 알고리즘 적용 풀이 탐색
    /// </summary>
    class FastSolver: Solver
    {

    }

    /// <summary>
    /// 풀이 로직과 외부와의 소통을 담당
    /// </summary>
    class HidatoSolver
    {
        DFSSolver dfs;
        FastSolver fest;
        public HidatoSolver(HidatoGrid hidato)
        {

        }
    }
    
    /// <summary>
    /// 외부에서 접근할 수 있는 설정 
    /// </summary>
    public static class Option
    {
        public static int NextUpdateSoconds;
        public static int ProcessWaitTime;
        public static bool ShowAllProcess;

    }
}
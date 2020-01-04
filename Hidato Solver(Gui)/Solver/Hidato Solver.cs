using Hidato_Solver_Gui_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Math;


namespace hidato_solver
{
    //이 열거형 변수는 인접한 칸을 8방위로 표현합니다. 
    //즉 위쪽에 있는 면을 N이라고 부르고(북쪽) 위쪽과 오른쪽의 사이에 있는 면을
    //NE(북동)쪽 이라 표현할 것입니다.
    //이 클래스는 어떠한 방향에 대하여 다음 숫자가 들어갈수 있는지 없는지 표시합니다.
    //true면 다음 숫자가 들어갈수 있다는 것을 의미하고, false면 다음 숫자가 들어가지 못한다는 것을 의미합니다.

    /// <summary>
    /// 가능한 경우의 수 저장
    /// </summary>
    class SurchMarking
    {
        public SurchMarking()
        {
            for (int i = 0; i < SurchMarker.Length; i++)
            {
                SurchMarker[i] = false;
            }
        }

        public bool[] SurchMarker = new bool[8];
        public void EmptyMarker()
        {
            for (int i = 0; i < SurchMarker.Length; i++)
            {
                SurchMarker[i] = false;
            }
        }

    }
        
    
    /// <summary>
    /// 풀이에 필요한 일반적 메서드와 값
    /// </summary>


    class LegacyDFSSolver
    {
        HidatoGrid m_hidatoGrid;
        HidatoGrid.Node Current;
        HidatoGrid.Node Target;

        int maxVal;
        DateTime DTNextUpdate;
        /// <summary>
        /// 풀이 이력 저장
        /// </summary>
        Stack<HidatoGrid.Node> history = new Stack<HidatoGrid.Node>();

        /// <summary>
        /// 생성자로 풀이 전 초기화
        /// </summary>
        public LegacyDFSSolver(HidatoGrid grid)
        {
            //전해받은 grid를 현재 클래스의 grid로 
            m_hidatoGrid = grid;

            //첫 번째 노드를 Currrent노드로
            Current = FindFirstNode();

            //다음 업데이트 시간을 현재 시간 + 설정된 다음 업데이트 시간으로 설정
            DTNextUpdate = DateTime.Now.AddSeconds(Option.NextUpdateSoconds);
        }
        
        public HidatoGrid.Node FindFirstNode()
        {
            HidatoGrid.Node tmp;
            for(int i = 0; i < m_hidatoGrid.GridCols; i++)
            {
                for(int j = 0; j < m_hidatoGrid.GridRows; j++)
                {
                    tmp = m_hidatoGrid.GetNodeAt(j, i);
                    if(tmp.Data == 1)
                    {
                        return tmp;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 8방위 중 어떤 방향에 다음 수가 와야 할지 SurchMarking에 마킹해서 반환(가능한 경우: true로 마킹)  
        /// </summary>
        /// <param name="Current"></param>
        /// <returns></returns>
        private SurchMarking FindOption(HidatoGrid.Node Current)
        {
            SurchMarking option = new SurchMarking();

            if(this.Current.N != null && this.Current.N.Data == 0)
            {
                option.SurchMarker[(int)Side.N] = true;
            }
            
            if(this.Current.NE != null && this.Current.NE.Data == 0)
            {
                option.SurchMarker[(int)Side.NE] = true;
            }

            if(this.Current.E != null && this.Current.E.Data == 0)
            {
                option.SurchMarker[(int)Side.E] = true;
            }

            if(this.Current.SE != null && this.Current.SE.Data == 0)
            {
                option.SurchMarker[(int)Side.SE] = true;
            }

            if(this.Current.S != null && this.Current.S.Data == 0)
            {
                option.SurchMarker[(int)Side.S] = true;
            }

            if(this.Current.SW != null && this.Current.SW.Data == 0)
            {
                option.SurchMarker[(int)Side.SW] = true;
            }

            if(this.Current.W != null && this.Current.W.Data == 0)
            {
                option.SurchMarker[(int)Side.W] = true;
            }

            if(this.Current.NW != null && this.Current.NW.Data == 0)
            {
                option.SurchMarker[(int)Side.NW] = true;
            }

            return option;
        }
        
        /// <summary>
        /// 재귀 탐색
        /// </summary>
        public bool Solve()
        {

            //유효성 검사(과연 이 노드가 여기 들어가는 게 맞을까)

            //탐색

            //삽입

            return false;
        }
    }

    /// <summary>
    /// 새로운 알고리즘 적용 풀이 탐색
    /// </summary>
    class FastSolver
    {
        public bool Solve()
        {
            return false;
        }
    }

    /// <summary>
    /// 풀이 로직과 외부와의 소통을 담당
    /// </summary>
    class HidatoSolverInterface
    {
        /// <summary>
        /// 히다토 보드를 업데이트 하기 위해 참조 값을 가져옴
        /// </summary>
        private static Hidato_Board ref_hidatoBoard;
        HidatoGrid m_hidatoGrid;

        LegacyDFSSolver dfsSolver;
        FastSolver festSolver;
        
        public HidatoSolverInterface(HidatoGrid grid, Hidato_Board board)
        {
            m_hidatoGrid = grid;
            ref_hidatoBoard = board;
        }

        public bool Startsolve()
        {
            dfsSolver = new LegacyDFSSolver(m_hidatoGrid);
            dfsSolver.Solve();

            return false;
        }

        private static void Update()
        {
            ref_hidatoBoard.UpdateTextBoxes();
        }

    }

    /// <summary>
    /// 외부에서 접근할 수 있는 설정 및 상태 정보
    /// </summary>
    public static class Option
    {

        /// <summary>
        /// 몇 초에 한 번 업데이트 하나?
        /// </summary>
        public static int NextUpdateSoconds;

        /// <summary>
        /// 다음 처리까지 얼마나 기다릴까?
        /// </summary>
        public static int ProcessWaitTime;

        /// <summary>
        /// 모든 처리 과정을 보여주는가?
        /// </summary>
        public static bool ShowAllProcess;

        /// <summary>
        /// 아직 풀이 진행중인가?
        /// </summary>
        public static bool IsProcessing;

        /// <summary>
        /// (Dfs 탐색 시 사용) 스마트 서치를 사용하나?
        /// </summary>
        public static bool SmartSuch;

        /// <summary>
        /// 풀이 취소 플레그(true면 종료)
        /// </summary>
        public static bool SolveCancel = false;
    }
}
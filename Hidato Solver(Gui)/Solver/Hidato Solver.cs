﻿using Hidato_Solver_Gui_;
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

        /// <summary>
        /// SurchMarker 배열에 대한 인덱서
        /// 외부에서 클레스 내부의 SurchMarker 배열에 직접 접근하지 못하도록 하는 역할 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool this[Side index]
        {
            get
            {
                return SurchMarker[(int)index];
            }
            set
            {
                SurchMarker[(int)index] = value;
            }
        }
        public bool this[int index]
        {
            get
            {
                if (index < 0 || index > SurchMarker.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    return SurchMarker[index];
                }
            }
            set
            {
                SurchMarker[index] = value;
            }
        }

        public int Length
        {
            get
            {
                return SurchMarker.Length;
            }
        }

        private bool[] SurchMarker = new bool[8];

        public void EmptyMarker()
        {
            for (int i = 0; i < SurchMarker.Length; i++)
            {
                SurchMarker[i] = false;
            }
        }

        public int GetTrueCount()
        {
            int count = 0;
            for (int i = 0; i < SurchMarker.Length; i++)
            { 
                if(SurchMarker[i] == true)
                {
                    count++;
                }
            }
            return count;
        }
    }
        
    
    /// <summary>
    /// 풀이에 필요한 일반적 메서드와 값
    /// </summary>


    class LegacyDFSSolver
    {
        HidatoGrid m_hidatoGrid;
        
        /// <summary>
        /// 현재 풀이 진행 중인 노드 여기서 다음 수가 들어갈 노드 탐색 
        /// </summary>
        HidatoGrid.Node Current;
        
        /// <summary>
        /// Current보다 큰 수 중에 가장 작은 수를 가지고 있는 노드  
        /// </summary>
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

            //현재 노드의 북쪽부터 북동쪽부터 순회하며
            for (Side curSide = Side.N; curSide < Side.NW; curSide++)
            {
                if (Current.Abutter[(int)curSide] != null && Current.Abutter[(int)curSide].Data == 0)
                {
                    option[curSide] = true;
                }
            }

            return option;
        }

        /// <summary>
        /// Current가 Target에 위치상으로 인접해 있는지 확인
        /// </summary>
        /// <param name="Currnet"></param>
        /// <param name="Target"></param>
        /// <returns></returns>
        private bool AbutsOnTarget(HidatoGrid.Node Currnet, HidatoGrid.Node Target)
        {
            bool tmp = false;
            return tmp;


        }

        /// <summary>
        /// 재귀 탐색
        /// </summary>
        public bool Solve()
        {
            SurchMarking currentOption;

        //유효성 검사(과연 이 노드가 여기 들어가는 게 맞을까)
            

            //현재 노드가 타겟 노드보다 1 작으면서 타겟 노드와 인접해 있지 않으면 
            
         //탐색
            currentOption = FindOption(Current);


            //다음 수가 들어갈 면이 없으면
            if(currentOption.GetTrueCount() == 0)
            {
                //되돌아감
                return false;
            }

        //삽입
            for (int i = 0; i < currentOption.Length; i++)
            {
                if(currentOption[Side.N] == true)
                {

                }
            }

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
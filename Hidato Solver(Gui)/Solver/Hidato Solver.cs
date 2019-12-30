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

    class SurchMarking
    {
        public bool[] SurchMarker = new bool[8];

        public SurchMarking()
        {
            //생성자를 이용해서 초기화 합니다. 기본값은 false입니다.
            for (int i = 0; i < SurchMarker.Length; i++)
            {
                this.SurchMarker[i] = false;
            }
        }

        public int sizeOfNumSet()
        {
            int temp = 0;
            for (int i = 0; i < SurchMarker.Length; i++)
            {
                if (this.SurchMarker[i] == true)
                {
                    temp++;
                }
            }

            return temp;
        }
    }

    class History
    {
        private List<HidatoGrid.Node> m_history = new List<HidatoGrid.Node>();

        public void AddNode(Coordinate node, HidatoGrid Data)
        {
            this.m_history.Add(node);
            m_history.Sort(delegate (Coordinate c1, Coordinate c2) { return Data.GetDataAt(c1).CompareTo(Data.GetDataAt(c2)); });
        }

        public Coordinate CurrentNode
        {
            get {
                    if (m_history.Count == 0)
                    {
                        return m_history[0];
                    }
                    else
                    {
                        return m_history[m_history.Count - 1];
                    }
                }
        }


        public Coordinate GetPrevNode(Coordinate current , HidatoGrid Data)
        {
            int cost = Data.GetDataAt(current);
            Coordinate MinCostNode = null;
            for(int i = 0; i < m_history.Count; i++)
            {
                int CurrentCost;
                Coordinate currentNode = m_history[i];

                CurrentCost = Data.GetDataAt(current) - Data.GetDataAt(currentNode);

                if(CurrentCost < cost)
                {
                    cost = CurrentCost;
                    MinCostNode = CurrentNode;
                }

                if(CurrentCost <= 0)
                {
                    break;
                }

            }

            m_history.Remove(MinCostNode);
            return MinCostNode;
        }
    }


    class HidatoSolver
    {
        private int hidatoCount = 1;
        private HidatoGrid m_hidatoGrid;
        private Coordinate current;
        private int maxval;
        private Coordinate TargetNode;
        private Coordinate FirstNode;
        private DateTime DTNextUpdate;
        private Hidato_Board RefBoard;
        private bool SolveCancel = false;
        private bool SmartSuch = false;
        private bool m_isProcess = false;

        /// <summary>
        /// 히다토 보드의 모든 노드를 오름차순으로 정렬
        /// </summary>
        private HidatoGrid.Node[] NodeOfHidatoGridArray;
        /// <summary>
        /// 비어있는 칸 카운트
        /// </summary>
        private int EmptyNodeCount;

#if DEBUG
        ulong HoWManyCallSolveFunc = 0;
        List<int> history = new List<int>();
#endif

        /// <summary>
        /// 실시간으로 업데이트 하는지 안 하는지를 지정하는 변수입니다.
        /// false면 정해진 시간마다 업데이트 하고, true면 solve함수에서 변경된 내용을 바로 반영합니다.
        /// </summary>
        ///  static public bool ShowAllProssace = false;
        static public bool ShowAllProcess = false;

        /// <summary>
        /// 몆초 간격으로 업데이트 하는지 지정하는 속성입니다.
        /// </summary>
        static public int NextUpdateSoconds = 5;

        /// <summary>
        /// 푸는 중에 일시 정지 얼마나 길게 일시 정지할지 확인합니다 단위는 ms입니다.
        /// </summary>
        static public int ProcessWaitTime = 0;

        /// <summary>
        /// 진행률에 따라 가변적으로 백트랙킹 여부를 나타내는 속성입니다.
        /// -1은 항상 비활성화 0은 자동 1은 항상 활성화를 나타냅니다.
        /// </summary>
        static public int EnalbleSmartSuchTracking = -1;

        public HidatoSolver(HidatoGrid refHidatoGrid)
        {
            m_hidatoGrid = refHidatoGrid;
        }

        public bool IsProcess
        {
            get { return m_isProcess; }
        }

        #region 풀이에 필요한 각종 함수입니다.
        private Coordinate FindFirstNode(HidatoGrid Data)
        {
            Coordinate temp = new Coordinate(0,0);
            for (int i = 0; i < m_hidatoGrid.GridCols; i++)
                for (int j = 0; j < m_hidatoGrid.GridRows; j++)
                {
                    
                    if (Data.GetDataAt(j,i) == 1)
                    {
                        return temp;
                    }
                }
            return null;
        }

        private HidatoGrid.Node FindNextTargetNode(HidatoGrid.Node current)
        {
            //다음노드를 일단 작은 노드로 설정
            HidatoGrid.Node TargetNode = FindFirstNode();
            //현재노드
            HidatoGrid.Node nNode;

            //최대값 지역변수를 선언한 다음 미리 구해놓은 맴버 변수(maxval변수)에서 값을 가져옴 
            int maxval = this.maxval;

            for (int i = 0; i < m_hidatoGrid.GridCols; i++)
            {
                for (int j = 0; j < m_hidatoGrid.GridRows; j++)
                {
                    //현재 위치의 노드;
                    nNode = m_hidatoGrid.GetNodeAt(j, i);

                    //현재 위치의 노드의 값이 매개변수로 넘어온 노드보다 크면서
                    //최대값보다 작으면

                    if (nNode.data > current.data && nNode.data <= maxval)
                    {
                        maxval = nNode.data;
                        TargetNode = nNode;
                    }

                }
            }
            return TargetNode;

        }

        private int FindMaxVal()
        {
            //최대
            int maxval = 0;
            //현재값
            int nval = 0;

            for (int i = 0; i < m_hidatoGrid.GridCols; i++)
                for (int j = 0; j < m_hidatoGrid.GridRows; j++)
                {
                    //현재 위치의 값
                    nval = m_hidatoGrid.GetDataAt(j, i);

                    //만약 최댓값이 현재 값보다 작으면
                    if (nval > maxval)
                    {
                        //최대값은 현재 값
                        maxval = nval;
                    }
                }

            return maxval;

        }

        private SurchMarking FindOption(Coordinate current, HidatoGrid Data)
        {
            SurchMarking marker = new SurchMarking();
            //현재 노드의 윗부분(북쪽)이 쓰지 않는 부분이거나, 윗부분이 없거나, 뭐가 차있으면
            if (current.N == null)
            {
                marker.SurchMarker[(int)side.N] = false;
            }
            else if (current.N.data == HidatoGrid.Node.unused || current.N.data > 0)
            {
                //사용 할 수 없다는 표시를 함 
                marker.SurchMarker[(int)side.N] = false;
            }
            else//아니면
            {
                marker.SurchMarker[(int)side.N] = true;
            }

            if (current.NE == null)
            {
                marker.SurchMarker[(int)side.NE] = false;
            }
            else if (current.NE.data == HidatoGrid.Node.unused || current.NE.data > 0)
            {
                marker.SurchMarker[(int)side.NE] = false;
            }
            else
            {
                marker.SurchMarker[(int)side.NE] = true;
            }

            if (current.E == null)
            {
                marker.SurchMarker[(int)side.E] = false;

            }
            else if (current.E.data == HidatoGrid.Node.unused || current.E.data > 0)
            {
                marker.SurchMarker[(int)side.E] = false;
            }
            else
            {
                marker.SurchMarker[(int)side.E] = true;
            }

            if (current.SE == null)
            {
                marker.SurchMarker[(int)side.SE] = false;

            }
            else if (current.SE.data == HidatoGrid.Node.unused || current.SE.data > 0)
            {
                marker.SurchMarker[(int)side.SE] = false;
            }
            else
            {
                marker.SurchMarker[(int)side.SE] = true;
            }

            if (current.S == null)
            {
                marker.SurchMarker[(int)side.S] = false;
            }
            else if (current.S.data == HidatoGrid.Node.unused || current.S.data > 0)
            {
                marker.SurchMarker[(int)side.S] = false;
            }
            else
            {
                marker.SurchMarker[(int)side.S] = true;
            }

            if (current.SW == null)
            {
                marker.SurchMarker[(int)side.SW] = false;

            }
            else if (current.SW.data == HidatoGrid.Node.unused || current.SW.data > 0)
            {
                marker.SurchMarker[(int)side.SW] = false;
            }
            else
            {
                marker.SurchMarker[(int)side.SW] = true;
            }

            if (current.W == null)
            {
                marker.SurchMarker[(int)side.W] = false;
            }
            else if (current.W.data == HidatoGrid.Node.unused || current.W.data > 0)
            {
                marker.SurchMarker[(int)side.W] = false;
            }
            else
            {
                marker.SurchMarker[(int)side.W] = true;
            }

            if (current.NW == null)
            {
                marker.SurchMarker[(int)side.NW] = false;
            }
            else if (current.NW.data == HidatoGrid.Node.unused || current.NW.data > 0)
            {
                marker.SurchMarker[(int)side.NW] = false;
            }
            else
            {
                marker.SurchMarker[(int)side.NW] = true;
            }


            return marker;
        }

        private bool ChekPrevPossibleVal(Coordinate Current, HidatoGrid Data)
        {
            Coordinate tmp;
            tmp = Current.GetSideDelta(Side.N);

            if (tmp != null)
            {

                if (current.data + 1 == current.N.data)
                {
                    return true;
                }
            }

            if (current.NE != null)
            {

                if (current.data + 1 == current.NE.data)
                {
                    return true;
                }
            }

            if (current.E != null)
            {

                if (current.data + 1 == current.E.data)
                {
                    return true;
                }
            }

            if (current.SE != null)
            {

                if (current.data + 1 == current.SE.data)
                {
                    return true;
                }
            }

            if (current.S != null)
            {

                if (current.data + 1 == current.S.data)
                {
                    return true;
                }
            }

            if (current.SW != null)
            {

                if (current.data + 1 == current.SW.data)
                {
                    return true;
                }
            }

            if (current.W != null)
            {

                if (current.data + 1 == current.W.data)
                {
                    return true;
                }
            }

            if (current.NW != null)
            {

                if (current.data + 1 == current.NW.data)
                {
                    return true;
                }
            }

            return false;
        }


        private Coordinate FindMinPossibe(HidatoGrid Data)
        {
            Array.Sort(NodeOfHidatoGridArray, delegate (HidatoGrid.Node c1, HidatoGrid.Node c2) { return c1.data.CompareTo(c2.data); });

            //구간의 시작부분과 마지막 부분을 저장하는 변수
            HidatoGrid.Node start = null;
            HidatoGrid.Node end = null;

            HidatoGrid.Node minNode = null;
            HidatoGrid.Node nextNodeOfminNode = null;

            int size = maxval;

            for (int i = 0; i < NodeOfHidatoGridArray.Length; i++)
            {
                //현재 노드
                HidatoGrid.Node currentPtr = NodeOfHidatoGridArray[i];

                if (start == null)
                {
                    start = currentNode;
                }
                else if (end == null)
                {
                    end = currentNode;

                    //if()

                    if (end.data - start.data < size && end.data - start.data > 1)
                    {
                        //모든 면이 둘러사여 있어 애초에 불가능한 구간은 건너뜀
                        if (FindOption(start).sizeOfnumSet() != 0)
                        {
                            size = end.data - start.data;
                            minNode = start;
                            nextNodeOfminNode = end;
                        }

                    }

                    start = end;
                    end = null;
                }


            }

            //NextNode = nextNodeOfminNode;
            return minNode;

        }

        public HidatoGrid.Node getPrevNode(HidatoGrid.Node current)
        {
            Array.Sort(NodeOfHidatoGridArray, delegate (HidatoGrid.Node c1, HidatoGrid.Node c2) { return c1.data.CompareTo(c2.data); });

            int cost = current.data;
            HidatoGrid.Node MinCostNode = null;
            for (int i = 0; i < NodeOfHidatoGridArray.Length; i++)
            {
                int CurrentCost;
                HidatoGrid.Node CurrentNode = NodeOfHidatoGridArray[i];

                CurrentCost = current.data - CurrentNode.data;

                if (CurrentCost <= 0)
                {
                    break;
                }

                if (CurrentCost < cost && CurrentNode.EverVisitThis == true)
                {
                    cost = CurrentCost;
                    MinCostNode = CurrentNode;
                }


            }

            return MinCostNode;
        }

        private void InitArray()
        {
            NodeOfHidatoGridArray = new HidatoGrid.Node[m_hidatoGrid.GridClength * m_hidatoGrid.GridRlength - m_hidatoGrid.DisableBoxesCount()];
            for (int i = 0; i < m_hidatoGrid.GridClength; i++)
            {
                for (int j = 0; j < m_hidatoGrid.GridRlength; j++)
                {
                    HidatoGrid.Node CurrentNode = m_hidatoGrid.GetNodeAt(i, j);
                    NodeOfHidatoGridArray[i * m_hidatoGrid.GridClength + j] = CurrentNode;
                }
            }
        }

#if DEBUG

        public void ShowArray(HidatoGrid.Node[] current)
        {
            for (int i = 0; i < current.Length; i++)
            {
                string OutPut = current[i].data.ToString() + " " + current[i].EverVisitThis + " " + (current[i].marker != null).ToString();
                Console.WriteLine(OutPut);
            }
        }

        public void ShowArray<T>(T[] obj)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                Console.WriteLine(obj[i].ToString());
            }
        }

#endif

        #endregion

        private bool solve()
        {

            //디버그와 릴리즈를 구분하나?
#if DEBUG
            int CurrentData = current.data;
            HoWManyCallSolveFunc++;
#endif


            if (SolveCancel)
            {
                return true;
            }


            bool PrevNodeIsPossible = ChekPrevPossibleVal(current);

           

            //비어있는 칸이 더 이상 없으면
            if (EmptyNodeCount == 0)
            {
                //풀린것
                return true;
            }


            ///현재 노드의 데이터가 다음 노드의 데이터보다 1이 작고, 다음 노드가 최대값이 아니면서
            #region 이전 노드의 유효성 검사
            if (current.data == TargetNode.data - 1)
            {
                ///바로 이전 노드가 올바르면(붙어 있으면)
                if (PrevNodeIsPossible == true)
                {
                    current.EverVisitThis = true;


                    if (SmartSuch == true)
                    {
                        HidatoGrid.Node NextNode = FindMinPossibe();

                        if (NextNode != null)
                        {
                            current = NextNode;

                            TargetNode = FindNextTargetNode(current);
                        }
                        else
                        {
                            if (current.data != 1)
                            {

                                //현재노드의 작업 공간(worksace)를 0으로 설정하고,(변경사항 없음 이라고 표시) 바로 이전 노드로 되돌림
                                HidatoGrid.Node prevNode = getPrevNode(current);
                                current.WorkSapce = 0;
                                current.EverVisitThis = false;

                                if (current.Input == 0)
                                {
                                    EmptyNodeCount++;
                                }
                                //맨 윗부분의 이전노드가 아니라 
                                current = prevNode;

                                //HidatoCount변수를 바꾼 현재노드의 데이터로 설정한 후
                                hidatoCount = current.data;

                                //NextNode를 업데이트 함
                                TargetNode = FindNextTargetNode(current);




                                #region 디버그를 편하게 하기 위해서 바로 업데이트 함
                                //#if DEBUG
                                if (ShowAllProcess == true)
                                {
                                    RefBoard.UpdateTextBoxes();
                                    DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                                    Thread.Sleep(ProcessWaitTime);
                                }
                                else if (DateTime.Now > DTNextUpdate)
                                {
                                    RefBoard.UpdateTextBoxes();
                                    DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                                }

                                //#endif
                                #endregion


                                return false;
                            }
                        }
                    }
                    else
                    {
                        //NextNode를 업데이트하고 현재 노드를 업데이트 전의TargetNode로 바꿈
                        current = TargetNode;
                        hidatoCount = current.data;
                        TargetNode = FindNextTargetNode(current);

                    }

                    if (solve())
                    {
                        return true;
                    }
                }
                else //그렇지 않으면
                {
                    if (current.data != 1)
                    {

                        //현재노드의 작업 공간(worksace)를 0으로 설정하고,(변경사항 없음 이라고 표시) 바로 이전 노드로 되돌림
                        HidatoGrid.Node prevNode = getPrevNode(current);
                        current.WorkSapce = 0;
                        current.EverVisitThis = false;
                        //current.emptyMarker();

                        if (current.Input == 0)
                        {
                            EmptyNodeCount++;
                        }

                        //if (current == history.currentNode)
                        {
                            //맨 윗부분의 이전노드가 아니라 
                            current = prevNode;
                        }
                        //else
                        //{
                        //맨 윗부분 노드를 현재 노드로 설정함
                        //current = history.currentNode;
                        //}

                        //HidatoCount변수를 바꾼 현재노드의 데이터로 설정한 후
                        hidatoCount = current.data;

                        //NextNode를 업데이트 함
                        TargetNode = FindNextTargetNode(current);




                        #region 디버그를 편하게 하기 위해서 바로 업데이트 함
                        //#if DEBUG
                        if (ShowAllProcess == true)
                        {
                            RefBoard.UpdateTextBoxes();
                            DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                            Thread.Sleep(ProcessWaitTime);
                        }
                        else if (DateTime.Now > DTNextUpdate)
                        {
                            RefBoard.UpdateTextBoxes();
                            DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                        }

                        //#endif
                        #endregion


                        return false;
                    }
                }
            }

            current.marker = FindOption(current);

            if (current.data == TargetNode.data - 1 && PrevNodeIsPossible == false)
            {

                if (current.data != 1)
                {

                    //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                    HidatoGrid.Node prevNode = getPrevNode(current);
                    current.WorkSapce = 0;
                    current.EverVisitThis = false;
                    current.emptyMarker();

                    if (current.Input == 0)
                    {
                        EmptyNodeCount++;
                    }

                    //if (current == history.currentNode)
                    {
                        //맨 윗부분의 이전노드가 아니라 
                        current = prevNode;
                    }
                    //else
                    //{
                    //    //맨 윗부분 노드를 현재 노드로 설정함
                    //    current = history.currentNode;
                    //}

                    hidatoCount = current.data;

                    //NextNode를 업데이트 함
                    TargetNode = FindNextTargetNode(current);

                    #region 디버그를 편하게 하기 위해서 바로 업데이트 함
                    //#if DEBUG
                    if (ShowAllProcess == true)
                    {
                        RefBoard.UpdateTextBoxes();
                        DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                        Thread.Sleep(ProcessWaitTime);
                    }
                    else if (DateTime.Now > DTNextUpdate)
                    {
                        RefBoard.UpdateTextBoxes();
                        DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                    }

                    //#endif
                    #endregion


                    return false;
                }
            }

            if ((current != TargetNode) && (current.data == TargetNode.data))
            {

                if (current.data != 1)
                {

                    HidatoGrid.Node prevNode = getPrevNode(current);
                    current.WorkSapce = 0;
                    current.EverVisitThis = false;
                    current.emptyMarker();

                    if (current.Input == 0)
                    {
                        EmptyNodeCount++;
                    }

                    //if (current == history.currentNode)
                    {
                        //맨 윗부분의 이전노드가 아니라 
                        current = prevNode;
                    }
                    //else
                    //{
                    //    //맨 윗부분 노드를 현재 노드로 설정함
                    //    current = history.currentNode;
                    //}
                    //HidatoCount변수를 바꾼 현재노드의 데이터로 설정함

                    hidatoCount = current.data;

                    //NextNode를 업데이트 함
                    TargetNode = FindNextTargetNode(current);

                    #region 디버그를 편하게 하기 위해서 바로 업데이트 함
                    //#if DEBUG
                    if (ShowAllProcess == true)
                    {
                        RefBoard.UpdateTextBoxes();
                        DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                        Thread.Sleep(ProcessWaitTime);
                    }
                    else if (DateTime.Now > DTNextUpdate)
                    {
                        RefBoard.UpdateTextBoxes();
                        DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                    }

                    //#endif
                    #endregion

                    return false;
                }

            }

            if (current.marker.sizeOfnumSet() == 0)
            {


                if (current.data != 1)
                {


                    //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                    HidatoGrid.Node prevNode = getPrevNode(current);
                    current.WorkSapce = 0;
                    current.EverVisitThis = false;
                    current.emptyMarker();

                    if (current.Input == 0)
                    {
                        EmptyNodeCount++;

                    }
                    //if (current == history.currentNode)
                    {
                        //맨 윗부분의 이전노드가 아니라 
                        current = prevNode;
                    }
                    //else
                    //{
                    //    //맨 윗부분 노드를 현재 노드로 설정함
                    //    current = history.currentNode;
                    //}

                    //HidatoCount변수를 바꾼 현재노드의 데이터로 설정한 후
                    hidatoCount = current.data;

                    //NextNode를 업데이트 함
                    TargetNode = FindNextTargetNode(current);

                    #region 디버그를 편하게 하기 위해서 바로 업데이트 함
                    //#if DEBUG
                    if (ShowAllProcess == true)
                    {
                        RefBoard.UpdateTextBoxes();
                        DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                        Thread.Sleep(ProcessWaitTime);
                    }
                    else if (DateTime.Now > DTNextUpdate)
                    {
                        RefBoard.UpdateTextBoxes();
                        DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                    }

                    //#endif
                    #endregion


                    return false;
                }

            }
            #endregion


            for (int i = 0; i < 8; i++)
            {
                #region 가변 백트랙킹 기능 구현을 위한 코드

                //if (NextNodeHasUpdated == false)
                //{
                //   TargetNode = FindNextNode(current);
                //    NextNodeHasUpdated = true;
                //}
                #endregion

                bool InsertSuccess = false;

                for (int j = 0; j < 8; j++)
                {
                    if (current.marker.SurchMarker[insertNow.current] == true)
                    {
                        
                        current.EverVisitThis = true;
                        hidatoCount = current.data;
                        //이미 탐색한 자리임으로 false마킹

#if DEBUG
                        history.Add(current.data);
#endif


                        #region 이미 탐색한 자리는 false로 마킹합니다. 
                        if (insertNow.current == (int)side.N)
                        {
                            current.marker.SurchMarker[(int)side.N] = false;
                        }
                        else if (insertNow.current == (int)side.NE)
                        {
                            current.marker.SurchMarker[(int)side.NE] = false;
                        }
                        else if (insertNow.current == (int)side.E)
                        {
                            current.marker.SurchMarker[(int)side.E] = false;
                        }
                        else if (insertNow.current == (int)side.SE)
                        {
                            current.marker.SurchMarker[(int)side.SE] = false;
                        }
                        else if (insertNow.current == (int)side.S)
                        {
                            current.marker.SurchMarker[(int)side.S] = false;
                        }
                        else if (insertNow.current == (int)side.SW)
                        {
                            current.marker.SurchMarker[(int)side.SW] = false;
                        }
                        else if (insertNow.current == (int)side.W)
                        {
                            current.marker.SurchMarker[(int)side.W] = false;
                        }
                        else if (insertNow.current == (int)side.NW)
                        {
                            current.marker.SurchMarker[(int)side.NW] = false;
                        }
                        #endregion

                        #region current참조를 알맞은 위치로 옮기는 코드블럭 입니다.
                        if (insertNow.current == (int)side.N)
                        {
                            current = current.N;
                        }
                        else if (insertNow.current == (int)side.NE)
                        {
                            current = current.NE;
                        }
                        else if (insertNow.current == (int)side.E)
                        {
                            current = current.E;
                        }
                        else if (insertNow.current == (int)side.SE)
                        {
                            current = current.SE;
                        }
                        else if (insertNow.current == (int)side.S)
                        {
                            current = current.S;
                        }
                        else if (insertNow.current == (int)side.SW)
                        {
                            current = current.SW;
                        }
                        else if (insertNow.current == (int)side.W)
                        {
                            current = current.W;
                        }
                        else if (insertNow.current == (int)side.NW)
                        {
                            current = current.NW;
                        }
                        #endregion

                        hidatoCount++;
                        current.WorkSapce = hidatoCount;
                        //비어있는 칸이 1칸 줄어들기 때문에 1을 줄여야 합니다.
                        EmptyNodeCount--;


                        #region 디버그를 편하게 하기 위해서 바로 업데이트 함
                        //#if DEBUG
                        if (ShowAllProcess == true)
                        {
                            RefBoard.UpdateTextBoxes();
                            DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                            Thread.Sleep(ProcessWaitTime);
                        }
                        else if (DateTime.Now > DTNextUpdate)
                        {
                            RefBoard.UpdateTextBoxes();
                            DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                        }

                        //#endif
                        #endregion

                        InsertSuccess = true;

                        break;
                    }

                    insertNow.increase();
                }

                if (!InsertSuccess)
                {

                    if (current.data != 1)
                    {

                        //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                        HidatoGrid.Node prevNode = getPrevNode(current);
                        current.WorkSapce = 0;
                        current.EverVisitThis = false;
                        current.emptyMarker();

                        if (current.Input == 0)
                        {
                            EmptyNodeCount++;
                        }

                        //if (current == history.currentNode)
                        {
                            //맨 윗부분의 이전노드가 아니라 
                            current = prevNode;
                        }
                        //else
                        //{
                        //    //맨 윗부분 노드를 현재 노드로 설정함
                        //    current = history.currentNode;
                        //}

                        //HidatoCount변수를 바꾼 현재노드의 데이터로 설정한 후
                        hidatoCount = current.data;

                        TargetNode = FindNextTargetNode(current);


                        #region 디버그를 편하게 하기 위해서 바로 업데이트 함
                        //#if DEBUG
                        if (ShowAllProcess == true)
                        {
                            RefBoard.UpdateTextBoxes();
                            DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                            Thread.Sleep(ProcessWaitTime);
                        }
                        else if (DateTime.Now > DTNextUpdate)
                        {
                            RefBoard.UpdateTextBoxes();
                            DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                        }

                        //#endif
                        #endregion
                        //#endif
                        return false;
                    }
                    else
                    {
                        continue;
                    }

                }


                if (ShowAllProcess == true)
                {
                    RefBoard.UpdateTextBoxes();
                    DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                    Thread.Sleep(ProcessWaitTime);
                }
                else if (DateTime.Now > DTNextUpdate)
                {
                    RefBoard.UpdateTextBoxes();
                    DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
                }

                if (solve() == true)
                {
                    return true;
                }
            }


            return false;
        }

        /// <summary>
        /// 풀이 시작하기 전 값 초기화
        /// </summary>
        /// <param name="board"></param>
        public void startsolve(Hidato_Board board)
        {

            SolveCancel = false;
            m_isProcess = true;
            DateTime dtstart = DateTime.Now;
            current = FirstNode = FindFirstNode();
            maxval = FindMaxVal();
            //history.AddNode(current);
            TargetNode = FindNextTargetNode(current);
            InitArray();
            SmartSuch = true;
            EmptyNodeCount = maxval - m_hidatoGrid.HintCount();

            if (SmartSuch == true)
            {
                current = FindMinPossibe();

                TargetNode = FindNextTargetNode(current);
            }

            RefBoard = board;
            DTNextUpdate = DTNextUpdate.AddSeconds(NextUpdateSoconds);

            bool success = solve();

            m_isProcess = false;
            if (success && !SolveCancel)
            {
                RefBoard.ShowSFdialog(true, dtstart);
                return;
            }
            else if (success && SolveCancel)
            {
                return;
            }
            else
            {
                RefBoard.ShowSFdialog(false, dtstart);
                return;
            }


        }

        public void cancel()
        {
            SolveCancel = true;
        }

        public void show()
        {
            Console.WriteLine("");
            for (int i = 0; i < m_hidatoGrid.GridCols; i++)
            {
                Console.WriteLine("");
                for (int j = 0; j < m_hidatoGrid.GridRows; j++)
                {
                    int data = m_hidatoGrid.GetDataAt(j, i);
                    Console.Write(" ");
                    Console.Write("{0}", data);
                }
            }

            Console.WriteLine("");
        }

    }
}
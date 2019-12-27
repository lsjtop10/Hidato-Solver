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
    enum Side { N = 0, NE, E, SE, S, SW, W, NW }
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

        public int SizeOfnumSet()
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


    class history
    {
        private List<HidatoGrid.Node> m_history = new List<HidatoGrid.Node>();

        public void AddNode(HidatoGrid.Node node)
        {
            this.m_history.Add(node);
            m_history.Sort(delegate (HidatoGrid.Node c1, HidatoGrid.Node c2) { return c1.Data.CompareTo(c2.Data); });
        }

        public HidatoGrid.Node CurrentNode
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

      
        public HidatoGrid.Node GetPrevNode(HidatoGrid.Node current)
        {
            int cost = current.Data;
            HidatoGrid.Node MinCostNode = null;
            for(int i = 0; i < m_history.Count; i++)
            {
                int CurrentCost;
                HidatoGrid.Node CurrentNode = m_history[i];

                CurrentCost = current.Data - CurrentNode.Data;

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
            //사용하지 않는 값을 unused로 상수화 라는 것을 시킵니다.
            //상수화는 문자처럼 보이지만(변수처럼 보이지만) 상수처럼 기능하는 것을 말합니다.
            //파이(pi)를 생각하면 쉽습니다.
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

        public Node GetNodeAt(int Yindex, int Xindex)
        {
            int count = 0;
            Node horse = hard;

            while (Yindex != count)
            {
                horse = horse.S;
                count++;
            }

            count = 0;
            while (Xindex != count)
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
            for(int i = 0; i < GridClength; i++)
            {
                for(int j = 0; j < GridRlength; j++)
                {
                    if(GetDataAt(i,j) == 0)
                    {
                        disableCount++;
                    }
                }
            }

            Disable = disableCount;
            return disableCount;
        }

    }


    public class WhereInsertNow
    {
        private int minval;
        private int maxval;

        private int m_current = 0;

        public WhereInsertNow(int minval, int maxval, int InitialVal)
        {
            this.minval = minval;
            this.maxval = maxval;
            m_current = InitialVal;
        }

        public int Current
        {
            set
            {
                if (!(value < maxval))
                {
                    m_current = value;
                }
            }

            get { return m_current; }
        }

        //1식 증가하다가 처음으로 돌아옵니다.
        public int Increase()
        {
            if (m_current >= maxval)
            {
                m_current = minval;
            }
            else
            {
                m_current++;
            }

            return m_current;
        }

    }


    class HidatoSolver
    {
        private int hidatoCount = 1;
        private history history;
        private HidatoGrid m_hidatoGrid;
        private HidatoGrid.Node current;
        private int maxval;
        private HidatoGrid.Node NextNode;
        private HidatoGrid.Node FirstNode;
        private DateTime DTNextUpdate;
        private Hidato_Board RefBoard;
        private bool SolveCancel = false;
        private WhereInsertNow insertNow = new WhereInsertNow((int)Side.N, (int)Side.NW, (int)Side.N);
        private bool SmartSuch = false;
        private bool m_isProcess = false;
        private List<HidatoGrid.Node> NodeOfHidatoGridList = new List<HidatoGrid.Node>();
        /// <summary>
        /// 비어있는 칸 카운트
        /// </summary>
        private int EmptyNodeCount;
        ulong HoWManyExit = 0;


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
        /// 푸는 중에 일시 얼마나 길게 일시 정지할지 확인합니다 단위는 ms입니다.
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
        private HidatoGrid.Node FindFirstNode()
        {
            for (int i = 0; i < m_hidatoGrid.GridClength; i++)
                for (int j = 0; j < m_hidatoGrid.GridRlength; j++)
                {
                    HidatoGrid.Node temp = m_hidatoGrid.GetNodeAt(i, j);
                    if (temp.Input == 1)
                    {
                        return temp;
                    }
                }
            return null;
        }

        private HidatoGrid.Node FindNextNode(HidatoGrid.Node current)
        {
            //다음노드
            HidatoGrid.Node nextnode = FindFirstNode();
            //현재노드
            HidatoGrid.Node nNode;

            //최대값 지역변수를 선언한 다음 미리 구해놓은 맴버 변수(maxval변수)에서 값을 가져옴 
            int maxval = this.maxval;

            for (int i = 0; i < m_hidatoGrid.GridClength; i++)
            {
                for (int j = 0; j < m_hidatoGrid.GridRlength; j++)
                {
                    //현재 위치의 노드;
                    nNode = m_hidatoGrid.GetNodeAt(i, j);

                    //현재 위치의 노드의 값이 매개변수로 넘어온 노드보다 크면서
                    //최대값보다 작으면

                    if (nNode.Data > current.Data && nNode.Data <= maxval)
                    {
                        maxval = nNode.Data;
                        nextnode = nNode;
                    }

                }
            }
            return nextnode;

        }

        private int FindMaxVal()
        {
            //최대값
            int maxval = 0;
            //현재값
            int nval = 0;

            for (int i = 0; i < m_hidatoGrid.GridClength; i++)
                for (int j = 0; j < m_hidatoGrid.GridRlength; j++)
                {
                    //현재 위치의 값
                    nval = m_hidatoGrid.GetDataAt(i, j);

                    //만약 최대값이 현재값보다 작으면
                    if (nval > maxval)
                    {
                        //최대값은 현재값
                        maxval = nval;
                    }
                }

            return maxval;

        }

        private SurchMarking FindOption(HidatoGrid.Node current)
        {
            SurchMarking marker = new SurchMarking();
            //현재 노드의 윗부분(북쪽)이 쓰지 않는 부분이거나(비대칭시 사용), 윗부분이 없거나, 뭐가 차있으면
            if (current.N == null)
            {
                marker.SurchMarker[(int)Side.N] = false;
            }
            else if (current.N.Data == HidatoGrid.Node.unused || current.N.Data > 0)
            {
                //사용 할 수없다는 표시를 함 
                marker.SurchMarker[(int)Side.N] = false;
            }
            else//아니면
            {
                marker.SurchMarker[(int)Side.N] = true;
            }

            if (current.NE == null)
            {
                marker.SurchMarker[(int)Side.NE] = false;
            }
            else if (current.NE.Data == HidatoGrid.Node.unused || current.NE.Data > 0)
            {
                marker.SurchMarker[(int)Side.NE] = false;
            }
            else
            {
                marker.SurchMarker[(int)Side.NE] = true;
            }

            if (current.E == null)
            {
                marker.SurchMarker[(int)Side.E] = false;

            }
            else if (current.E.Data == HidatoGrid.Node.unused || current.E.Data > 0)
            {
                marker.SurchMarker[(int)Side.E] = false;
            }
            else
            {
                marker.SurchMarker[(int)Side.E] = true;
            }

            if (current.SE == null)
            {
                marker.SurchMarker[(int)Side.SE] = false;

            }
            else if (current.SE.Data == HidatoGrid.Node.unused || current.SE.Data > 0)
            {
                marker.SurchMarker[(int)Side.SE] = false;
            }
            else
            {
                marker.SurchMarker[(int)Side.SE] = true;
            }

            if (current.S == null)
            {
                marker.SurchMarker[(int)Side.S] = false;
            }
            else if (current.S.Data == HidatoGrid.Node.unused || current.S.Data > 0)
            {
                marker.SurchMarker[(int)Side.S] = false;
            }
            else
            {
                marker.SurchMarker[(int)Side.S] = true;
            }

            if (current.SW == null)
            {
                marker.SurchMarker[(int)Side.SW] = false;

            }
            else if (current.SW.Data == HidatoGrid.Node.unused || current.SW.Data > 0)
            {
                marker.SurchMarker[(int)Side.SW] = false;
            }
            else
            {
                marker.SurchMarker[(int)Side.SW] = true;
            }

            if (current.W == null)
            {
                marker.SurchMarker[(int)Side.W] = false;
            }
            else if (current.W.Data == HidatoGrid.Node.unused || current.W.Data > 0)
            {
                marker.SurchMarker[(int)Side.W] = false;
            }
            else
            {
                marker.SurchMarker[(int)Side.W] = true;
            }

            if (current.NW == null)
            {
                marker.SurchMarker[(int)Side.NW] = false;
            }
            else if (current.NW.Data == HidatoGrid.Node.unused || current.NW.Data > 0)
            {
                marker.SurchMarker[(int)Side.NW] = false;
            }
            else
            {
                marker.SurchMarker[(int)Side.NW] = true;
            }


            return marker;
        }

        private bool ChekPrevPossibleVal(HidatoGrid.Node current)
        {
            if (current.N != null)
            {

                if (current.Data + 1 == current.N.Data)
                {
                    return true;
                }
            }

            if (current.NE != null)
            {

                if (current.Data + 1 == current.NE.Data)
                {
                    return true;
                }
            }

            if (current.E != null)
            {

                if (current.Data + 1 == current.E.Data)
                {
                    return true;
                }
            }

            if (current.SE != null)
            {

                if (current.Data + 1 == current.SE.Data)
                {
                    return true;
                }
            }

            if (current.S != null)
            {

                if (current.Data + 1 == current.S.Data)
                {
                    return true;
                }
            }

            if (current.SW != null)
            {

                if (current.Data + 1 == current.SW.Data)
                {
                    return true;
                }
            }

            if (current.W != null)
            {

                if (current.Data + 1 == current.W.Data)
                {
                    return true;
                }
            }

            if (current.NW != null)
            {

                if (current.Data + 1 == current.NW.Data)
                {
                    return true;
                }
            }

            return false;
        }

        private int HintCount()
        {
            int HintCount = 0;
            for (int i = 0; i < m_hidatoGrid.GridClength; i++)
            {
                for (int j = 0; j < m_hidatoGrid.GridRlength; j++)
                {
                    if (m_hidatoGrid.GetNodeAt(i, j).Data > 0)
                    {
                        HintCount++;
                    }
                }
            }

            return HintCount;
        }

        private HidatoGrid.Node FindMinPossibe()
        {
            NodeOfHidatoGridList.Sort(delegate (HidatoGrid.Node c1, HidatoGrid.Node c2) { return c1.Data.CompareTo(c2.Data); });

            //구간의 시작부분과 마지막 부분을 저장하는 변수
            HidatoGrid.Node start = null;
            HidatoGrid.Node end = null;

            HidatoGrid.Node minNode = null;
            HidatoGrid.Node nextNodeOfminNode = null;

            int size = maxval;

            for (int i = 0; i < NodeOfHidatoGridList.Count; i++)
            {
                //현재 노드
                HidatoGrid.Node currentNode = NodeOfHidatoGridList[i];

                if(currentNode.Data == 0)
                {
                    NodeOfHidatoGridList.Remove(currentNode);
                    continue;
                }
                else
                {
                    if (start == null)
                    {
                        start = currentNode;
                    }
                    else if (end == null)
                    {
                        end = currentNode;

                        //if()

                        if (end.Data - start.Data < size && end.Data - start.Data > 1)
                        {
                            //모든 면이 둘러사여 있어 애초에 불가능한 구간은 건너뜀
                            if(FindOption(start).SizeOfnumSet() != 0)
                            {
                                size = end.Data - start.Data;
                                minNode = start;
                                nextNodeOfminNode = end;
                            }
                            
                        }

                        start = end;
                        end = null;
                    }

                }

            }

            //NextNode = nextNodeOfminNode;
            return minNode;

        }

        private void InitList()
        {
            for (int i = 0; i < m_hidatoGrid.GridClength; i++)
            {
                for (int j = 0; j < m_hidatoGrid.GridRlength; j++)
                {
                    HidatoGrid.Node nNode = m_hidatoGrid.GetNodeAt(i, j);
                    if (nNode.Data > 0)
                    {
                        NodeOfHidatoGridList.Add(nNode);
                    }
                }
            }
        }


        #endregion

        //몆번에 한번 꼴로 HowManyExitStack변수의 값을 올릴 것 인지 설정하는 상수
        private bool Solve()
        {

            //디버그와 릴리즈를 구분하나?
#if DEBUG
            int CurrentData = current.Data;
#endif

            
            if (SolveCancel)
            {
                return true;
            }

            //if (ShowAllProcess == true)
            //{
            //    RefBoard.UpdateTextBoxes();
            //    DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
            //    Thread.Sleep(ProcessWaitTime);
            //}
            //else if (DateTime.Now > DTNextUpdate)
            //{
            //    RefBoard.UpdateTextBoxes();
            //    DTNextUpdate = DateTime.Now.AddSeconds(NextUpdateSoconds);
            //}

            //SmartSuch옵션이 켜져 있으면
            //if (SmartSuch == true)
            //{
            //    //가장 작은 경우의 수를 가지는 구간의 시작 노드를 현재 노드로 설정하고 그 다음 노드(구간의 끝 노드)를 NextNode변수에 저장
            //    current = FindMinPossibe();
            //    NextNode = FindNextNode(current);
            //}

            bool PrevNodeIsPossible = ChekPrevPossibleVal(current);


            current.marker = FindOption(current);


            //if (current.data == maxval - 1 && PrevNodeIsPossible == true)
            //{
            //    return true;
            //}

            //비어있는 칸이 더 이상 없으면
            if (EmptyNodeCount == 0)
            {
                //풀린것
                return true;
            }


            ///현재 노드의 데이터가 다음 노드의 데이터보다 1이 작고, 다음 노드가 최대값이 아니면서
            #region 이전 노드의 유효성 검사
            if (current.Data == NextNode.Data - 1)
            {
                ///바로 이전 노드가 올바르면(붙어 있으면)
                if (PrevNodeIsPossible == true)
                {
                    history.AddNode(current);

                    if (SmartSuch == true)
                    {
                        current = FindMinPossibe();

                        NextNode = FindNextNode(current);
                    }
                    else
                    {
                        //NextNode를 업데이트하고 현재 노드를 업데이트 전의 NextNode로 바꿈
                        current = NextNode;
                        hidatoCount = current.Data;
                        NextNode = FindNextNode(current);
                        
                    }

                    if (Solve())
                    {
                        return true;
                    }
                }
                else //그렇지 않으면
                {
                    if (current.Data != 1)
                    {

                        //현재노드의 작업 공간(worksace)를 0으로 설정하고,(변경사항 없음 이라고 표시) 바로 이전 노드로 되돌림
                        HidatoGrid.Node prevNode = history.GetPrevNode(current);
                        current.WorkSapce = 0;
                        current.EmptyMarker();

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
                        hidatoCount = current.Data;

                        //NextNode를 업데이트 함
                        NextNode = FindNextNode(current);




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


#if DEBUG
                        HoWManyExit++;
                        Console.WriteLine("");
                        Console.WriteLine("{0}", EmptyNodeCount);
                        Console.WriteLine("{0}", maxval - HintCount());
                        Console.WriteLine("{0}", HoWManyExit);

#endif

                        return false;
                    }
                }
            }

            if (current.Data == NextNode.Data - 1 && PrevNodeIsPossible == false)
            {

                if (current.Data != 1)
                {

                    //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                    HidatoGrid.Node prevNode = history.GetPrevNode(current);
                    current.WorkSapce = 0;
                    current.EmptyMarker();

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

                    hidatoCount = current.Data;

                    //NextNode를 업데이트 함
                    NextNode = FindNextNode(current);

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

                   
//#if DEBUG
//                    HoWManyExit++;
//                    Console.WriteLine("");
//                    Console.WriteLine("{0}", EmptyNodeCount);
//                    Console.WriteLine("{0}", maxval - HintCount());
//                    Console.WriteLine("{0}", HoWManyExit);
//#endif

                    return false;
                }
            }

            if ((current != NextNode) && (current.Data == NextNode.Data))
            {

                if (current.Data != 1)
                {

                    HidatoGrid.Node prevNode = history.GetPrevNode(current);
                    current.WorkSapce = 0;
                    current.EmptyMarker();

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

                    hidatoCount = current.Data;

                    //NextNode를 업데이트 함
                    NextNode = FindNextNode(current);

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
                    

//#if DEBUG
//                    HoWManyExit++;
//                    Console.WriteLine("");
//                    Console.WriteLine("{0}", EmptyNodeCount);
//                    Console.WriteLine("{0}", maxval - HintCount());
//                    Console.WriteLine("{0}", HoWManyExit);
//#endif

                    return false;
                }

            }

            if (current.marker.SizeOfnumSet() == 0)
            {


                if (current.Data != 1)
                {


                    //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                    HidatoGrid.Node prevNode = history.GetPrevNode(current);
                    current.WorkSapce = 0;
                    current.EmptyMarker();

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
                    hidatoCount = current.Data;

                    //NextNode를 업데이트 함
                    NextNode = FindNextNode(current);

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

//#if DEBUG
//                    HoWManyExit++;
//                    Console.WriteLine("");
//                    Console.WriteLine("{0}", EmptyNodeCount);
//                    Console.WriteLine("{0}", maxval - HintCount());
//                    Console.WriteLine("{0}", HoWManyExit);
//#endif

                    return false;
                }

            }
#endregion

            
            for (int i = 0; i < 8; i++)
            {
#region 가변 백트랙킹 기능 구현을 위한 코드
                //while (StackCount < HowManyExitStack)
                //{
                //    StackCount++;

                //    //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                //    current.WorkSapce = 0;

                //    current = history.prevNode;

                //    hidatoCount = current.WorkSapce;

                //    return false;
                //}


                //if (NextNodeHasUpdated == false)
                //{
                //    NextNode = FindNextNode(current);
                //    NextNodeHasUpdated = true;
                //}
#endregion

                bool InsertSuccess = false;

                for (int j = 0; j < 8; j++)
                {
                    if (current.marker.SurchMarker[insertNow.Current] == true)
                    {

                        hidatoCount = current.Data;
                        //이미 탐색한 자리임으로 false마킹
                        history.AddNode(current);

#region 이미 탐색한 자리는 false로 마킹합니다. 
                        if (insertNow.Current == (int)Side.N)
                        {
                            current.marker.SurchMarker[(int)Side.N] = false;
                        }
                        else if (insertNow.Current == (int)Side.NE)
                        {
                            current.marker.SurchMarker[(int)Side.NE] = false;
                        }
                        else if (insertNow.Current == (int)Side.E)
                        {
                            current.marker.SurchMarker[(int)Side.E] = false;
                        }
                        else if (insertNow.Current == (int)Side.SE)
                        {
                            current.marker.SurchMarker[(int)Side.SE] = false;
                        }
                        else if (insertNow.Current == (int)Side.S)
                        {
                            current.marker.SurchMarker[(int)Side.S] = false;
                        }
                        else if (insertNow.Current == (int)Side.SW)
                        {
                            current.marker.SurchMarker[(int)Side.SW] = false;
                        }
                        else if (insertNow.Current == (int)Side.W)
                        {
                            current.marker.SurchMarker[(int)Side.W] = false;
                        }
                        else if (insertNow.Current == (int)Side.NW)
                        {
                            current.marker.SurchMarker[(int)Side.NW] = false;
                        }
#endregion

#region current참조를 알맞은 위치로 옮기는 코드블럭 입니다.
                        if (insertNow.Current == (int)Side.N)
                        {
                            current = current.N;
                        }
                        else if (insertNow.Current == (int)Side.NE)
                        {
                            current = current.NE;
                        }
                        else if (insertNow.Current == (int)Side.E)
                        {
                            current = current.E;
                        }
                        else if (insertNow.Current == (int)Side.SE)
                        {
                            current = current.SE;
                        }
                        else if (insertNow.Current == (int)Side.S)
                        {
                            current = current.S;
                        }
                        else if (insertNow.Current == (int)Side.SW)
                        {
                            current = current.SW;
                        }
                        else if (insertNow.Current == (int)Side.W)
                        {
                            current = current.W;
                        }
                        else if (insertNow.Current == (int)Side.NW)
                        {
                            current = current.NW;
                        }
#endregion

                        NodeOfHidatoGridList.Add(current);

                        hidatoCount++;
                        current.WorkSapce = hidatoCount;
                        //history.AddNode(current);
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
                    
                    insertNow.Increase();
                }

                if (!InsertSuccess)
                {

                    if (current.Data != 1)
                    {

                        //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                        HidatoGrid.Node prevNode = history.GetPrevNode(current);
                        current.WorkSapce = 0;
                        current.EmptyMarker();

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
                        hidatoCount = current.Data;

                        NextNode = FindNextNode(current);
                        

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

                if (Solve() == true)
                {
                    return true;
                }
            }


            return false;
        }

        public void Startsolve(Hidato_Board board)
        {

            SolveCancel = false;
            m_isProcess = true;
            DateTime dtstart = DateTime.Now;
            current = FirstNode = FindFirstNode();
            maxval = FindMaxVal();
            history = new history();
            //history.AddNode(current);
            NextNode = FindNextNode(current);
            InitList();
            SmartSuch = false;
            EmptyNodeCount = maxval - HintCount();

            if (SmartSuch == true)
            {
                current = FindMinPossibe();
            }

            RefBoard = board;
            DTNextUpdate = DTNextUpdate.AddSeconds(NextUpdateSoconds);

            bool success = Solve();

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

        public void Cancel()
        {
            SolveCancel = true;
        }

        public void Show()
        {
            Console.WriteLine("");
            for (int i = 0; i < m_hidatoGrid.GridClength; i++)
            {
                Console.WriteLine("");
                for (int j = 0; j < m_hidatoGrid.GridRlength; j++)
                {
                    int data = m_hidatoGrid.GetDataAt(i, j);
                    Console.Write(" ");
                    Console.Write("{0}", data);
                }
            }

            Console.WriteLine("");
        }

    }
}

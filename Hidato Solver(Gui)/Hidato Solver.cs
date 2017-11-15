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
    enum side { N = 0, NE, E, SE, S, SW, W, NW }
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

        public int sizeOfnumSet()
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
        private Node current;
        private Node hard;

        //구조체로 하면 에러가 난다 구조체는 참조 형식이 아니라 값 형식이기 때문
        public class Node
        {
            public HidatoGrid.Node node;
            public history.Node next;
            public history.Node prev;

        }

        public history()
        {
            hard = new history.Node();
            current = hard;
        }

        public history.Node currentNode
        {
            get { return current; }
        }

        public void AddNode(HidatoGrid.Node node)
        {
            //while(current.next != null)
            //{
            //    current = current.next; 
            //}

            history.Node temp = new history.Node();
            temp.node = node;
            current.next = temp;
            temp.prev = current;


            current = current.next;
        }

        public history.Node GetCurrentElement()
        {
            Node temp = current;

            if (current.node.data == 1)
            {
                return current;
            }
            //else
            //{
            //    current = current.prev;

            //    current.next.prev = null;
            //    current.next = null;
            //}

            return temp;
        }

        public history.Node GetPrevElement()
        {

            if (current.node.data == 1)
            {
                return current;
            }
            else
            {
                current = current.prev;
                current.next.prev = null;
                current.next = null;
            }

            return current;
        }
    }

    //hidato는 인접한 면에 자기숫자 +1만큼의 숫자가 들어가야 합니다. 
    //그래서 주변의 인접한 면과 연결되야 하는데.저는 링크드 리스트(연결된 리스트)를 사용하겠습니다.
    //연결된 리스트는 추적을 위한 hard값과 나머지node 를 서로 참조(원래는 포인터를 이용합니다.)즉 가리키게 해서
    //묶어놓는 방식입니다. 지금은 8개의 면과 대응되야 하기 때문에 8개의 변수를 만들겠습니다.
    //node의 삽입과 삭제는 필요가 없기 때문에 (처음에 판만 만들면 되므로) 삭제는 구현에서 생략합니다.
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

            public int data
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

        public int current
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
        public int increase()
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
        private int StackCount = 1;
        private int HowManyExitStack = 1;
        private int InsertCount = 0;
        private bool NextNodeHasUpdated = true;
        private DateTime DTNextUpdate;
        private Hidato_Board RefBoard;
        private bool SolveCancel = false;
        private WhereInsertNow insertNow = new WhereInsertNow((int)side.N, (int)side.NW, (int)side.N);
        private bool SmartSuch = false;
        private bool m_isProcess = false;
        private List<HidatoGrid.Node> NodeOfHidatoGridList = new List<HidatoGrid.Node>();
        /// <summary>
        /// 비어있는 칸 카운트
        /// </summary>
        private int EmptyNodeCount;

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

                    if (nNode.data > current.data && nNode.data <= maxval)
                    {
                        maxval = nNode.data;
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
                marker.SurchMarker[(int)side.N] = false;
            }
            else if (current.N.data == HidatoGrid.Node.unused || current.N.data > 0)
            {
                //사용 할 수없다는 표시를 함 
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

        private bool ChekPrevPossibleVal(HidatoGrid.Node current)
        {
            if (current.N != null)
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

        private int HintCount()
        {
            int HintCount = 0;
            for (int i = 0; i < m_hidatoGrid.GridClength; i++)
            {
                for (int j = 0; j < m_hidatoGrid.GridRlength; j++)
                {
                    if (m_hidatoGrid.GetNodeAt(i, j).Input > 0)
                    {
                        HintCount++;
                    }
                }
            }

            return HintCount;
        }

        private HidatoGrid.Node FindMinPossibe()
        {
            NodeOfHidatoGridList.Sort(delegate (HidatoGrid.Node c1, HidatoGrid.Node c2) { return c1.data.CompareTo(c2.data); });

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

                if(currentNode.data == 0)
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

                        if (end.data - start.data < size && end.data - start.data > 1)
                        {
                            //모든 면이 둘러사여 있어 애초에 불가능한 구간은 건너뜀
                            if(FindOption(start).sizeOfnumSet() != 0)
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
                    if (nNode.data > 0)
                    {
                        NodeOfHidatoGridList.Add(nNode);
                    }
                }
            }
        }


        #endregion

        //몆번에 한번 꼴로 HowManyExitStack변수의 값을 올릴 것 인지 설정하는 상수
        private const int HowOftenChange = 50;
        private bool solve()
        {

            //디버그와 릴리즈를 구분하나?
#if DEBUG
            int CurrentData = current.data;
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

            SurchMarking marker = FindOption(current);

            //비어있는 칸이 더 이상 없으면
            if (EmptyNodeCount == 0)
            {
                //풀린것
                return true;
            }

            
            ///현재 노드의 데이터가 다음 노드의 데이터보다 1이 작고, 다음 노드가 최대값이 아니면서
            #region 이전 노드의 유효성 검사
            if (current.data == NextNode.data - 1)
            {
                ///바로 이전 노드가 올바르면(붙어 있으면)
                if (PrevNodeIsPossible == true)
                {
                    if (SmartSuch == true)
                    {
                        current = FindMinPossibe();

                        NextNode = FindNextNode(current);
                    }
                    else
                    {
                        //NextNode를 업데이트하고 현재 노드를 업데이트 전의 NextNode로 바꿈
                        current = NextNode;
                        hidatoCount = current.data;
                        NextNode = FindNextNode(current);
                        history.AddNode(current);
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
                        current.WorkSapce = 0;

                        //smartshch기능을 사용하면 history의 맨 윗부분이 현재노드와 같지 않기 때문에 
                        if (current == history.currentNode.node)
                        {
                            //맨 윗부분의 이전노드가 아니라 
                            current = history.GetPrevElement().node;
                        }
                        //else
                        {
                            //맨 윗부분 노드를 현재 노드로 설정함
                            current = history.GetCurrentElement().node;
                        }
                        //HidatoCount변수를 바꾼 현재노드의 데이터로 설정한 후
                        hidatoCount = current.data;

                        //스텍이 빠져나가는 깊이가 1이면
                        if (HowManyExitStack == 1)
                        {
                            //NextNode변수를 업데이트 하고
                            NextNode = FindNextNode(current);
                            //NextNode변수 업데이트 여부를 저장하는 bool형 변수를 true로 마킹
                            NextNodeHasUpdated = true;
                        }
                        else
                        {
                            ///아니면 NextNode변수 업데이트 여부를 저장하는 bool형 변수를 false로 마킹을해서
                            ///스택 깊이만큼 BackTracking처리하는 while문에서 처리가 끝나면 
                            ///최종 값으로 업데이트 되게 맡킴
                            NextNodeHasUpdated = false;
                        }

                        #region 디버그를 편하게 하기 위해서 바로 업데이트 함
#if DEBUG
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

#endif
                        #endregion

                        EmptyNodeCount++;

                        return false;
                    }
                }
            }

            if (current.data == NextNode.data - 1 && PrevNodeIsPossible == false)
            {

                if (current.data != 1)
                {
                    
                    //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                    current.WorkSapce = 0;

                    if (current == history.currentNode.node)
                    {
                        //맨 윗부분의 이전노드가 아니라 
                        current = history.GetPrevElement().node;
                    }
                    else
                    {
                        //    맨 윗부분 노드를 현재 노드로 설정함
                        current = history.GetCurrentElement().node;
                    }

                    hidatoCount = current.data;

                    if (HowManyExitStack == 1)
                    {
                        NextNode = FindNextNode(current);
                        NextNodeHasUpdated = true;
                    }
                    else
                    {
                        NextNodeHasUpdated = false;
                    }

                    hidatoCount = current.data;

                    #region 디버그를 편하게 하기 위해서 바로 업데이트 함
#if DEBUG
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

#endif
                    #endregion

                    EmptyNodeCount++;
                    return false;
                }
            }

            if ((current != NextNode) && (current.data == NextNode.data))
            {

                if (current.data != 1)
                {

                    StackCount = 1;

                    current.WorkSapce = 0;

                    if (current == history.currentNode.node)
                    {
                        //맨 윗부분의 이전노드가 아니라 
                        current = history.GetPrevElement().node;
                    }
                    else
                    {
                        //맨 윗부분 노드를 현재 노드로 설정함
                        current = history.GetCurrentElement().node;
                    }
                    //HidatoCount변수를 바꾼 현재노드의 데이터로 설정함

                    hidatoCount = current.data;

                    if (HowManyExitStack == 1)
                    {
                        NextNode = FindNextNode(current);
                        NextNodeHasUpdated = true;
                    }
                    else
                    {
                        NextNodeHasUpdated = false;
                    }

                    hidatoCount = current.data;

                    #region 디버그를 편하게 하기 위해서 바로 업데이트 함
#if DEBUG
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

#endif
                    #endregion

                    EmptyNodeCount++;
                    return false;
                }

            }

            if (marker.sizeOfnumSet() == 0)
            {


                if (current.data != 1)
                {
                    StackCount = 1;

                  
                    //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                    current.WorkSapce = 0;

                    if (current == history.currentNode.node)
                    {
                        //맨 윗부분의 이전노드가 아니라 
                        current = history.GetPrevElement().node;
                    }
                    else
                    {
                        //맨 윗부분 노드를 현재 노드로 설정함
                        current = history.GetCurrentElement().node;
                    }

                    //HidatoCount변수를 바꾼 현재노드의 데이터로 설정한 후
                    hidatoCount = current.data;

                    //빠져나가야 하는 스택 깊이가 1이면
                    if (HowManyExitStack == 1)
                    {
                        //바로 NextNode를 업데이트 하고
                        NextNode = FindNextNode(current);
                        //NextNode가 업데이트 되었다는 표시를 함
                        NextNodeHasUpdated = true;
                    }
                    else
                    {
                        //아니면 while문 끝나고 처리 하게 업데이트가 되지 않았다는 표시만 함
                        NextNodeHasUpdated = false;
                    }
#region 디버그를 편하게 하기 위해서 바로 업데이트 함
#if DEBUG
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

#endif
                    #endregion

                    EmptyNodeCount++;
                    return false;
                }

            }
            #endregion

            history.AddNode(current);

            for (int i = 0; i < 8; i++)
            {
                #region 가변 백트랙킹 기능 구현을 위한 코드
                //while (StackCount < HowManyExitStack)
                //{
                //    StackCount++;

                //    //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                //    current.WorkSapce = 0;

                //    current = history.GetPrevElement().node;

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
                    if (marker.SurchMarker[insertNow.current] == true)
                    {

                        hidatoCount = current.data;

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

                        NodeOfHidatoGridList.Add(current);

                        hidatoCount++;
                        current.WorkSapce = hidatoCount;
                        //history.AddNode(current);
                        //비어있는 칸이 1칸 줄어들기 때문에 1을 줄여야 합니다.
                        EmptyNodeCount--;
                        //이미 탐색한 자리임으로 false마킹
                        #region 이미 탐색한 자리는 false로 마킹합니다. 
                        if (insertNow.current == (int)side.N)
                        {
                            marker.SurchMarker[(int)side.N] = false;
                        }
                        else if (insertNow.current == (int)side.NE)
                        {
                            marker.SurchMarker[(int)side.NE] = false;
                        }
                        else if (insertNow.current == (int)side.E)
                        {
                            marker.SurchMarker[(int)side.E] = false;
                        }
                        else if (insertNow.current == (int)side.SE)
                        {
                            marker.SurchMarker[(int)side.SE] = false;
                        }
                        else if (insertNow.current == (int)side.S)
                        {
                            marker.SurchMarker[(int)side.S] = false;
                        }
                        else if (insertNow.current == (int)side.SW)
                        {
                            marker.SurchMarker[(int)side.SW] = false;
                        }
                        else if (insertNow.current == (int)side.W)
                        {
                            marker.SurchMarker[(int)side.W] = false;
                        }
                        else if (insertNow.current == (int)side.NW)
                        {
                            marker.SurchMarker[(int)side.NW] = false;
                        }
                        #endregion


                        #region 디버그를 편하게 하기 위해서 바로 업데이트 함
#if DEBUG
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

#endif
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
                        current.WorkSapce = 0;

                        if (current == history.currentNode.node)
                        {
                            //맨 윗부분의 이전노드가 아니라 
                            current = history.GetPrevElement().node;
                        }
                        else
                        {
                            //맨 윗부분 노드를 현재 노드로 설정함
                            current = history.GetCurrentElement().node;
                        }

                        //HidatoCount변수를 바꾼 현재노드의 데이터로 설정한 후
                        hidatoCount = current.data;

                        NextNode = FindNextNode(current);


                        #region 디버그를 편하게 하기 위해서 바로 업데이트 함
#if DEBUG
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

#endif
                        #endregion
                        EmptyNodeCount++;
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

        public void startsolve(Hidato_Board board)
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
            SmartSuch = true;
            EmptyNodeCount = maxval - HintCount();

            if (SmartSuch == true)
            {
                current = FindMinPossibe();
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

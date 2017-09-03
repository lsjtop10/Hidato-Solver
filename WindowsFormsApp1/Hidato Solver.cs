using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

        public class Node
        {
            public int data;
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

            public Node()
            {
                this.NE = null;
                this.E = null;
                this.SE = null;
                this.S = null;
                this.SE = null;
                this.W = null;
                this.NW = null;
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

        public void SetDataAt(int Yindex, int Xindex, int data)
        {
            HidatoGrid.Node taget = GetNodeAt(Yindex, Xindex);
            taget.data = data;
        }

        public int GetDataAt(int Yindex, int Xindex)
        {
            Node taget = GetNodeAt(Yindex, Xindex);
            int TagetData = taget.data;
            return TagetData;
        }

    }

    class HidatoSolver
    {
        private int hidatoCount = 1;
        private history history;
        private HidatoGrid m_hidatoGrid;
        private HidatoGrid.Node current;
        private int m_Rlength;
        private int m_Clength;
        private int maxval;
        private HidatoGrid.Node NextNode;
        private HidatoGrid.Node FirstNode;
        private HidatoGrid.Node PrevNode;
        private Stack<HidatoGrid.Node> HintStack = new Stack<HidatoGrid.Node>(); //현재까지 찾은 힌트들의 스택

        public int GridRlength
        {
            get { return m_Rlength; }
        }

        public int GridClength
        {
            get { return m_Clength; }
        }

        public HidatoSolver(int CLength, int RLength)
        {
            m_hidatoGrid = new HidatoGrid();
            m_hidatoGrid.GenerateGrid(CLength, RLength);
            m_Rlength = RLength;
            m_Clength = CLength;
        }

        private HidatoGrid.Node FindFirstNode()
        {
            for (int i = 0; i < GridClength; i++)
                for (int j = 0; j < GridRlength; j++)
                {
                    HidatoGrid.Node temp = m_hidatoGrid.GetNodeAt(i, j);
                    if (temp.data == 1)
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

            if (current.data == HidatoGrid.Node.unused)
            {
                return null;
            }

            for (int i = 0; i < GridClength; i++)
                for (int j = 0; j < GridRlength; j++)
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
            return nextnode;

        }

        private int FindMaxVal()
        {
            //최대값
            int maxval = 0;
            //현재값
            int nval = 0;

            for (int i = 0; i < GridClength; i++)
                for (int j = 0; j < GridRlength; j++)
                {
                    //현재 위치의 값
                    nval = GetDataAt(i, j);

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
                marker.SurchMarker[(int)side.W] = false;
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

        private bool solve()
        {
            ///디버그 하기 쉽게 출력을 시켜줌
            show();
            int DataOfCurrentNode = current.data;

            PrevNode = HintStack.Peek();

            bool PrevNodeIsPossible = ChekPrevPossibleVal(current);

            ///HintStack의 맨 위에 있는 노드의 데이타가 다음 노드의 데이타와 같지 않으면
            if (HintStack.Peek().data != NextNode.data)
            {
                ///현재 노드의 다음 노드를 찾아서 집어넣음
                HintStack.Push(FindNextNode(current));
                ///이전 노드는 힌트 스텍의 맨 위쪽
                PrevNode = HintStack.Peek();
            }

            SurchMarking marker = FindOption(current);

            ///현재 노드의 데이터가 다음 노드의 데이터보다 1이 작고, 다음 노드가 최대값이 아니면서
            if (current.data == NextNode.data - 1 && NextNode.data != maxval)
            {
                ///바로 이전 노드가 올바르면(붙어 있으면)
                if (PrevNodeIsPossible == true)
                {
                    //NextNode를 업데이트하고 현재 노드를 업데이트 전의 NextNode로 바꿈
                    current = NextNode;
                    hidatoCount = current.data;
                    NextNode = FindNextNode(current);
                    history.AddNode(current);
                    if (solve())
                    {
                        return true;
                    }
                }
                else //그렇지 않으면
                {
                    ///InsertCount를 +1하고,  

                    if (current.data != 1)
                    {
                        if (PrevNode.data > current.data)
                        {
                            HintStack.Pop();
                            PrevNode = HintStack.Peek();
                        }

                        //현재 노드가 힌트와 겹치면(히다토에서 미리 정해져 있는 상수)
                        if (current.data == PrevNode.data)
                        {
                            //바로 이전 노드로 되돌리기만함
                            current = history.GetPrevElement().node;
                        }
                        else
                        {
                            //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                            current.data = 0;
                            current = history.GetPrevElement().node;
                        }

                        //HidatoCount변수를 바꾼 현재노드의 데이터로 설정한 후
                        hidatoCount = current.data;

                        //NextNode변수를 업데이트 하고
                        NextNode = FindNextNode(current);

                        hidatoCount = current.data;

                        return false;
                    }
                }
            }
            else if (current.data == maxval - 1 && PrevNodeIsPossible == true)
            {
                return true;
            }

            if (current.data == NextNode.data - 1 && PrevNodeIsPossible == false)
            {
               
                if (current.data != 1)
                {
                    if (PrevNode.data > current.data)
                    {
                        HintStack.Pop();
                        PrevNode = HintStack.Peek();
                    }

                    //현재 노드가 힌트와 겹치면(히다토에서 미리 정해져 있는 상수)
                    if (current.data == PrevNode.data)
                    {
                        //바로 이전 노드로 되돌리기만함
                        current = history.GetPrevElement().node;
                    }
                    else
                    {
                        //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                        current.data = 0;
                        current = history.GetPrevElement().node;
                    }


                    //HidatoCount변수를 바꾼 현재노드의 데이터로 설정한 후
                    hidatoCount = current.data;

                    NextNode = FindNextNode(current);

                    return false;
                }
            }
        

            if ((current != NextNode) && (current.data == NextNode.data))
            {
                

                if (current.data != 1)
                {
                    if (PrevNode.data > current.data)
                    {
                        HintStack.Pop();
                        PrevNode = HintStack.Peek();
                    }

                    
                    current.data = 0;
                    current = history.GetPrevElement().node;
                    
                    hidatoCount = current.data;

                   
                    
                    NextNode = FindNextNode(current);
                    

                    hidatoCount = current.data;

                    return false;
                }

            }

            if (marker.sizeOfnumSet() == 0)
            {
               

                if (current.data != 1)
                {
                    //PrevNode변수는 HintStack에 가장 윗 부분이다.HintStack의 가장 윗부분은 NextNode임으로(다음 노드임으로) 
                    if (PrevNode.data > current.data)
                    {
                        //가장 윗부분을 덜어낸 다음
                        HintStack.Pop();
                        //덜어낸 스택의 가장 윗부분을 이전 노드로 설정
                        PrevNode = HintStack.Peek();
                    }

                    //현재 노드가 힌트와 겹치면(히다토에서 미리 정해져 있는 상수)
                    if (current.data == PrevNode.data)
                    {
                        //바로 이전 노드로 되돌리기만함
                        current = history.GetPrevElement().node;
                    }
                    else
                    {
                        //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                        current.data = 0;
                        current = history.GetPrevElement().node;
                    }

                    //HidatoCount변수를 바꾼 현재노드의 데이터로 설정한 후
                    hidatoCount = current.data;

                    //바로 NextNode를 업데이트 하고
                    NextNode = FindNextNode(current);

                    return false;
                }

            }

            //SurchMarking marker1 = ChekPrevNodeCorrectVal(NextNode);
            //8개의 인접한 칸에 뭐가 있으면서도(0이 아니면) 이전노드보다 1이 작지 않으면

            for (int i = 0; i < 8; i++)
            {
                if (marker.SurchMarker[(int)side.N] == true)
                {
                    //현재노드를 북쪽으로 설정한 다음
                    current = current.N;
                    hidatoCount++;
                    current.data = hidatoCount;
                    history.AddNode(current);
                    //이미 탐색한 자리임으로 false마킹
                    marker.SurchMarker[(int)side.N] = false;

                }
                else if (marker.SurchMarker[(int)side.NE] == true)
                {
                    current = current.NE;
                    hidatoCount++;
                    current.data = hidatoCount;
                    history.AddNode(current);
                    marker.SurchMarker[(int)side.NE] = false;
                }
                else if (marker.SurchMarker[(int)side.E] == true)
                {
                    current = current.E;
                    hidatoCount++;
                    current.data = hidatoCount;
                    history.AddNode(current);
                    marker.SurchMarker[(int)side.E] = false;
                }
                else if (marker.SurchMarker[(int)side.SE] == true)
                {
                    current = current.SE;
                    history.AddNode(current);
                    hidatoCount++;
                    current.data = hidatoCount;
                    marker.SurchMarker[(int)side.SE] = false;

                }
                else if (marker.SurchMarker[(int)side.S] == true)
                {
                    current = current.S;
                    hidatoCount++;
                    current.data = hidatoCount;
                    history.AddNode(current);
                    marker.SurchMarker[(int)side.S] = false;

                }
                else if (marker.SurchMarker[(int)side.SW] == true)
                {
                    current = current.SW;
                    history.AddNode(current);
                    hidatoCount++;
                    current.data = hidatoCount;
                    marker.SurchMarker[(int)side.SW] = false;
                }
                else if (marker.SurchMarker[(int)side.W] == true)
                {
                    //HowManyTry++;
                    current = current.W;
                    hidatoCount++;
                    current.data = hidatoCount;
                    history.AddNode(current);
                    marker.SurchMarker[(int)side.W] = false;
                }
                else if (marker.SurchMarker[(int)side.NW] == true)
                {
                    //HowManyTry++;
                    current = current.NW;
                    hidatoCount++;
                    current.data = hidatoCount;
                    history.AddNode(current);
                    marker.SurchMarker[(int)side.NW] = false;
                }
                else
                {
                    if (current.data != 1)
                    {
                        if (PrevNode.data > current.data)
                        {
                            HintStack.Pop();
                            PrevNode = HintStack.Peek();
                        }

                        //현재 노드가 힌트와 겹치면(히다토에서 미리 정해져 있는 상수)
                        if (current.data == PrevNode.data)
                        {
                            //바로 이전 노드로 되돌리기만함
                            current = history.GetPrevElement().node;
                        }
                        else
                        {
                            //아니면, 현재노드의 데이터를 0으로 설정하고,(비워놓음) 바로 이전 노드로 되돌림
                            current.data = 0;
                            current = history.GetPrevElement().node;
                        }

                        //HidatoCount변수를 바꾼 현재노드의 데이터로 설정한 후
                        hidatoCount = current.data;
                      
                        return false;
                    }
                    else
                    {
                        continue;
                    }

                }

                //if(solve())와 같은 표현
                if (solve() == true)
                {
                    return true;
                }

            }

            return false;
        }

        public bool startsolve()
        {
            current = FirstNode = FindFirstNode();
            HintStack.Push(FirstNode);
            maxval = FindMaxVal();
            history = new history();
            history.AddNode(current);
            NextNode = FindNextNode(current);
            HintStack.Push(NextNode);
            if (solve())
            {
                return true;
            }
            else
            {
                return false;

            }
        }

        public void show()
        {
            Console.WriteLine("");
            for (int i = 0; i < GridClength; i++)
            {
                Console.WriteLine("");
                for (int j = 0; j < GridRlength; j++)
                {
                    int data = GetDataAt(i, j);
                    Console.Write(" ");
                    Console.Write("{0}", data);
                }
            }

            Console.WriteLine("");
        }

        public void SetDataAt(int Yindex, int Xindex, int data)
        {
            HidatoGrid.Node taget = m_hidatoGrid.GetNodeAt(Yindex, Xindex);
            taget.data = data;
        }

        public int GetDataAt(int Yindex, int Xindex)
        {
            HidatoGrid.Node taget = m_hidatoGrid.GetNodeAt(Yindex, Xindex);
            int TagetData = taget.data;
            return TagetData;
        }

    }
}
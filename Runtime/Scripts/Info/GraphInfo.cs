using System.Collections.Generic;

namespace OmicronFSM
{
    public struct GraphInfo
    {
        public string GameObject;
        public StateInfo Selected;
        public List<ConnectionInfo> Previous;
        public List<ConnectionInfo> Next;
        public int? CurrentStateIndex;
        public int? PreviousStateIndex;
        public int? PreviousTransitionIndex;

        public static GraphInfo Empty => new GraphInfo() { Next = new List<ConnectionInfo>(), Previous = new List<ConnectionInfo>() };

        public void Clear()
        {
            GameObject = null;
            CurrentStateIndex = null;
            PreviousStateIndex = null;
            PreviousTransitionIndex = null;
            Previous.Clear();
            Next.Clear();
        }
    }
}
namespace OmicronFSM
{
    public class SameStateSelector : IStateSelector
    {
        private State _state;

        public SameStateSelector(State state)
        {
            _state = state;
        }

        public bool Valid(State state)
        {
            return _state == state;
        }
    }

}
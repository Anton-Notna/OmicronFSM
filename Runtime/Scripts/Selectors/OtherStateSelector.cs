namespace OmicronFSM
{
    public class OtherStateSelector : IStateSelector
    {
        private State _state;

        public OtherStateSelector(State state)
        {
            _state = state;
        }

        public bool Valid(State state)
        {
            return _state != state;
        }
    }

}
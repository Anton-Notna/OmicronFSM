namespace OmicronFSM
{
    public class AnySelector : IStateSelector
    {
        public bool Valid(State state) => true;
    }

}
namespace OmicronFSM
{
    public interface IStateSelector
    {
        public bool Valid(State state);
    }

}
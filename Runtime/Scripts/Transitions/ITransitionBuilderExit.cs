namespace OmicronFSM
{
    public interface ITransitionBuilderExit
    {
        public MachineBuilder To(IStateSelector state);
    }
}
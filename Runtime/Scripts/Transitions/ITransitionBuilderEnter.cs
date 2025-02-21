namespace OmicronFSM
{
    public interface ITransitionBuilderEnter
    {
        public ITransitionBuilderExit From(IStateSelector state);

        public ITransitionBuilderEnter Identify(IIdentifier identifier);
    }
}
namespace OmicronFSM
{
    public class IdentitySelector : IStateSelector
    {
        private IIdentifier _identifier;

        public IdentitySelector(IIdentifier identifier)
        {
            _identifier = identifier;
        }

        public bool Valid(State state)
        {
            if (state.Identifier == null)
                return false;

            return _identifier.Same(state.Identifier);
        }
    }

}
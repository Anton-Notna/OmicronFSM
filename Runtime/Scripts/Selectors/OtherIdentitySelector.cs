namespace OmicronFSM
{
    public class OtherIdentitySelector : IStateSelector
    {
        private IIdentifier _identifier;

        public OtherIdentitySelector(IIdentifier identifier)
        {
            _identifier = identifier;
        }

        public bool Valid(State state)
        {
            if (state.Identifier == null)
                return true;

            return _identifier.Same(state.Identifier) == false;
        }
    }

}
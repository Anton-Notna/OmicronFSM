using System.Collections.Generic;

namespace OmicronFSM
{
    public class AggregatorSelector : IStateSelector
    {
        private readonly List<IStateSelector> _innerSelectors;

        public AggregatorSelector(List<IStateSelector> innerSelectors)
        {
            _innerSelectors = innerSelectors;
        }

        public bool Valid(State state)
        {
            for (int i = 0; i < _innerSelectors.Count; i++)
            {
                if (_innerSelectors[i].Valid(state))
                    return true;
            }

            return false;
        }
    }

}
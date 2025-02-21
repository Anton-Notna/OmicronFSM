using System;

namespace OmicronFSM
{
    public class GenericCondition : Condition
    {
        private readonly Func<bool> _canTransit;

        public GenericCondition(Func<bool> canTransit) => _canTransit = canTransit;

        public override bool CanTransit() => _canTransit.Invoke();
    }
}
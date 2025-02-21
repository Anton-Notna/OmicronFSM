namespace OmicronFSM
{
    public class Transition
    {
        private readonly Condition _condition;
        private readonly State _next;

        public Transition(Condition condition, State next)
        {
            _condition = condition;
            _next = next;
        }

        public Condition Condition => _condition;

        public bool CanTransit() => _condition.CanTransit();

        public State Next() => _next;
    }
}
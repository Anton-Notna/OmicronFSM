using System;

namespace OmicronFSM
{
    public class GenericState : State
    {
        private readonly Action _enter;
        private readonly Action _update;
        private readonly Action _exit;

        public GenericState(Action enter, Action update, Action exit)
        {
            _enter = enter;
            _update = update;
            _exit = exit;
        }

        public GenericState(Action update)
        {
            _update = update;
        }

        public override void Enter() => _enter?.Invoke();

        public override void Update() => _update?.Invoke();

        public override void Exit() => _exit?.Invoke();
    }
}
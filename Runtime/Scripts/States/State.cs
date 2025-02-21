namespace OmicronFSM
{
    public abstract class State
    {
        internal IIdentifier Identifier { get; set; }

        protected Context Context { get; private set; }


        internal void Init(Context context)
        {
            Context = context;
            OnInited();
        }

        public virtual bool CanEnter() => true;

        public virtual bool CanExit() => true;

        public virtual void Enter() { }

        public virtual void Update() { }

        public virtual void Exit() { }

        public override string ToString() => Identifier == null ? GetType().Name : Identifier.Name;

        protected virtual void OnInited() { }
    }
}
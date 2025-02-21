namespace OmicronFSM
{
    public abstract class Condition
    {
        internal IIdentifier Identifier { get; set; }

        protected Context Context { get; private set; }

        internal void Init(Context context)
        {
            Context = context;
            OnInited();
        }

        public abstract bool CanTransit();

        public override string ToString() => Identifier == null ? GetType().Name : Identifier.Name;

        protected virtual void OnInited() { }
    }
}
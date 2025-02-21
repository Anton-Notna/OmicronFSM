using System.Collections.Generic;
using System.Text;

namespace OmicronFSM
{
    public class CombinedState : State
    {
        private readonly List<State> _combined = new List<State>();

        public CombinedState(params State[] toCombine)
        {
            for (int i = 0; i < toCombine.Length; i++)
                _combined.Add(toCombine[i]);
        }

        public CombinedState Combine(State toAdd)
        {
            _combined.Add(toAdd);
            return this;
        }

        protected override void OnInited()
        {
            for (int i = 0; i < _combined.Count; i++)
                _combined[i].Init(Context);
        }

        public override void Enter()
        {
            for (int i = 0; i < _combined.Count; i++)
                _combined[i].Enter();
        }

        public override void Update()
        {
            for (int i = 0; i < _combined.Count; i++)
                _combined[i].Update();
        }

        public override void Exit()
        {
            for (int i = 0; i < _combined.Count; i++)
                _combined[i].Exit();
        }

        public override string ToString()
        {
            if (Identifier != null)
                return Identifier.Name;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('[');
            for (int i = 0; i < _combined.Count; i++)
            {
                stringBuilder.Append(_combined[i].ToString());
                if (i < _combined.Count - 1)
                stringBuilder.Append(" + ");
            }
            stringBuilder.Append(']');

            return stringBuilder.ToString();
        }
    }
}
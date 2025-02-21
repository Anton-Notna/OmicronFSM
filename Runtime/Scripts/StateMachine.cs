using System;
using System.Collections.Generic;
using UnityEngine;

namespace OmicronFSM
{
    [Serializable]
    public class StateMachine
    {
        private readonly GameObject _owner;
        private readonly Dictionary<State, List<Transition>> _graph;
        private readonly List<State> _states;
        private readonly List<Transition> _transitions;
        private readonly State _enter;
        private Context _context;
        private State _current;
        private int _ticks;
        private State _previous;
        private Transition _previousTransition;

        public StateMachine(GameObject owner, Dictionary<State, List<Transition>> graph, State enter, List<State> allStates, List<Transition> allTransitions)
        {
            _owner = owner;
            _graph = graph;
            _states = allStates;
            _transitions = allTransitions;
            _enter = enter;
            Init();
        }

        public int GetStatesCount() => _states == null ? 0 : _states.Count;

        public int? GetCurrentStateIndex() => _current == null ? null : _states.IndexOf(_current);

        public void GetGraphInfo(ref GraphInfo info, int stateIndex)
        {
            info.Clear();

            if (_context == null)
                return;

            if (_context.GameObject == null)
                return;

            if (_previous != null)
                info.PreviousStateIndex = _states.IndexOf(_previous);

            if (_previousTransition != null)
                info.PreviousTransitionIndex = _transitions.IndexOf(_previousTransition);

            if (_current != null)
                info.CurrentStateIndex = _states.IndexOf(_current);

            info.Selected = GetStateInfo(stateIndex);

            if (_graph.TryGetValue(_states[stateIndex], out var nextTransitions))
            {
                for (int i = 0; i < nextTransitions.Count; i++)
                {
                    var transition = nextTransitions[i];
                    var connection = new ConnectionInfo()
                    {
                        State = GetStateInfo(_states.IndexOf(transition.Next())),
                        Condition = transition.Condition.ToString(),
                        TransitionIndex = _transitions.IndexOf(transition),
                    };

                    info.Next.Add(connection);
                }
            }

            foreach (var previousTransitions in _graph)
            {
                int previousStateIndex = _states.IndexOf(previousTransitions.Key);
                for (int i = 0; i < previousTransitions.Value.Count; i++)
                {
                    var previousTransition = previousTransitions.Value[i];
                    int nextStateIndex = _states.IndexOf(previousTransition.Next());
                    if (nextStateIndex != stateIndex)
                        continue;

                    var connection = new ConnectionInfo()
                    {
                        State = GetStateInfo(previousStateIndex),
                        Condition = previousTransition.Condition.ToString(),
                        TransitionIndex = _transitions.IndexOf(previousTransition),
                    };

                    info.Previous.Add(connection);
                }
            }

        }

        public StateInfo GetStateInfo(int index)
        {
            return new StateInfo()
            {
                Index = index,
                Info = _states[index].ToString(),
            };
        }

        public void Tick()
        {
            TickTransitions();
            TickCurrent();
            _ticks++;
        }

        public void Reset() 
        {
            _current?.Exit();
            _current = null;
            _previous = null;
            _previousTransition = null;
            _ticks = 0;
        } 

        private void Init()
        {
            _context = new Context(_owner);

            for (int i = 0; i < _states.Count; i++)
                _states[i].Init(_context);

            for (int i = 0; i < _transitions.Count; i++)
                _transitions[i].Condition.Init(_context);
        }

        private void TickCurrent()
        {
            _current.Update();
        }

        private void TickTransitions()
        {
            if (_ticks == 0)
            {
                _current = _enter;
                _current.Enter();
                return;
            }

            if (_current.CanExit() == false)
                return;

            if (_graph.TryGetValue(_current, out var transitions) == false)
                return;

            for (var i = 0; i < transitions.Count; i++)
            {
                var transition = transitions[i];
                if (transition.CanTransit() == false)
                    continue;

                if (transition.Next().CanEnter() == false)
                    continue;
                
                _previous = _current;
                _previousTransition = transition;

                ChangeCurrent(transition.Next());
                break;
            }
        }

        private void ChangeCurrent(State state)
        {
            _current.Exit();
            _current = state;
            _current.Enter();
        }
    }
}
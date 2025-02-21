using System;
using System.Collections.Generic;
using UnityEngine;

namespace OmicronFSM
{
    public class MachineBuilder
    {
        private readonly GameObject _owner;
        private readonly List<State> _states = new List<State>();
        private readonly List<RawTransition> _transitions = new List<RawTransition>();
        private readonly TransitionBuilder _transitionBuilder;
        private State _enter;

        public MachineBuilder(GameObject owner)
        {
            _owner = owner;
            _transitionBuilder = new TransitionBuilder(this);
        }

        public StateMachine Build()
        {
            if (_enter == null)
                throw new NullReferenceException("There is no Enter State");

            Dictionary<State, List<Transition>> graph = new Dictionary<State, List<Transition>>();
            List<State> states = new List<State>() { _enter };
            List<Transition> transitions = new List<Transition>();
            List<State> from = new List<State>();
            List<State> to = new List<State>();
            for (int i = 0; i < _transitions.Count; i++)
                AddTransitions(graph, _transitions[i], i, states, transitions, from, to);

            return new StateMachine(_owner, graph, _enter, states, transitions);
        }

        public MachineBuilder State(State state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

            if (_states.Contains(state))
                throw new ArgumentException($"{state} already in MachineBuilder");

            _states.Add(state);
            return this;
        }

        public MachineBuilder AsEnter()
        {
            if (_states.Count == 0)
                throw new InvalidOperationException("There is no states in MachineBuilder");

            if (_enter != null)
                throw new InvalidOperationException($"Enter state already set, enter: {_enter}");

            _enter = _states[_states.Count - 1];
            return this;
        }

        public MachineBuilder Identify(IIdentifier identifier)
        {
            if (_states.Count == 0)
                throw new InvalidOperationException("There is no states in MachineBuilder");

            ThrowIfIdentifierExists(identifier);

            _states[_states.Count - 1].Identifier = identifier;
            return this;
        }

        public ITransitionBuilderEnter Transit(Condition condition) => _transitionBuilder.Transit(condition);

        private void ThrowIfIdentifierExists(IIdentifier identifier)
        {
            if (identifier == null)
                return;

            for (int i = 0; i < _states.Count; i++)
            {
                var other = _states[i].Identifier;
                if (other == null)
                    continue;

                if (identifier.Same(other))
                    throw new InvalidOperationException($"Identifier {identifier.Name} already exists");
            }
        }

        private void AddTransitions(Dictionary<State, List<Transition>> graph, RawTransition rawTransition, int index, List<State> allStates, List<Transition> allTransitions, List<State> fromList, List<State> toList)
        {
            if (rawTransition.Ready == false)
                throw new InvalidOperationException("Wrong transition building order");

            FindStates(rawTransition.From, rawTransition.Condition, index, false, fromList);
            FindStates(rawTransition.To, rawTransition.Condition, index, true, toList);
            State to = toList[0];
            if (allStates.Contains(to) == false)
                allStates.Add(to);

            for (int i = 0; i < fromList.Count; i++)
            {
                State from = fromList[i];
                if (allStates.Contains(from) == false)
                    allStates.Add(from);

                if (graph.TryGetValue(from, out List<Transition> transitions) == false)
                {
                    transitions = new List<Transition>();
                    graph.Add(from, transitions);
                }

                var result = new Transition(rawTransition.Condition, to);
                transitions.Add(result);
                allTransitions.Add(result);
            }
        }

        private void FindStates(IStateSelector selector, Condition condition, int index, bool multipleCheck, List<State> result)
        {
            result.Clear();
            for (int i = 0; i < _states.Count; i++)
            {
                var state = _states[i];
                if (selector.Valid(state) == false)
                    continue;

                if (multipleCheck && result.Count > 0)
                    throw new InvalidOperationException($"Multiple states selection in {selector} selector, {condition} condition, transition index: {index}");

                result.Add(state);
            }

            if (result.Count == 0)
                throw new InvalidOperationException($"Cannot find state by {selector} selector, {condition} condition, transition index: {index}");
        }

        private class RawTransition
        {
            public IStateSelector From { get; set; }

            public IStateSelector To { get; set; }

            public Condition Condition { get; set; }

            public bool Ready => From != null && To != null && Condition != null;
        }

        private class TransitionBuilder : ITransitionBuilderEnter, ITransitionBuilderExit
        {
            private readonly MachineBuilder _owner;
            private RawTransition _current;

            public TransitionBuilder(MachineBuilder owner)
            {
                _owner = owner;
            }

            public ITransitionBuilderEnter Transit(Condition condition)
            {
                if (_current != null)
                    throw new InvalidOperationException("Wrong transition building order");

                _current = new RawTransition()
                {
                    Condition = condition,
                };

                return this;
            }

            public ITransitionBuilderEnter Identify(IIdentifier identifier)
            {
                if (_current == null)
                    throw new InvalidOperationException("Wrong transition building order");

                _current.Condition.Identifier = identifier;
                return this;
            }

            public ITransitionBuilderExit From(IStateSelector state)
            {
                if (_current == null)
                    throw new InvalidOperationException("Wrong transition building order");

                _current.From = state;
                return this;
            }

            public MachineBuilder To(IStateSelector state)
            {
                if (_current == null)
                    throw new InvalidOperationException("Wrong transition building order");

                _current.To = state;
                _owner._transitions.Add(_current);
                _current = null;

                return _owner;
            }
        }
    }
}
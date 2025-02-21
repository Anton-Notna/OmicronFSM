using System;
using System.Collections.Generic;

namespace OmicronFSM
{
    public static class MachineBuilderExtensions
    {
        public static MachineBuilder Named(this MachineBuilder builder, string name)
            => builder.Identify(new StringIdentifier(name));

        public static MachineBuilder Typed<T>(this MachineBuilder builder, T type) where T : Enum
            => builder.Identify(new EnumIdentifier<T>(type));

        public static MachineBuilder State(this MachineBuilder builder, State state, string name)
            => builder.State(state).Named(name);

        public static MachineBuilder State<T>(this MachineBuilder builder, State state, T type) where T : Enum
            => builder.State(state).Typed(type);

        public static MachineBuilder State(this MachineBuilder builder, Action onUpdate, string name)
            => builder.State(new GenericState(onUpdate)).Named(name);

        public static MachineBuilder State<T>(this MachineBuilder builder, Action onUpdate, T type) where T : Enum
            => builder.State(new GenericState(onUpdate)).Typed(type);

        public static MachineBuilder State(this MachineBuilder builder, Action onStart, Action onUpdate, Action onExit, string name)
            => builder.State(new GenericState(onStart, onUpdate, onExit)).Named(name);

        public static MachineBuilder State<T>(this MachineBuilder builder, Action onStart, Action onUpdate, Action onExit, T type) where T : Enum
            => builder.State(new GenericState(onStart, onUpdate, onExit)).Typed(type);

        public static MachineBuilder StateCombine(this MachineBuilder builder, params State[] states)
            => builder.State(new CombinedState(states));

        public static CombinedState Combine(this State state, State other)
            => new CombinedState(state, other);

        public static ITransitionBuilderEnter Named(this ITransitionBuilderEnter builder, string name)
            => builder.Identify(new StringIdentifier(name));

        public static ITransitionBuilderEnter Typed<T>(this ITransitionBuilderEnter builder, T type) where T : Enum
            => builder.Identify(new EnumIdentifier<T>(type));

        public static ITransitionBuilderEnter Transit(this MachineBuilder builder, Func<bool> condition)
            => builder.Transit(new GenericCondition(condition));

        public static ITransitionBuilderEnter Transit(this MachineBuilder builder, Func<bool> condition, string name)
            => builder.Transit(new GenericCondition(condition)).Named(name);

        public static ITransitionBuilderEnter Transit<T>(this MachineBuilder builder, Func<bool> condition, T type) where T : Enum
            => builder.Transit(new GenericCondition(condition)).Typed(type);

        public static ITransitionBuilderExit From(this ITransitionBuilderEnter builder, State state)
            => builder.From(new SameStateSelector(state));

        public static ITransitionBuilderExit From(this ITransitionBuilderEnter builder, string name)
            => builder.From(new IdentitySelector(new StringIdentifier(name)));

        public static ITransitionBuilderExit From<T>(this ITransitionBuilderEnter builder, T type) where T : Enum
            => builder.From(new IdentitySelector(new EnumIdentifier<T>(type)));

        public static ITransitionBuilderExit From(this ITransitionBuilderEnter builder, params State[] states)
        {
            List<IStateSelector> selectors = new List<IStateSelector>();
            for (int i = 0; i < states.Length; i++)
                selectors.Add(new SameStateSelector(states[i]));

            return builder.From(new AggregatorSelector(selectors));
        }

        public static ITransitionBuilderExit From(this ITransitionBuilderEnter builder, params string[] names)
        {
            List<IStateSelector> selectors = new List<IStateSelector>();
            for (int i = 0; i < names.Length; i++)
                selectors.Add(new IdentitySelector(new StringIdentifier(names[i])));

            return builder.From(new AggregatorSelector(selectors));
        }

        public static ITransitionBuilderExit From<T>(this ITransitionBuilderEnter builder, params T[] types) where T : Enum
        {
            List<IStateSelector> selectors = new List<IStateSelector>();
            for (int i = 0; i < types.Length; i++)
                selectors.Add(new IdentitySelector(new EnumIdentifier<T>(types[i])));

            return builder.From(new AggregatorSelector(selectors));
        }

        public static MachineBuilder To(this ITransitionBuilderExit builder, State state)
            => builder.To(new SameStateSelector(state));

        public static MachineBuilder To(this ITransitionBuilderExit builder, string name)
            => builder.To(new IdentitySelector(new StringIdentifier(name)));

        public static MachineBuilder To<T>(this ITransitionBuilderExit builder, T type) where T : Enum
            => builder.To(new IdentitySelector(new EnumIdentifier<T>(type)));

        public static ITransitionBuilderExit FromAny(this ITransitionBuilderEnter builder)
            => builder.From(new AnySelector());

        public static MachineBuilder FromOthersTo(this ITransitionBuilderEnter builder, State state)
            => builder.From(new OtherStateSelector(state)).To(state);

        public static MachineBuilder FromOthersTo(this ITransitionBuilderEnter builder, string name)
            => builder.From(new OtherIdentitySelector(new StringIdentifier(name))).To(name);

        public static MachineBuilder FromOthersTo<T>(this ITransitionBuilderEnter builder, T type) where T : Enum
            => builder.From(new OtherIdentitySelector(new EnumIdentifier<T>(type))).To(type);
    }
}
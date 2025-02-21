# Omicron Finite State Machine

https://github.com/user-attachments/assets/50a3b617-9abc-4974-b527-da0bc9eff606

The motivation for writing this repository is to create a flexible and simple FSM for Unity. 

This package involves writing your own code.

Inspired by [fluid-behavior-tree](https://github.com/ashblue/fluid-behavior-tree) and [fluid-state-machine](https://github.com/ashblue/fluid-state-machine) created by [ashblue](https://github.com/ashblue). I highly recomend to try his behaviour tree.

# Installation
Omicron FSM is the upm package, so the installation is similar to other upm packages:
1. Open `Window/PackageManager`
2. Click `+` in the right corner and select `Add package from git url...`
3. Paste the link to this package `https://github.com/Anton-Notna/OmicronFSM.git` and click `Add`

# Usage
## Quick start
Let's create a simple class to figure out how it all works:
```csharp
using UnityEngine;
using OmicronFSM;

public class OmicronFSMExample : MonoBehaviour
{
    [SerializeField]
    private StateMachine _machine; // There will be the "Display Graph" button in the Inspector Window
    [Space]
    [SerializeField]
    private int _enemies = 500;
    [SerializeField]
    private int _health = 10;

    private void Start()
    {
        _machine = new MachineBuilder(gameObject)
            .State(() => Debug.Log("I'm do nothing."), "Idle").AsEnter()
            .State(() => _enemies--, "Attack")
            .State(() => Debug.Log("I'm dead..."), "Dead")

            .Transit(() => _enemies > 0, "Enemies exists").From("Idle").To("Attack")
            .Transit(() => _enemies <= 0, "No enemies").From("Attack").To("Idle")
            .Transit(() => _health <= 0, "No life").FromOthersTo("Dead")

            .Build();
    }

    private void Update() => _machine.Tick();
}
```
We can run the game and click at the `Display Graph` button inside `OmicronFSMExample` Inspector to open a new window with the current state machine information.

As we can see, we have the `MachineBuilder` class, that provides a fluid-builder-like way to setup our `StateMachine`.
We setup 3 states: `Idle`, `Attack` and `Dead`. Next, we should setup transitions between states by calling `.Transit` method.
There are lot of ways how to add states and transitions, which we will see below.

Remember to call `.AsEnter()` in builder to set entry point state and `StateMachine.Tick()` to update our state machine at runtime.
## StateMachine class
There are only 2 methods that we need to know:
1. `StateMachine.Tick()` - An analog of `Update()` method in `MonoBehaviour` classes. It updates current transitions, transits between states (if it can) and updates current state.
2. `StateMachine.Reset()` - Exits current state and retuns `StateMachine` to after-build condition.
## States
### Generic states
We can create generic state like this:
```csharp
.State(() => { /*Calls every .Tick() when state is active*/ }, "SomeState")
```
Or use extended version of generic state with `Start()` and `Exit()` blocks:
```csharp
.State(
    () => { /*Calls on enter*/ },
    () => { /*Calls every .Tick() when state is active*/ },
    () => { /*Calls on exit*/ },
    "SomeState")
```
### Custom states
There is class called `State` that has empty virtual methods, which you can override or not:
1. `State.OnInited()` - Calls once inside `MachineBuilder.Build()` at every state in added order.
2. `State.Enter()`
3. `State.Update()`
4. `State.Exit()`

There is also `Context` field thats become available in `State.OnInited()`

Let's create a new custom state:
```csharp
public class MoveForwardState : State
{
    private readonly float _speed;

    public MoveForwardState(float speed) => _speed = speed;

    protected override void OnInited()
    {
        Debug.Log($"I'm on {Context.GameObject.name} GameObject!");
    }

    public override void Enter()
    {
        Debug.Log($"{Context.GameObject.name} Start moving!");
    }

    public override void Update()
    {
        Context.Transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    public override void Exit()
    {
        Debug.Log($"{Context.GameObject.name} Stop moving...");
    }
}
```
And register it inside builder:
```csharp
.State(new MoveForwardState(5f), "Move")
```
### Combine states
There is also an option to merge different states into one. Like this:
```csharp
.StateCombine(new SomeState(), new SomeState()).Named("Combined state")
```
Or this:
```csharp
State combined = new SomeState().Combine(new SomeState());
```
## Transitions and Conditions
As we know, FSM contains a graph of `states` and `transitions` between them. So, we need the way to create `transitions`. And here it is:

`.Transit([Condition], [Optional name]).From([State]).To([State])`

In `.Transit()` we can pass generic func:
```csharp
.Transit(() => _enemies > 0).From("Idle").To("Attack")
```
Or `Condition` class realization:
```csharp
public class WaitForTimeStamp : Condition
{
    private readonly float _availableTimeStamp;

    public WaitForTimeStamp(float availableTimeStamp) => _availableTimeStamp = availableTimeStamp;

    public override bool CanTransit() => Time.time >= _availableTimeStamp;
}
```
```csharp
.Transit(new WaitForTimeStamp(5f)).From("Sleep").To("Idle")
```

There are also some useful extension methods:
```csharp
.Transit(condition).From("State0", "State1", "State2").To("SomeState") // Transit from multiple states
.Transit(condition).FromOthersTo("SomeState") // Transit from other states
.Transit(condition).FromAny().To("SomeState") // Transit from all states
```
## Members Identification
Identifications of states and conditions are used to refer them in transitions and to display names in Display Window.

There are 3 ways to identify and refer states:
### Instance identification
Recommended way to use. Refer to states directly:
```csharp
private void Start()
{
    // Create states
    var idle = new GenericState(() => Debug.Log("I'm do nothing."));
    var attack = new GenericState(() => _enemies--);
    var dead = new GenericState(() => Debug.Log("I'm do dead..."));

    _machine = new MachineBuilder(gameObject)
        // Pass instances of states
        .State(idle).AsEnter()
        .State(attack)
        .State(dead)
        // Refer to instances in .From() and .To()
        .Transit(() => _enemies > 0).From(idle).To(attack)
        .Transit(() => _enemies <= 0).From(attack).To(idle)
        .Transit(() => _health <= 0).FromOthersTo(dead)

        .Build();
}
```
I also recommend to use names to identify states and transitions to get clear view what is going on in Display Window. So, the ideal usage (in my opinion) looks like this:
```csharp
private void Start()
{
    var idle = new GenericState(() => Debug.Log("I'm do nothing."));
    var attack = new GenericState(() => _enemies--);
    var dead = new GenericState(() => Debug.Log("I'm do dead..."));

    _machine = new MachineBuilder(gameObject)
        // Pass instances of states with names
        .State(idle, "Idle").AsEnter()
        .State(attack, "Attack")
        .State(dead, "Dead")

        .Transit(() => _enemies > 0, "Enemies exists").From(idle).To(attack)
        .Transit(() => _enemies <= 0, "No enemies").From(attack).To(idle)
        .Transit(() => _health <= 0, "No life").FromOthersTo(dead)

        .Build();
}
```
### String identification
In that case, the name of state is some sort of unique key that you can refer in transitions:
```csharp
private void Start()
{
    var attack = new GenericState(() => _enemies--);
    var dead = new GenericState(() => Debug.Log("I'm do dead..."));

    _machine = new MachineBuilder(gameObject)
        .State(() => Debug.Log("I'm do nothing."), "Idle").AsEnter() // Create generic and name it in one method
        .State(attack, "Attack") // Add new state and name it in one method
        .State(dead).Named("Dead") // Set state name in different method

        .Transit(() => _enemies > 0, "Enemies exists").From("Idle").To("Attack")
        .Transit(() => _enemies <= 0, "No enemies").From("Attack").To("Idle")
        .Transit(() => _health <= 0, "No life").FromOthersTo("Dead")

        .Build();
}
```
### Enum identification
Same as a previous way, but with enums:
```csharp
public enum StateType
{
    Idle,
    Attack,
    Dead,
}

private void Start()
{
    var attack = new GenericState(() => _enemies--);
    var dead = new GenericState(() => Debug.Log("I'm do dead..."));

    _machine = new MachineBuilder(gameObject)
        .State(() => Debug.Log("I'm do nothing."), StateType.Idle).AsEnter() // Create generic and type it in one method
        .State(attack, StateType.Attack) // Add new state and type it in one method
        .State(dead).Typed(StateType.Dead) // Set state type in different method

        .Transit(() => _enemies > 0).From(StateType.Idle).To(StateType.Attack)
        .Transit(() => _enemies <= 0).From(StateType.Attack).To(StateType.Idle)
        .Transit(() => _health <= 0).FromOthersTo(StateType.Dead)

        .Build();
}
```

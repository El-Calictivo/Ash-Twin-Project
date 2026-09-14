# AshTwinProject

AshTwinProject is a reusable Unity multiplayer package built on top of Netcode for GameObjects.

It provides utilities for transporting polymorphic C# objects across the network and synchronizing application-level systems such as state machines and event buses without requiring those systems themselves to understand Netcode serialization.

## Features

### Polymorphic Network Data

AshTwinProject provides network-friendly representations of regular C# objects through `INetworkable`.

```csharp
public interface INetworkable
{
    INetworkData GetNetworkRepresentation();
}
```

Objects implementing `INetworkable` can be converted into one of the available network payload structures:

```csharp
NetworkData32
NetworkData64
NetworkData128
NetworkData512
NetworkData4096
```

Each structure uses an appropriate Unity `FixedString` payload size while remaining compatible with Netcode's `INetworkSerializable`.

For example:

```csharp
public struct NetworkData128 : INetworkData
{
    public bool HasData;
    public uint TypeID;
    public FixedString128Bytes DataPayload;
}
```

An object can expose the representation appropriate for its expected payload size:

```csharp
public class ExampleState : ISyncedState
{
    public int Value;

    public INetworkData GetNetworkRepresentation()
    {
        return this.To128();
    }
}
```

## Polymorphic Deserialization

The concrete implementation of an interface does not need to be known by the receiver.

For example:

```csharp
ISyncedState state = networkData.ToObject<ISyncedState>();
```

The transmitted `TypeID` identifies the concrete type that should be instantiated.

This allows code to work with abstractions such as:

```csharp
IState
ISyncedState
IEvent
ISyncedEvent
```

while still reconstructing the correct concrete implementation.

## Network Type Registry

Every concrete `INetworkable` type is assigned a generated `uint` identifier.

For example:

```text
ExplorationState -> 382918271
CombatState      -> 1837264512
```

These IDs replace assembly-qualified CLR type names in network messages.

The registry provides two-way lookup:

```csharp
uint id = NetworkTypeRegistry.GetTypeId(instance);
Type type = NetworkTypeRegistry.GetType(id);
```

The registry generator automatically discovers valid public `INetworkable` implementations from runtime assemblies.

Internal, abstract, generic, editor-only, and test-only types are excluded from the generated runtime registry.

Generated IDs are persisted inside the consuming project under:

```csharp
Assets/Generated/AshTwinProject.Networking/
```

The generated ID database should be committed to source control so clients, servers, developers, and CI builds share the same network type IDs.

## Network Payload Sizes

AshTwinProject currently provides the following network representations:

```csharp
NetworkData32
NetworkData64
NetworkData128
NetworkData512
NetworkData4096
```

Choose the smallest representation capable of holding the serialized JSON payload.

For example:

```csharp
public INetworkData GetNetworkRepresentation()
{
    return this.To64();
}
```

Payload size should be measured using UTF-8 bytes rather than `string.Length`.

```csharp
int size = Encoding.UTF8.GetByteCount(json);
```

For persistent polymorphic state stored in a `NetworkVariable`, using `NetworkData4096` as the common representation may be preferable because a `NetworkVariable<T>` must have one concrete type for its lifetime.

## State Machine Synchronization

`StateMachineSyncService` synchronizes compatible state machines through a `NetworkVariable<NetworkData4096>`.

The server observes state transitions and writes synchronized states into the associated `NetworkVariable`.

Clients listen for changes, deserialize the concrete `ISyncedState`, and apply it to their local state machine.

A state must implement `ISyncedState` to participate in synchronization.

Conceptually:

```text
Server State Machine
        |
        v
ISyncedState
        |
        v
NetworkData4096
        |
        v
NetworkVariable
        |
        v
Client
        |
        v
Concrete ISyncedState
        |
        v
Client State Machine
```

State machines can synchronize either when a state change is requested or after a state has changed, depending on the selected binding mode.

## EventBus Synchronization

`EventBusSyncService` propagates synchronized EventBus events from the server to clients.

An event implementing `ISyncedEvent` can define its preferred network representation.

For example:

```csharp
public INetworkData GetNetworkRepresentation()
{
    return this.To64();
}
```

When the server dispatches the event:

```text
EventBus
   |
   v
ISyncedEvent
   |
   v
NetworkDataXX
   |
   v
ClientRpc
   |
   v
Client EventBus
```

The receiving client reconstructs the concrete event implementation and dispatches it through its local EventBus.

Regular events that do not implement `ISyncedEvent` remain local.

## Multiplayer Sessions

`SessionService` wraps Unity Multiplayer Services session operations.

It provides functionality for:

* Creating sessions.
* Joining sessions.
* Leaving the current session.
* Preventing multiple simultaneous session requests.

The service is designed to integrate with Nomai Framework's `IService` and `ServiceLocator`.

## Netcode Considerations

`INetworkData` is an application-level abstraction.

Netcode RPCs and `NetworkVariable<T>` still require concrete serializable types.

For example:

```csharp
[ClientRpc]
private void HandleEventClientRpc(NetworkData128 data) { }
```

is valid, while using:

```csharp
INetworkData
```

directly as an RPC parameter is not supported as a polymorphic Netcode payload.

The package handles this by providing concrete network representations while exposing a common abstraction to application code.

## Dependencies

AshTwinProject depends on:

* Unity Netcode for GameObjects
* Unity Collections
* Unity Multiplayer Services
* Newtonsoft Json
* UniTask
* Nomai Framework

Nomai Framework is used for systems such as:

* Services and `ServiceLocator`
* EventBus
* State management

## Usage Example

A synchronized state could look like:

```csharp
public class ExplorationState : ISyncedState
{
    public int CurrentRoom;

    public INetworkData GetNetworkRepresentation()
    {
        return this.To128();
    }
}
```

The server can convert it to network data:

```csharp
NetworkData128 data = explorationState.To128();
```

and the receiver can reconstruct it through its abstraction:

```csharp
ISyncedState state = data.ToObject<ISyncedState>();
```

The actual object returned will still be:

```csharp
ExplorationState
```

because the transmitted `TypeID` is resolved through `NetworkTypeRegistry`.

## Design Philosophy

AshTwinProject keeps networking concerns at the transport boundary.

Application systems can continue working with interfaces and regular C# objects while the package handles:

```csharp
Object
  -> serialization
  -> type identification
  -> Netcode-compatible representation
  -> transport
  -> reconstruction
  -> original abstraction
```

This keeps game architecture independent from the specific representation required by Netcode.

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/02d07a0e-1c7d-48a7-a9c8-ed97a7acbffc" />

# AshTwinProject

AshTwinProject is a Unity project used to develop, test, and maintain the `AshTwinProject` Unity package.

The repository acts as the development environment for the package and contains the Unity project, package source code, test assemblies, and supporting examples required to validate the package in isolation before importing it into another project.

## Requirements

* **Unity:** 6000.0 or newer
* **Package:** `com.payosky.ash-twin-project`
* **Current Version:** `0.2.0`

## Dependencies

Ash Twin Project requires Unity **6000.0** or newer.

| Dependency                 | Package ID                       |  Version |
| -------------------------- | -------------------------------- | -------: |
| Unity Test Framework       | `com.unity.test-framework`       |  `1.0.0` |
| Unity Collections          | `com.unity.collections`          |  `2.0.0` |
| Netcode for GameObjects    | `com.unity.netcode.gameobjects`  | `2.0.0` |
| Unity Multiplayer Services | `com.unity.services.multiplayer` |  `2.0.0` |
| UniTask                    | `com.cysharp.unitask`            | `2.0.0` |
| Nomai Framework            | `com.payosky.nomai-framework`    |  `0.2.0` |

## Installation

The reusable package lives inside this repository at:

```text
Packages/com.payosky.ash-twin-project
```

You can install it directly into another Unity project through Unity Package Manager.

### Install through Package Manager

In Unity:

1. Open `Window > Package Management > Package Manager`.
2. Click the `+` button.
3. Select `Install package from git URL`.
4. Enter:

```text
https://github.com/El-Calictivo/Ash-Twin-Project.git?path=/Packages/com.payosky.ash-twin-project
```

5. Click `Install`.

Unity supports the `?path=` syntax when the package is located inside a subfolder of a Git repository. The selected folder must contain the package's `package.json`.

### Install through `manifest.json`

You can also add the package directly to your project's:

```text
Packages/manifest.json
```

Add the following dependency:

```json
{
    "dependencies": {
        "com.payosky.ash-twin-project": "https://github.com/El-Calictivo/Ash-Twin-Project.git?path=/Packages/com.payosky.ash-twin-project"
    }
}
```

Keep the rest of your existing dependencies unchanged.

### Install a specific version

When releases or Git tags are available, you can lock the package to a specific version by appending the tag after the package path:

```text
https://github.com/El-Calictivo/Ash-Twin-Project.git?path=/Packages/com.payosky.ash-twin-project#v1.0.0
```

Unity requires the `?path=` portion to come before the `#revision`.

This is recommended for projects that should not automatically track changes from the repository's default branch.

## Repository Structure

A typical structure looks like:

```text
AshTwinProject/
├── Assets/
│   ├── Generated/
│   │   └── AshTwinProject.Networking/
│   │       ├── NetworkTypeIds.json
│   │       └── GeneratedNetworkTypeRegistry.g.cs
│   └── ...
│
├── Packages/
│   └── ...
│
└── ...
```

The `AshTwinProject` package contains the reusable runtime and editor code.

Generated network type information is stored under:

```text
Assets/Generated/AshTwinProject.Networking/
```

## Development

Open the repository as a Unity project.

The project is intended to be used as the development and testing environment for the package.

When adding functionality:

1. Implement reusable code inside the package.
2. Add or update unit tests.
3. Validate Netcode-specific behavior with PlayMode or multiplayer integration tests when necessary.
4. Keep generated networking data committed when it represents stable type IDs.

### PlayMode / Integration Tests

Used for behavior that requires actual Unity or Netcode runtime state.

Examples include:

* `NetworkVariable` synchronization.
* Client RPC delivery.
* Server/client state synchronization.
* Spawned `NetworkObject` behavior.
* Multiplayer session integration.

## Network Type Registry

Objects implementing `INetworkable` receive a generated stable numeric type ID.

The registry is generated automatically and is used instead of transmitting CLR type names such as:

```csharp
AshTwinProject.States.ExplorationState, Assembly-CSharp, ...
```

Network payloads can therefore use:

```csharp
TypeID: 382918271
Payload: {...}
```

The generated ID database is persisted so IDs remain stable across recompilations and builds.

The generated ID file should be committed to source control.

Do not regenerate IDs manually unless intentionally rebuilding the network protocol.

## Documentation

Detailed documentation and usage examples are available in the package [README](https://github.com/El-Calictivo/Ash-Twin-Project/blob/main/Packages/com.payosky.ash-twin-project/README.md
).

## Architecture Philosophy

The goal of this repository is to provide a reusable multiplayer architecture layer built on top of Unity and Netcode for GameObjects.

The package currently focuses on:

* Network-friendly polymorphic data serialization.
* Synchronization of state machines between server and clients.
* Synchronization of EventBus events through RPCs.
* Multiplayer session management using Unity Multiplayer Services.
* Integration with Nomai Framework services, state management, and event systems.

## Author

Created by **Payosky**.

Website: http://www.payosky.dev
GitHub: https://github.com/JuanDavidPF
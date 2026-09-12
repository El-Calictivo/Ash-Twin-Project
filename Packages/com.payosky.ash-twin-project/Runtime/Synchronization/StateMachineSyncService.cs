using System;
using System.Collections.Generic;
using AshTwinProject.Core;
using Cysharp.Threading.Tasks;
using NomaiFramework.Services;
using NomaiFramework.StateManagement;
using Unity.Netcode;
using UnityEngine;

namespace AshTwinProject.Synchronization
{
    /// <summary>
    ///     Provides functionality for synchronizing state machines across networked environments.
    /// </summary>
    public class StateMachineSyncService : NetworkBehaviour, IService
    {
        public virtual Type TypeSignature => typeof(StateMachineSyncService);
        private readonly Dictionary<IStateMachine, NetworkVariable<NetworkData4096>> _stateMachinesBound = new();
        private readonly Dictionary<IStateMachine, NetworkVariable<NetworkData4096>.OnValueChangedDelegate> _clientSyncListeners = new();

        public override void OnNetworkSpawn()
        {
            ServiceLocator.AddService(this);
        }

        public void OnServerStateChanged(IStateMachine stateMachine, IState _, IState newState)
        {
            if (!IsServer) return;
            if (newState is not ISyncedState syncedState || !_stateMachinesBound.TryGetValue(stateMachine, out NetworkVariable<NetworkData4096> networkVariable)) return;
            networkVariable.Value = syncedState.To4096();
#if DEBUG
            Debug.Log($"Requesting ServerState Sync: {newState.GetType().Name}\nPayload: {networkVariable.Value.DataPayload.Value}");
#endif
        }

        public void BindStateMachine(IStateMachine stateMachine, NetworkVariable<NetworkData4096> syncedState, BindingMode bindingMode = BindingMode.OnStateChangeRequested)
        {
            if (!_stateMachinesBound.TryAdd(stateMachine, syncedState)) return;

            if (IsServer) {
                switch (bindingMode) {
                    case BindingMode.OnStateChangeRequested:
                    default:
                        stateMachine.OnStateChangeRequested += OnServerStateChanged;
                        break;
                    case BindingMode.OnStateChanged:
                        stateMachine.OnStateChanged += OnServerStateChanged;
                        break;
                }
            }
            else {
                NetworkVariable<NetworkData4096>.OnValueChangedDelegate clientDelegate = (_, serverState) =>
                {
#if DEBUG
                    Debug.Log($"ClientState Sync Requested: {serverState.DataType.Value}\nPayload: {serverState.DataPayload.Value}");
#endif
                    stateMachine.SetState(serverState.ToObject<ISyncedState>()).Forget();
                };

                _clientSyncListeners.Add(stateMachine, clientDelegate);
                if (syncedState.Value.HasData) {
                    clientDelegate(syncedState.Value, syncedState.Value);
                }

                syncedState.OnValueChanged += clientDelegate;
            }
        }

        public void UnbindStateMachine(IStateMachine stateMachine)
        {
            if (!_stateMachinesBound.Remove(stateMachine, out NetworkVariable<NetworkData4096> syncedState)) return;

            if (IsServer) {
                stateMachine.OnStateChangeRequested -= OnServerStateChanged;
                stateMachine.OnStateChanged -= OnServerStateChanged;
            }
            else if (_clientSyncListeners.TryGetValue(stateMachine, out NetworkVariable<NetworkData4096>.OnValueChangedDelegate listener)) {
                syncedState.OnValueChanged -= listener;
            }
        }

        public override void OnNetworkDespawn()
        {
            ServiceLocator.RemoveService(this);

            foreach (IStateMachine stateMachine in new List<IStateMachine>(_stateMachinesBound.Keys)) {
                UnbindStateMachine(stateMachine);
            }

            _stateMachinesBound.Clear();
        }

        public enum BindingMode
        {
            OnStateChangeRequested,
            OnStateChanged
        }
    }
}
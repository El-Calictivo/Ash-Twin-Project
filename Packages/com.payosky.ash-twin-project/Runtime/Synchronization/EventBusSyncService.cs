using System;
using System.Collections.Generic;
using AshTwinProject.Core;
using NomaiFramework.EventBus;
using NomaiFramework.Services;
using NomaiFramework.StateManagement;
using Unity.Netcode;

namespace AshTwinProject.Synchronization
{
    /// <summary>
    ///     This service integrates with Unity's Netcode for GameObjects to propagate specific events
    ///     to clients when those events are raised on the server side.
    /// </summary>
    public class EventBusSyncService : NetworkBehaviour, IService
    {
        public virtual Type TypeSignature => typeof(EventBusSyncService);

        public override void OnNetworkSpawn()
        {
            ServiceLocator.AddService(this);
            ServiceLocator.GetService<EventBusService>().OnEventDispatched += HandleRaisedEvent;
        }

        public void HandleRaisedEvent(IEvent raisedEvent)
        {
            if (!IsServer || raisedEvent is not ISyncedEvent syncedEvent) return;

            switch (syncedEvent.GetNetworkRepresentation()) {
                case NetworkData32 networkData32:
                    HandleRaisedEventClientRPC(networkData32);
                    return;
                case NetworkData64 networkData64:
                    HandleRaisedEventClientRPC(networkData64);
                    return;
                case NetworkData128 networkData128:
                    HandleRaisedEventClientRPC(networkData128);
                    return;
                case NetworkData512 networkData512:
                    HandleRaisedEventClientRPC(networkData512);
                    return;
                case NetworkData4096 networkData4096:
                    HandleRaisedEventClientRPC(networkData4096);
                    return;
            }
        }

        [ClientRpc]
        private void HandleRaisedEventClientRPC(NetworkData32 networkEventData)
        {
            if (IsServer) return;
            ISyncedEvent serverEvent = networkEventData.ToObject<ISyncedEvent>();
            ServiceLocator.GetService<EventBusService>().Trigger(serverEvent);
        }

        [ClientRpc]
        private void HandleRaisedEventClientRPC(NetworkData64 networkEventData)
        {
            if (IsServer) return;
            ISyncedEvent serverEvent = networkEventData.ToObject<ISyncedEvent>();
            ServiceLocator.GetService<EventBusService>().Trigger(serverEvent);
        }

        [ClientRpc]
        private void HandleRaisedEventClientRPC(NetworkData128 networkEventData)
        {
            if (IsServer) return;
            ISyncedEvent serverEvent = networkEventData.ToObject<ISyncedEvent>();
            ServiceLocator.GetService<EventBusService>().Trigger(serverEvent);
        }

        [ClientRpc]
        private void HandleRaisedEventClientRPC(NetworkData512 networkEventData)
        {
            if (IsServer) return;
            ISyncedEvent serverEvent = networkEventData.ToObject<ISyncedEvent>();
            ServiceLocator.GetService<EventBusService>().Trigger(serverEvent);
        }

        [ClientRpc]
        private void HandleRaisedEventClientRPC(NetworkData4096 networkEventData)
        {
            if (IsServer) return;
            ISyncedEvent serverEvent = networkEventData.ToObject<ISyncedEvent>();
            ServiceLocator.GetService<EventBusService>().Trigger(serverEvent);
        }

        public override void OnNetworkDespawn()
        {
            ServiceLocator.RemoveService(this);
            if (ServiceLocator.TryGetService(out EventBusService eventService)) {
                eventService.OnEventDispatched -= HandleRaisedEvent;
            }
        }
    }
}
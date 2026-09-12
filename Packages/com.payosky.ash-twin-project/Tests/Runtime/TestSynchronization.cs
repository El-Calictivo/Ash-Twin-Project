using AshTwinProject.Core;
using AshTwinProject.Samples;
using AshTwinProject.Synchronization;
using Unity.Netcode;
using UnityEngine;

namespace AshTwinProject
{
    internal class TestSynchronization : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            SyncedStateExample syncedStateExample = new()
            {
                TestString = "This State should appear on deserialization"
            };

            SyncedEventExample syncedEventExample = new()
            {
                TestString = "This Event should appear on deserialization"
            };

            TestStateClientRpc(syncedStateExample.To128());
            TestEventClientRpc(syncedEventExample.To64());
        }

        [ClientRpc]
        public void TestStateClientRpc(NetworkData128 networkData)
        {
            if (networkData.ToObject<ISyncedState>() is SyncedStateExample syncedStateExample) {
                Debug.Log($"{networkData.TypeID} - {syncedStateExample.TestString}");
            }
        }

        [ClientRpc]
        public void TestEventClientRpc(NetworkData64 networkData)
        {
            if (networkData.ToObject<ISyncedEvent>() is SyncedEventExample syncedEventExample) {
                Debug.Log($"{networkData.TypeID} - {syncedEventExample.TestString}");
            }
        }
    }
}
using AshTwinProject.Core;
using AshTwinProject.Synchronization;
using Cysharp.Threading.Tasks;
using NomaiFramework.StateManagement;

namespace AshTwinProject.Samples
{
    public class SyncedStateExample : ISyncedState
    {
        public PlayerLoopTiming TickMode => PlayerLoopTiming.Update;
        public string TestString = string.Empty;

        public UniTask OnStateEnter(IStateMachine parentMachine)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStateExit()
        {
            return UniTask.CompletedTask;
        }

        public INetworkData GetNetworkRepresentation()
        {
            return this.To128();
        }
    }
}
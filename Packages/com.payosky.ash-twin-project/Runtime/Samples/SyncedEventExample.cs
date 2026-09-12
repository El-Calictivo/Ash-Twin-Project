using AshTwinProject.Core;
using AshTwinProject.Synchronization;

namespace AshTwinProject.Samples
{
    public class SyncedEventExample : ISyncedEvent
    {
        public string TestString = string.Empty;

        public INetworkData GetNetworkRepresentation()
        {
            return this.To64();
        }
    }
}
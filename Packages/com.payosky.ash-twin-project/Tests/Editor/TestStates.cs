using AshTwinProject.Core;

namespace AshTwinProject.Editor.Tests
{
    internal interface ITestState : INetworkable { }

    internal sealed class TestState : ITestState
    {
        public int Value;

        public INetworkData GetNetworkRepresentation()
        {
            return this.To32();
        }
    }

    internal sealed class OtherTestState : ITestState
    {
        public string Name;

        public INetworkData GetNetworkRepresentation()
        {
            return this.To64();
        }
    }

    internal sealed class UnregisteredState : ITestState
    {
        public INetworkData GetNetworkRepresentation()
        {
            return this.To32();
        }
    }

    internal interface IOtherNetworkable : INetworkable { }

    internal sealed class OtherNetworkable : IOtherNetworkable
    {
        public int Value;

        public INetworkData GetNetworkRepresentation()
        {
            return this.To32();
        }
    }
}
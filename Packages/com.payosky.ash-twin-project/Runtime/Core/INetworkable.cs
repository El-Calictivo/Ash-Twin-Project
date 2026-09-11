namespace AshTwinProject.Core
{
    /// <summary>
    /// This interface contracts classes to define the size for their network friendly representation.
    /// </summary>
    public interface INetworkable
    {
        INetworkData GetNetworkRepresentation();
    }
}
using AshTwinProject.Core;
using NomaiFramework.EventBus;

namespace AshTwinProject.Synchronization
{
    /// <summary>
    /// Represents an event synchronized across multiple systems or components
    /// within the application.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface are intended to integrate with the event bus
    /// system to ensure consistent communication and state synchronization.
    /// </remarks>
    public interface ISyncedEvent : IEvent, INetworkable { }
}
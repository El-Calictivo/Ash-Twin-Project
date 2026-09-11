using AshTwinProject.Core;
using NomaiFramework.StateManagement;

namespace AshTwinProject.Synchronization
{
    /// <summary>
    /// Represents a synchronized state within a state management system.
    /// </summary>
    /// <remarks>
    /// This interface is intended to be implemented by classes that
    /// require their state to be synchronized within a shared system or framework.
    /// It extends the base <see cref="IState"/> interface, inheriting core state-related functionality.
    /// </remarks>
    public interface ISyncedState : IState, INetworkable { }
}
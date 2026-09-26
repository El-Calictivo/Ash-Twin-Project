using System;
using Cysharp.Threading.Tasks;
using NomaiFramework.Services;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace AshTwinProject
{
    /// <summary>
    /// Provides services for managing multiplayer game sessions, such as creating, joining, and leaving sessions.
    /// </summary>
    [Serializable]
    public class SessionService : IService
    {
        public virtual Type TypeSignature => typeof(SessionService);
        public UniTaskCompletionSource<ISession> CurrentSession { get; private set; }

        public virtual async UniTask<ISession> CreateSession(SessionOptions options)
        {
            if (CurrentSession == null) {
                CurrentSession = new UniTaskCompletionSource<ISession>();
            }
            else {
#if UNITY_ENABLE_CHECKS
                Debug.LogWarning("A session is already joined.");
# endif
                return await CurrentSession.Task;
            }

            try {
                IHostSession session = await MultiplayerService.Instance.CreateSessionAsync(options);
#if DEBUG
                Debug.Log($"Created session: {session.Id}");
#endif
                CurrentSession.TrySetResult(session);
                return session;
            }
            catch (Exception e) {
                CurrentSession = null;
                Debug.LogException(e);
                return null;
            }
        }

        public virtual async UniTask<ISession> JoinSession(string sessionId, JoinSessionOptions options)
        {
            if (CurrentSession == null) {
                CurrentSession = new UniTaskCompletionSource<ISession>();
            }
            else {
#if UNITY_ENABLE_CHECKS
                Debug.LogWarning("A session is already joined.");
# endif
                return await CurrentSession.Task;
            }

            try {
                ISession session = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionId, options);
#if DEBUG
                Debug.Log($"Joined session: {session.Id}");
#endif
                CurrentSession.TrySetResult(session);
                return session;
            }
            catch (Exception e) {
                CurrentSession = null;
                Debug.LogException(e);
                return null;
            }
        }

        public virtual async UniTask LeaveCurrentSession()
        {
            if (CurrentSession == null) return;

            try {
                ISession currentSession = await CurrentSession.Task;
                await currentSession.LeaveAsync();
#if DEBUG
                Debug.Log($"Left session: {currentSession.Id}");
#endif
                CurrentSession = null;
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
        }
    }
}
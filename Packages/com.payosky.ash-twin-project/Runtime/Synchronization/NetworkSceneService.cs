using System;
using Cysharp.Threading.Tasks;
using NomaiFramework.Services;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AshTwinProject.Synchronization
{
    /// <summary>
    /// Provides functionality to load and unload scenes in a networked environment.
    /// </summary>
    /// <remarks>
    /// This service is designed to manage scenes in applications that leverage Unity's Netcode for GameObjects.
    /// It integrates with the Unity.Netcode.SceneManager and provides asynchronous scene management capabilities,
    /// including the ability to load or unload scenes based on network events.
    /// </remarks>
    public class NetworkSceneService : IService
    {
        public virtual Type TypeSignature => typeof(NetworkSceneService);

        public async UniTask<Scene> LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool setActive = false)
        {
            UniTaskCompletionSource<Scene> completionSource = new();

            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;

            SceneEventProgressStatus status = NetworkManager.Singleton.SceneManager.LoadScene(sceneName, loadSceneMode);

            if (status != SceneEventProgressStatus.Started) {
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
#if UNITY_ENABLE_CHECKS
                Debug.LogError($"Failed to start loading scene: {sceneName}. Status: {status}");
#endif
            }

            Scene scene = await completionSource.Task;
            await UniTask.WaitUntil(() =>
            {
                scene = SceneManager.GetSceneByName(sceneName);
                return scene.IsValid() && scene.isLoaded;
            });

            if (!setActive) return scene;
            if (!SceneManager.SetActiveScene(scene)) {
#if UNITY_ENABLE_CHECKS
                Debug.LogError($"Failed to set active scene: {sceneName}");
#endif
            }

            return scene;

            void OnSceneEvent(SceneEvent sceneEvent)
            {
                if (sceneEvent.SceneEventType != SceneEventType.LoadEventCompleted || sceneEvent.SceneName != sceneName) {
                    return;
                }

                NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
                completionSource.TrySetResult(sceneEvent.Scene);
            }
        }

        public async UniTask<Scene> UnloadScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);

#if UNITY_ENABLE_CHECKS
            if (!scene.IsValid() || !scene.isLoaded) {
                throw new Exception($"Scene is not loaded: {sceneName}");
            }
#endif
            UniTaskCompletionSource completionSource = new();
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            SceneEventProgressStatus status = NetworkManager.Singleton.SceneManager.UnloadScene(scene);

            if (status != SceneEventProgressStatus.Started) {
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;

#if UNITY_ENABLE_CHECKS
                Debug.LogError($"Failed to start unloading scene: {sceneName}. Status: {status}");
#endif
            }

            await completionSource.Task;
            await UniTask.Yield();
            return scene;

            void OnSceneEvent(SceneEvent sceneEvent)
            {
                if (sceneEvent.SceneEventType != SceneEventType.UnloadEventCompleted || sceneEvent.SceneName != sceneName) {
                    return;
                }

                NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
                completionSource.TrySetResult();
            }
        }
    }
}
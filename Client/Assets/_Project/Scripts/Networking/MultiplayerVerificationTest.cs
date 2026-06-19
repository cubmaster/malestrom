using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Networking
{
    public static class MultiplayerVerificationTest
    {
        const float TimeoutSeconds = 20f;

        public static void RunAsClient()
        {
            var go = new GameObject("VerificationRunner");
            go.AddComponent<VerificationBehaviour>();
        }

        sealed class VerificationBehaviour : MonoBehaviour
        {
            float _elapsed;
            bool _resolved;

            void Start()
            {
                var sessionManager = Object.FindFirstObjectByType<NetworkSessionManager>();
                if (sessionManager != null)
                {
                    sessionManager.StartClient();
                }
                else
                {
                    Debug.LogError("[Verification] NetworkSessionManager not found in scene");
                    Application.Quit(1);
                }
            }

            void Update()
            {
                if (_resolved)
                {
                    return;
                }

                _elapsed += Time.deltaTime;

                var nm = NetworkManager.Singleton;
                if (nm != null && nm.IsConnectedClient)
                {
                    var spawnedObjects = nm.SpawnManager.SpawnedObjectsList;
                    var playerShipCount = 0;
                    foreach (var obj in spawnedObjects)
                    {
                        if (obj.IsPlayerObject)
                        {
                            playerShipCount++;
                        }
                    }

                    if (playerShipCount >= 1)
                    {
                        Debug.Log($"[Verification] PASS — connected, local player ship spawned (total spawned: {playerShipCount})");
                        _resolved = true;
                        Application.Quit(0);
                        return;
                    }
                }

                if (_elapsed >= TimeoutSeconds)
                {
                    Debug.LogError("[Verification] FAIL — timed out waiting for connection/spawn");
                    _resolved = true;
                    Application.Quit(1);
                }
            }
        }
    }
}

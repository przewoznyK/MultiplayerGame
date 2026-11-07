using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

namespace Core
{
    public static class FishNetExtensions
    {
        public static T GetPooledInstantiated<T>(this NetworkManager manager, GameObject prefab, Vector3 position, Quaternion rotation, bool asServer) where T : NetworkBehaviour
        {
            NetworkObject netObj = manager.GetPooledInstantiated(prefab, position, rotation, asServer);

            return netObj.GetComponent<T>();
        }
    }
}
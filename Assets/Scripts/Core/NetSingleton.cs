using FishNet.Object;
using UnityEngine;

namespace Core
{
    public class NetSingleton<T> : NetworkBehaviour where T : Component
    {
        public static T Instance { get; private set; }
        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
        }
    }
}
using FishNet.Object;
using UnityEngine;

namespace Game
{
    public abstract class GamePlayerAttack : NetworkBehaviour
    {
        public abstract void Attack();
    }
}
using Game.Data;
using UnityEngine;

namespace Game
{
    public abstract class EquippableController : MonoBehaviour
    {
        protected ItemData BaseItemData;

        public virtual void Initialize(ItemData data)
        {
            BaseItemData = data;
        }

        public abstract void PlayPrimaryVisuals();
        public abstract void PerformServerLogic(GamePlayer owner);
    }
}
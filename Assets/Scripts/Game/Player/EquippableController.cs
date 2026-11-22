using Game.Data;
using UnityEngine;

namespace Game
{
    public abstract class EquippableController : MonoBehaviour
    {
        protected GamePlayer _player;
        protected ItemData _baseItemData;

        public virtual void Initialize(GamePlayer gamePlayer, ItemData data)
        {
            _player = gamePlayer;
            _baseItemData = data;
        }

        public abstract void PlayPrimaryUseVisuals();
        public abstract void PerformPrimaryUseLogic(GamePlayer owner);
        public virtual void PlayPrimaryReleaseVisuals() { }
        public virtual void PerformPrimaryReleaseLogic(GamePlayer owner) { }
        public virtual void PlayPrimaryCancelVisuals() { }
        public virtual void PerformPrimaryCancelLogic(GamePlayer owner) { }
    }
}
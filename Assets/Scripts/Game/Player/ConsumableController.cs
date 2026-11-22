using Game.Data;
using UnityEngine;

namespace Game
{
    public class ConsumableController : EquippableController
    {
        private ConsumableData _consumableData;

        public override void Initialize(GamePlayer gamePlayer, ItemData data)
        {
            base.Initialize(gamePlayer, data);

            _consumableData = data as ConsumableData;
        }

        public override void PlayPrimaryUseVisuals()
        {
            Debug.Log("Cosnuamble animation");
        }

        public override void PerformPrimaryUseLogic(GamePlayer owner)
        {
            if (owner.TryGetComponent(out GamePlayerHealth health))
                health.Heal(_consumableData.HealAmount);

            if (owner.TryGetComponent(out GamePlayerInventory inventory))
                inventory.ConsumeActiveItem();
        }
    }
}
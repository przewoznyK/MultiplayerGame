using Game.Data;

namespace Game
{
    public class ConsumableController : EquippableController
    {
        private ConsumableData _consumableData;

        public override void Initialize(ItemData data)
        {
            base.Initialize(data);

            _consumableData = data as ConsumableData;
        }

        public override void PlayPrimaryVisuals()
        {

        }

        public override void PerformServerLogic(GamePlayer owner)
        {
            if (owner.TryGetComponent(out GamePlayerHealth health))
                health.Heal(_consumableData.HealAmount);

            if (owner.TryGetComponent(out GamePlayerInventory inventory))
                inventory.ConsumeActiveItem();
        }
    }
}
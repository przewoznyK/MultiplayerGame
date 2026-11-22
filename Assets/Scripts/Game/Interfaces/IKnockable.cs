using UnityEngine;

namespace Game
{
    public interface IKnockable
    {
        void ApplyKnockback(Vector2 direction, float force);
    }
}
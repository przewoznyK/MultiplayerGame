using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ProjectileManager : NetSingleton<ProjectileManager>
    {
        private List<ArrowProjectile> _activeArrows = new();

        public void RegisterArrow(ArrowProjectile arrow)
        {
            _activeArrows.Add(arrow);
        }

        public void UnregisterArrow(ArrowProjectile arrow)
        {
            _activeArrows.Remove(arrow);
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            for (int i = _activeArrows.Count - 1; i >= 0; i--)
            {
                ArrowProjectile arrow = _activeArrows[i];

                if (arrow == null)
                {
                    _activeArrows.RemoveAt(i);
                    continue;
                }

                arrow.MoveArrow(dt);
            }
        }
    }
}
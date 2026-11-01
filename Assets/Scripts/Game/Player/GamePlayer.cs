using Global;
using UnityEngine;

namespace Game
{
    public class GamePlayer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bodySR;

        public void Initialize(int classIndex)
        {
            _bodySR.sprite = SpriteManager.Instance.GetClassSpriteById(classIndex);
        }
    }
}
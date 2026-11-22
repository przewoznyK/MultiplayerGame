using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class InteractorUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void SetAndActive(string text)
        {
            _text.text = text;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
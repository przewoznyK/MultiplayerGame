using System.Collections;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class ServerMessageItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private float _displayTime = 4f;

        public void Setup(string message)
        {
            _messageText.text = message;
            StartCoroutine(LifeCycleRoutine());
        }

        private IEnumerator LifeCycleRoutine()
        {
            yield return new WaitForSeconds(_displayTime);
            Destroy(gameObject);
        }
    }
}
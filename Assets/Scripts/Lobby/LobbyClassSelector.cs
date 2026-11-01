using Global;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyClassSelector : MonoBehaviour
    {
        [SerializeField] private Image _classImage;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        private PlayerLobby _targetPlayer;

        public void Setup(PlayerLobby player)
        {
            _targetPlayer = player;

            if (_targetPlayer.IsOwner)
            {
                _leftButton.gameObject.SetActive(true);
                _rightButton.gameObject.SetActive(true);

                _leftButton.onClick.AddListener(() => _targetPlayer.CmdChangeClass(-1, SpriteManager.Instance.GetClassCount()));
                _rightButton.onClick.AddListener(() => _targetPlayer.CmdChangeClass(1, SpriteManager.Instance.GetClassCount()));
            }
            else
            {
                _leftButton.gameObject.SetActive(false);
                _rightButton.gameObject.SetActive(false);
            }

            _targetPlayer.ClassIndex.OnChange += OnClassIndexChanged;
            UpdateClassVisual(_targetPlayer.ClassIndex.Value);
        }

        private void OnDestroy()
        {
            if (_targetPlayer != null)
                _targetPlayer.ClassIndex.OnChange -= OnClassIndexChanged;
        }

        private void OnClassIndexChanged(int prevIndex, int newIndex, bool asServer)
        {
            UpdateClassVisual(newIndex);
        }

        private void UpdateClassVisual(int index)
        {
            _classImage.sprite = SpriteManager.Instance.GetClassSpriteById(index);
        }
    }
}
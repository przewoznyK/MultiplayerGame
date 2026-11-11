using Global;
using Global.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyClassSelector : MonoBehaviour
    {
        [SerializeField] private Image _classImage;
        [SerializeField] private TextMeshProUGUI _classNameText;
        [SerializeField] private TextMeshProUGUI _classBuffText;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        private LobbyPlayerPawn _targetPlayer;

        public void Setup(LobbyPlayerPawn player)
        {
            _targetPlayer = player;

            if (_targetPlayer.IsOwner)
            {
                _leftButton.gameObject.SetActive(true);
                _rightButton.gameObject.SetActive(true);

                _leftButton.onClick.AddListener(() => _targetPlayer.ChangeClass(-1, PlayerClassManager.Instance.GetClassCount()));
                _rightButton.onClick.AddListener(() => _targetPlayer.ChangeClass(1, PlayerClassManager.Instance.GetClassCount()));
            }
            else
            {
                _leftButton.gameObject.SetActive(false);
                _rightButton.gameObject.SetActive(false);
            }

            _targetPlayer.SessionData.ClassIndex.OnChange += OnClassIndexChanged;
            UpdateClassVisual(_targetPlayer.SessionData.ClassIndex.Value);
        }

        private void OnDestroy()
        {
            if (_targetPlayer != null)
                _targetPlayer.SessionData.ClassIndex.OnChange -= OnClassIndexChanged;
        }

        private void OnClassIndexChanged(int prevIndex, int newIndex, bool asServer)
        {
            UpdateClassVisual(newIndex);
        }

        private void UpdateClassVisual(int index)
        {
            PlayerClassData classData = PlayerClassManager.Instance.GetClassById(index);

            if (classData != null)
            {
                _classImage.sprite = classData.ClassSprite;
                _classNameText.text = classData.ClassType.ToString();
                _classBuffText.text = classData.ClassBuffDescription;
            }
        }
    }
}
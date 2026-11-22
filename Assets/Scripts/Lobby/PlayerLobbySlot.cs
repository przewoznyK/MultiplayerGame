using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class PlayerLobbySlot : MonoBehaviour
    {
        public LobbyPlayerPawn PlayerData { get; private set; }
        [SerializeField] private TextMeshProUGUI _playerNameText;
        [SerializeField] private RawImage _playerIcon;
        [SerializeField] private Image _readyStatusImage;
        [SerializeField] private Color _readyColor = Color.green;
        [SerializeField] private Color _notReadyColor = Color.red;
        private bool _avatarReceived;

        protected Callback<AvatarImageLoaded_t> ImageLoaded;

        private void Start()
        {
            ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
        }
        private void OnDestroy()
        {
            if (PlayerData != null)
            {
                PlayerData.SessionData.PlayerName.OnChange -= OnNameChanged;
                PlayerData.IsReady.OnChange -= OnReadyStateChanged;
            }
        }

        public void Setup(LobbyPlayerPawn player)
        {
            PlayerData = player;
            PlayerData.SessionData.PlayerName.OnChange += OnNameChanged;
            PlayerData.IsReady.OnChange += OnReadyStateChanged;
            RefreshUI();

            if (_avatarReceived == false)
                GetPlayerIcon();
        }

        public void RefreshUI()
        {
            if (PlayerData == null)
                return;

            _playerNameText.text = PlayerData.SessionData.PlayerName.Value;
            _readyStatusImage.color = PlayerData.IsReady.Value ? _readyColor : _notReadyColor;
        }


        private void OnNameChanged(string oldValue, string newValue, bool asServer)
        {
            RefreshUI();
        }

        private void OnReadyStateChanged(bool oldValue, bool newValue, bool asServer)
        {
            RefreshUI();
        }

        private void GetPlayerIcon()
        {
            int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerData.SessionData.PlayerSteamID.Value);
            if (ImageID == -1) 
                return;

            _playerIcon.texture = GetSteamImageAsTexture(ImageID);
        }

        private void OnImageLoaded(AvatarImageLoaded_t callback)
        {
            if (PlayerData != null && callback.m_steamID.m_SteamID == PlayerData.SessionData.PlayerSteamID.Value)
            {
                Texture2D avatar = GetSteamImageAsTexture(callback.m_iImage);
                _playerIcon.texture = avatar;
                _avatarReceived = true;
            }
        }

        private Texture2D GetSteamImageAsTexture(int iImage)
        {
            bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
            if (isValid == false || width == 0 || height == 0) 
                return null;

            byte[] imageData = new byte[width * height * 4];
            isValid = SteamUtils.GetImageRGBA(iImage, imageData, (int)(width * height * 4));

            if (isValid == false) 
                return null;

            Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
            texture.LoadRawTextureData(imageData);
            texture.Apply();
            return texture;
        }
    }
}
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private RawImage _playerIcon;
    private bool _avatarReceived;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == PlayerSteamID)
        {
            Texture2D avatar = GetSteamImageAsTexture(callback.m_iImage);
            _playerIcon.texture = avatar;
            _avatarReceived = true;
        }
        else
        {
            return;
        }
    }

    public void SetPlayerValues()
    {
        _playerNameText.text = PlayerName;
        if(!_avatarReceived)
        {
            GetPlayerIcon();
        }
    }

    private void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if (ImageID == -1)
        {
            return;
        }

        _playerIcon.texture = GetSteamImageAsTexture(ImageID);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;
        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (!isValid || width == 0 || height == 0)
        {
            Debug.LogWarning("Failed to get image size.");
            return null;
        }

        byte[] imageData = new byte[width * height * 4];
        isValid = SteamUtils.GetImageRGBA(iImage, imageData, (int)(width * height * 4));
        if (!isValid)
        {
            Debug.LogWarning("Failed to get image RGBA data.");
            return null;
        }

        texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
        texture.LoadRawTextureData(imageData);
        texture.Apply();
        return texture;
    }
}
using UnityEngine;

namespace Game
{
    public interface IInteractable
    {
        string GetInteractionPrompt();
        void OnServerInteract(GameObject interactor);
    }
}
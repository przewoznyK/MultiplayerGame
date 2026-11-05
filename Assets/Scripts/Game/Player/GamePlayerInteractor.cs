using FishNet.Object;
using Game.UI;
using UnityEngine;

namespace Game
{
    public class GamePlayerInteractor : NetworkBehaviour
    {
        [SerializeField] private float _interactionRadius = 1.5f;
        [SerializeField] private float _maxInteractDistance = 2.5f;
        [SerializeField] private LayerMask _interactableLayer;

        private IInteractable _closestInteractable;
        private NetworkObject _closestNetObj;
        private IInteractable _previousInteractable;

        public void RequestInteract()
        {
            if (_closestInteractable != null)
                ServerInteract(_closestNetObj);
        }

        private void Update()
        {
            if (IsOwner == false)
                return;

            FindClosestInteractable();

            if (_closestInteractable != _previousInteractable)
            {
                UpdateInteractionUI();
                _previousInteractable = _closestInteractable;
            }
        }

        private void FindClosestInteractable()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _interactionRadius, _interactableLayer);

            float minDistance = float.MaxValue;
            _closestInteractable = null;
            _closestNetObj = null;

            foreach (Collider2D col in colliders)
            {
                if (col.TryGetComponent(out IInteractable interactable) && col.TryGetComponent(out NetworkObject netObj))
                {
                    float dist = Vector2.Distance(transform.position, col.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        _closestInteractable = interactable;
                        _closestNetObj = netObj;
                    }
                }
            }
        }

        private void UpdateInteractionUI()
        {
            if (_closestInteractable != null)
            {
                string prompt = _closestInteractable.GetInteractionPrompt();
                GameUIManager.Instance.InteractorUI.SetAndActive(prompt);
            }
            else
                GameUIManager.Instance.InteractorUI.Deactivate();
        }

        [ServerRpc]
        private void ServerInteract(NetworkObject targetNetObj)
        {
            if (targetNetObj == null || targetNetObj.IsSpawned == false) 
                return;

            float distance = Vector2.Distance(transform.position, targetNetObj.transform.position);
            if (distance > _maxInteractDistance) 
                return;

            if (targetNetObj.TryGetComponent(out IInteractable interactable))
                interactable.OnServerInteract(gameObject);
        }
    }
}
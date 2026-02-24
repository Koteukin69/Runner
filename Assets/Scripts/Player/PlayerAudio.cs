using UnityEngine;

namespace Player
{
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField] PlayerMovement _playerMovement;
        [SerializeField] AudioSource _audioSource;
        [SerializeField, HideInInspector] float _jumpClipLength = 1f;
        [SerializeField, HideInInspector] float _slideClipLength = 1f;

        private void OnValidate()
        {
            if (!_playerMovement) TryGetComponent(out _playerMovement);
            if (!_audioSource) TryGetComponent(out _audioSource);
            
        }

        private void Start()
        {
            _playerMovement.OnJump += Jump;
            _playerMovement.OnRoll += Slide;
            GameManager.OnDie += Die;
        }

        private void Jump(float duration)
        {
            
        }

        private void Slide(float duration)
        {
            
        }

        private void Die()
        {
            
        }
    }
}

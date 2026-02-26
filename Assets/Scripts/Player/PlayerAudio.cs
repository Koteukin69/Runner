using System;
using UnityEngine;

namespace Player
{
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField] PlayerMovement _playerMovement;
        [SerializeField] AudioSource _audioSource;

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
            throw new NotImplementedException();
        }

        private void Slide(float duration)
        {
            throw new NotImplementedException();
        }

        private void Die()
        {
            throw new NotImplementedException();
        }
    }
}

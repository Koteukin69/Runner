using System;
using UnityEngine;

namespace Player
{
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField] PlayerMovement _playerMovement;
        [SerializeField] AudioSource _audioSource;

        [SerializeField] AudioClip _moveClip;
        [SerializeField] AudioClip _jumpClip;
        [SerializeField] AudioClip _slideClip;
        [SerializeField] AudioClip _dieClip;
        [SerializeField] AudioClip _coinCollectClip;
        
        private void OnValidate()
        {
            if (!_playerMovement) TryGetComponent(out _playerMovement);
            if (!_audioSource) TryGetComponent(out _audioSource);
            
        }

        private void Start()
        {
            _playerMovement.OnMove += _ => PlaySoundIfExits(_moveClip);
            _playerMovement.OnJump += _ => PlaySoundIfExits(_jumpClip);
            _playerMovement.OnRoll += _ => PlaySoundIfExits(_slideClip);
            GameManager.CoinsManager.OnCoinsChange += _ => PlaySoundIfExits(_coinCollectClip);
            GameManager.OnDie += () => PlaySoundIfExits(_dieClip);
        }

        private void PlaySoundIfExits(AudioClip clip)
        {
            if (!clip) return;
            _audioSource.PlayOneShot(clip);
        }
    }
}

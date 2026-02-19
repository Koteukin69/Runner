using System;
using Level;
using UnityEngine;

namespace Player
{
    public class CoinsCollisions : MonoBehaviour
    {
        [SerializeField] private PlayerCollision _playerCollision;

        private void OnValidate()
        {
            if (!_playerCollision) TryGetComponent(out _playerCollision);
        }

        private void Awake()
        {
            if (!_playerCollision) throw new MissingFieldException(nameof(_playerCollision));
            _playerCollision.OnCollision += TryCollideCoin;
        }

        private void OnDestroy()
        {
            if (_playerCollision) _playerCollision.OnCollision -= TryCollideCoin;
        }

        private void TryCollideCoin(LevelObjectType type)
        {
            if (type != LevelObjectType.Coin) return;
            GameManager.CoinsManager.AddCoins();
        }
    }
}
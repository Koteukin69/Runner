using System;
using Level;
using UnityEngine;

namespace Player
{
    public class ObstaclesCollisions : MonoBehaviour
    {
        [SerializeField] private PlayerCollision _playerCollision;
        [SerializeField] private PlayerMovement _playerMovement;

        private void OnValidate()
        {
            if (!_playerCollision) TryGetComponent(out _playerCollision);
            if (!_playerMovement) TryGetComponent(out _playerMovement);
        }

        private void Awake()
        {
            if (!_playerCollision) throw new MissingFieldException(nameof(_playerCollision));
            _playerCollision.OnCollision += TryCollideObstacle;
        }

        private void OnDestroy()
        {
            if (_playerCollision) _playerCollision.OnCollision -= TryCollideObstacle;
        }

        private void TryCollideObstacle(LevelObjectType type)
        {
            if (!_playerMovement) throw new MissingFieldException(nameof(_playerCollision));
            switch (type)
            {
                case LevelObjectType.Barrier:
                    GameManager.OnDie?.Invoke();
                    break;
                case LevelObjectType.Jumpable:
                    if (_playerMovement.IsJumping) break;
                    GameManager.OnDie?.Invoke();
                    break;
                case LevelObjectType.Rollable:
                    if (_playerMovement.IsRolling) break;
                    GameManager.OnDie?.Invoke();
                    break;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Animations;
using Level;
using UnityEngine;

namespace Player
{
    public class PlayerCollision : MonoBehaviour
    {
        [Serializable]
        public struct CollisionSettings
        {
            public LevelObjectType Type;
            [Min(0)] public float Distance;
            public bool SingleUse;
        }

        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private List<CollisionSettings> _settings = new()
        {
            new() { Type = LevelObjectType.Coin, Distance = 0.5f, SingleUse = true },
            new() { Type = LevelObjectType.Barrier, Distance = 0.5f, SingleUse = true },
            new() { Type = LevelObjectType.Jumpable, Distance = 0.5f, SingleUse = true },
            new() { Type = LevelObjectType.Rollable, Distance = 0.5f, SingleUse = true },
        };

        public Action<LevelObjectType> OnCollision;

        private Dictionary<LevelObjectType, CollisionSettings> _settingsMap;
        private readonly HashSet<GameObject> _collided = new();
        private LevelChunk _lastChunk;

        private void OnValidate()
        {
            if (!_playerMovement) TryGetComponent(out _playerMovement);
        }

        private void Start()
        {
            if (!_playerMovement)
                _playerMovement = GetComponent<PlayerMovement>();

            _settingsMap = new Dictionary<LevelObjectType, CollisionSettings>();
            foreach (var entry in _settings)
                _settingsMap[entry.Type] = entry;
        }

        private void FixedUpdate()
        {
            var levelManager = GameManager.LevelManager;
            float playerZ = transform.position.z;

            LevelChunk chunk = levelManager.GetChunkAt(playerZ);
            if (chunk == null) return;

            if (chunk != _lastChunk)
            {
                _collided.Clear();
                _lastChunk = chunk;
            }

            uint lines = GameManager.Lines;
            uint chunkSize = GameManager.ChunkSize;
            int lane = _playerMovement.Lane;
            float rowSpacing = levelManager.RowSpacing;

            for (int row = 0; row < chunkSize; row++)
            {
                int index = (int)(row * lines) + lane;
                var cell = chunk.Cells[index];

                if (cell.Template == null) continue;
                if (cell.SpawnedObject == null) continue;
                if (_collided.Contains(cell.SpawnedObject)) continue;

                float cellZ = (chunk.Position + row) * rowSpacing;
                LevelObjectType type = cell.Template.Type;

                if (!_settingsMap.TryGetValue(type, out var settings)) continue;

                if (Mathf.Abs(playerZ - cellZ) <= settings.Distance)
                {
                    if (settings.SingleUse)
                        _collided.Add(cell.SpawnedObject);

                    OnCollision?.Invoke(type);

                    cell.SpawnedObject.TryGetComponent(out ICollidable collidable);
                    collidable?.OnCollide();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField, Min(0.001f)] private float _rowSpacing = 1f;
        [SerializeField, Min(0)] private uint _gapRows = 2;
        [SerializeField, Min(1)] private uint _chunksAhead = 3;
        [SerializeField, Min(0)] private uint _startOffset = 5;

        [Header("References")]
        [SerializeField] private Transform _playerTransform;

        private List<LevelObject[]> _templates;
        private int _nextTemplateIndex;
        private readonly Queue<LevelChunk> _spawnedChunks = new();
        private Dictionary<GameObject, ObstaclePool> _pools;
        private uint _nextChunkPosition;
        private bool _initialized;
        private bool _spawnFinished;
        private bool _finished;
        public event Action<uint> OnFinishSpawn;

        public void Initialize(ILevelProvider provider)
        {
            if (_initialized) throw new InvalidOperationException("LevelManager is already initialized.");
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            if (!_playerTransform) throw new MissingFieldException(nameof(_playerTransform));

            _templates = provider.GetTemplates();

            if (_templates == null) throw new InvalidOperationException("Provider returned null templates.");

            uint expectedLength = GameManager.Lines * GameManager.ChunkSize;
            for (int i = 0; i < _templates.Count; i++)
                if (_templates[i] == null || _templates[i].Length != expectedLength)
                    throw new InvalidOperationException($"Template {i} has invalid length.");

            _spawnFinished = false;
            _finished = false;
            BuildPools();
            SpawnInitialChunks();
            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized) return;
            RecyclePassedChunks();
        }

        private void OnDestroy()
        {
            if (!_initialized) return;

            var poolContainer = transform.Find("ObstaclePools");
            if (poolContainer) Destroy(poolContainer.gameObject);

            _spawnedChunks.Clear();
            _pools?.Clear();
            _initialized = false;
        }

        private void BuildPools()
        {
            var poolSizes = CalculatePoolSizes();
            var container = new GameObject("ObstaclePools").transform;
            container.SetParent(transform);

            _pools = new Dictionary<GameObject, ObstaclePool>();
            foreach (var kv in poolSizes)
                _pools[kv.Key] = new ObstaclePool(kv.Key, kv.Value, container);
        }

        private Dictionary<GameObject, int> CalculatePoolSizes()
        {
            int window = (int) _chunksAhead;
            var maxCounts = new Dictionary<GameObject, int>();

            if (_templates.Count <= window)
            {
                foreach (var template in _templates)
                    AccumulateCounts(template, maxCounts, true);
                return maxCounts;
            }

            var windowCounts = new Dictionary<GameObject, int>();

            for (int i = 0; i < window; i++)
                AccumulateCounts(_templates[i], windowCounts, true);
            CopyMaxima(windowCounts, maxCounts);

            for (int i = window; i < _templates.Count; i++)
            {
                AccumulateCounts(_templates[i - window], windowCounts, false);
                AccumulateCounts(_templates[i], windowCounts, true);
                CopyMaxima(windowCounts, maxCounts);
            }

            return maxCounts;
        }

        private static void AccumulateCounts(LevelObject[] template, Dictionary<GameObject, int> counts, bool add)
        {
            foreach (var cell in template)
            {
                if (cell == null) continue;
                var count = counts.GetValueOrDefault(cell.Prefab) + (add ? 1 : -1);
                if (count > 0)
                    counts[cell.Prefab] = count;
                else
                    counts.Remove(cell.Prefab);
            }
        }

        private static void CopyMaxima(Dictionary<GameObject, int> source, Dictionary<GameObject, int> maxima)
        {
            foreach (var kv in source)
                if (!maxima.ContainsKey(kv.Key) || kv.Value > maxima[kv.Key])
                    maxima[kv.Key] = kv.Value;
        }

        private void SpawnInitialChunks()
        {
            _nextChunkPosition = _startOffset;
            _nextTemplateIndex = 0;

            int count = Mathf.Min((int) _chunksAhead, _templates.Count);
            for (int i = 0; i < count; i++)
                SpawnNextChunk();
        }

        private void SpawnNextChunk()
        {
            if (_nextTemplateIndex >= _templates.Count) return;

            var template = _templates[_nextTemplateIndex];
            uint lines = GameManager.Lines;
            uint chunkSize = GameManager.ChunkSize;
            float lineShift = GameManager.LinesShift;
            float laneCenter = (lines - 1f) / 2f;

            var chunk = new LevelChunk(template, _nextChunkPosition);

            for (int row = 0; row < chunkSize; row++)
            {
                for (int lane = 0; lane < lines; lane++)
                {
                    int i = (int)(row * lines) + lane;
                    var cell = chunk.Cells[i].Template;
                    if (cell == null) continue;

                    if (!_pools.TryGetValue(cell.Prefab, out var pool))
                        throw new InvalidOperationException($"Pool missing for prefab on {cell.Type}.");

                    float x = lineShift * (lane - laneCenter);
                    float z = (_nextChunkPosition + row) * _rowSpacing;
                    chunk.Cells[i].SpawnedObject = pool.Get(new Vector3(x, 0f, z), Quaternion.identity);
                }
            }

            _spawnedChunks.Enqueue(chunk);
            _nextTemplateIndex++;
            _nextChunkPosition += chunkSize + _gapRows;

            if (!_spawnFinished && _nextTemplateIndex >= _templates.Count)
            {
                _spawnFinished = true;
                OnFinishSpawn?.Invoke((uint)(_nextTemplateIndex - 1));
            }
        }

        private void RecyclePassedChunks()
        {
            float playerZ = _playerTransform.position.z;

            while (_spawnedChunks.Count > 0)
            {
                var chunk = _spawnedChunks.Peek();
                float endZ = (chunk.Position + GameManager.ChunkSize - 1) * _rowSpacing;

                float chunkLength = (GameManager.ChunkSize + _gapRows) * _rowSpacing;
                if (playerZ - chunkLength <= endZ) break;

                _spawnedChunks.Dequeue();
                ReturnChunkToPool(chunk);
                SpawnNextChunk();
            }

            if (!_finished && _spawnFinished && _spawnedChunks.Count == 0)
            {
                _finished = true;
                GameManager.OnFinish?.Invoke();
            }
        }

        public float RowSpacing => _rowSpacing;
        public uint StartOffset => _startOffset;

        public LevelChunk GetChunkAt(float worldZ)
        {
            foreach (var chunk in _spawnedChunks)
            {
                float startZ = chunk.Position * _rowSpacing;
                float endZ = (chunk.Position + GameManager.ChunkSize - 1) * _rowSpacing;
                if (worldZ >= startZ && worldZ <= endZ)
                    return chunk;
            }
            return null;
        }

        private void ReturnChunkToPool(LevelChunk chunk)
        {
            for (int i = 0; i < chunk.Cells.Length; i++)
            {
                if (chunk.Cells[i].SpawnedObject == null) continue;
                _pools[chunk.Cells[i].Template.Prefab].Return(chunk.Cells[i].SpawnedObject);
            }
        }
    }
}

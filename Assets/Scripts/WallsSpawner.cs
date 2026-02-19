using System;
using System.Linq;
using UnityEngine;

public class WallsSpawner : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject _wallsPrefab;
    [SerializeField, Min(0.001f)] private float _size = 10f;
    [SerializeField] private uint _count;
    
    [Header("Auto generated")]
    [SerializeField] private Transform[] _walls;
    
    private uint _lastMoved;
    
    private void OnValidate()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall -= SpawnWalls;
        UnityEditor.EditorApplication.delayCall += SpawnWalls;
#endif
    }
    
    private void Update() =>
        TryMoveWalls();

    private void SpawnWalls()
    {
        if (this == null) return;
        if (!_wallsPrefab) throw new MissingFieldException(nameof(_wallsPrefab));
        
        ClearWalls();
        
        _walls = Enumerable.Range(0, (int)_count).Select(i =>
            Instantiate(_wallsPrefab, transform.position + Vector3.forward * (_size * i),
                Quaternion.identity, transform).transform).ToArray();
    }

    private void ClearWalls()
    {
        foreach (var wall in _walls)
        {
            GameObject wallObj = wall?.gameObject;

            if (!wallObj) continue;
            DestroyImmediate(wallObj);
        }
    }
    
    private void TryMoveWalls()
    {
        if (!_playerTransform) throw new MissingFieldException(nameof(_playerTransform));
        
        if (_playerTransform.position.z <= _walls[_lastMoved].position.z) return;
        _walls[_lastMoved].Translate(Vector3.forward * (_size * _count));
        _lastMoved = _lastMoved < _count - 1 ? _lastMoved + 1 : 0;
    }
}

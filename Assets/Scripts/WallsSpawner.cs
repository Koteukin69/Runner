using System;
using System.Linq;
using UnityEngine;

public class WallsSpawner : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject _wallsPrefab;
    [SerializeField, Min(0.001f)] private float _size = 10f;
    [SerializeField] private uint _count;
    
    private Transform[] _walls;
    private uint _lastMoved;
    
    private void Start() =>
        SpawnWalls();

    private void Update() =>
        TryMoveWalls();

    private void SpawnWalls()
    {
        if (!_wallsPrefab) throw new ("");
        
        _walls = Enumerable.Range(0, (int) _count).Select(i => 
            Instantiate(_wallsPrefab, transform.position + Vector3.forward * (_size * i),
                Quaternion.identity, transform).transform).ToArray();
    }

    private void TryMoveWalls()
    {
        if (!_playerTransform) throw new ("");
        
        if (_playerTransform.position.z <= _walls[_lastMoved].position.z) return;
        _walls[_lastMoved].Translate(Vector3.forward * (_size * _count));
        _lastMoved = _lastMoved < _count - 1 ? _lastMoved + 1 : 0;
    }
}

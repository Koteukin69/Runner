using System;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class WallsSpawner : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject[] _wallsPrefabs;
    [SerializeField, Min(0.001f)] private float _size = 10f;
    [SerializeField] private uint _count;
    [SerializeField] private int _seed;
    
    [Header("Auto generated")]
    [SerializeField] private Transform[] _walls;
    
    private uint _lastMoved;
    
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
        UnityEditor.EditorApplication.delayCall -= SpawnWallsEditor;
        UnityEditor.EditorApplication.delayCall += SpawnWallsEditor;
#endif
    }

#if UNITY_EDITOR
    private void SpawnWallsEditor()
    {
        UnityEditor.EditorApplication.delayCall -= SpawnWallsEditor;
        if (this == null) return;
        SpawnWalls();
    }
#endif
    
    private void Update() =>
        TryMoveWalls();

    private void SpawnWalls()
    {
        if (this == null) return;
        if (_wallsPrefabs.Length < 1) throw new MissingFieldException(nameof(_wallsPrefabs));
        
        ClearWalls();
        
        var rnd = new Random(_seed);
        _walls = Enumerable.Range(0, (int)_count).Select(i =>
            Instantiate(_wallsPrefabs[rnd.Next(0, _wallsPrefabs.Length)], transform.position + Vector3.forward * (_size * i),
                Quaternion.identity, transform).transform).ToArray();
    }

    private void ClearWalls()
    {
#if UNITY_EDITOR
        UnityEditor.Selection.objects = System.Array.Empty<UnityEngine.Object>();
#endif
        foreach (var wall in _walls)
        {
            GameObject wallObj = wall?.gameObject;

            if (!wallObj) continue;
#if UNITY_EDITOR
            UnityEditor.Undo.DestroyObjectImmediate(wallObj);
#else
            DestroyImmediate(wallObj);
#endif
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

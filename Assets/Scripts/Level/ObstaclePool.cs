using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Level
{
    public class ObstaclePool
    {
        private readonly Stack<GameObject> _available;
        private readonly Transform _parent;

        public ObstaclePool(GameObject prefab, int capacity, Transform parent)
        {
            _parent = parent;
            _available = new Stack<GameObject>(capacity);

            for (int i = 0; i < capacity; i++) Return(Object.Instantiate(prefab, _parent));
        }

        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            if (_available.Count == 0)
                throw new InvalidOperationException("ObstaclePool exhausted. Pool size calculation is incorrect.");

            var obj = _available.Pop();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            _available.Push(obj);
        }
    }
}
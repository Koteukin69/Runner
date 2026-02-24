using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level
{
    public class LevelTemplates : MonoBehaviour
    {
        [SerializeField] private LevelObject[] _levelObjects;
        [SerializeField] private List<LevelTemplate> _templates = new();

        public IReadOnlyList<LevelObject> LevelObjects => _levelObjects;
        public IReadOnlyList<LevelTemplate> Templates => _templates;

        public Dictionary<LevelObjectType, LevelObject[]> GetObjectsByType()
        {
            return _levelObjects
                .GroupBy(o => o.Type)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
    }
}

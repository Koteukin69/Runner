using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level
{
    public class LevelTemplates : MonoBehaviour
    {
        [SerializeField] private LevelObject[] _levelObjects;
        [SerializeField] private List<LevelTemplate> _templates = new();

        private List<LevelObject[]> _converted;

        public IReadOnlyList<LevelObject> LevelObjects => _levelObjects;
        public IReadOnlyList<LevelTemplate> Templates => _templates;

        public IReadOnlyList<LevelObject[]> ConvertedTemplates =>
            _converted ??= _templates
                .Select(t => t.Grid
                    .Select(i => i >= 0 && i < _levelObjects.Length ? _levelObjects[i] : null)
                    .ToArray()
                ).ToList();
    }
}

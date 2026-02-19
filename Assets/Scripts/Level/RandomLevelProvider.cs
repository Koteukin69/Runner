using System;
using System.Collections.Generic;
using System.Linq;

namespace Level
{
    public class RandomLevelProvider : ILevelProvider
    {
        private readonly LevelTemplates _source;

        public RandomLevelProvider(LevelTemplates source) =>
            _source = source ?? throw new ArgumentNullException(nameof(source));

        public List<LevelObject[]> GetTemplates()
        {
            var converted = _source.ConvertedTemplates;
            return Enumerable.Range(0, (int)GameManager.LevelLength)
                .Select(_ => converted[new Random().Next(converted.Count)]).ToList();
        }
    }
}

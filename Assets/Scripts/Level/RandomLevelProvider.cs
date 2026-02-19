using System;
using System.Collections.Generic;
using System.Linq;

namespace Level
{
    public class RandomLevelProvider : ILevelProvider
    {
        private readonly LevelTemplates _source;
        private readonly int _seed;

        public RandomLevelProvider(LevelTemplates source, int seed = 0)
        {
            _seed = seed;
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public List<LevelObject[]> GetTemplates()
        {
            var converted = _source.ConvertedTemplates;
            Random rng = new (_seed);
            
            return Enumerable.Range(0, (int)GameManager.LevelLength)
                .Select(_ => converted[rng.Next(converted.Count)]).ToList();
        }
    }
}

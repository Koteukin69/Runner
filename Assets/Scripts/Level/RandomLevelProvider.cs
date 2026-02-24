using System;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

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
            var rng = new Random(_seed);
            var byType = _source.GetObjectsByType();

            var pool = _source.Templates
                .SelectMany(t => new[] { t.Grid, FlipGrid(t.Grid) })
                .ToList();

            return Enumerable.Range(0, (int)GameManager.LevelLength)
                .Select(_ => ResolveGrid(pool[rng.Next(pool.Count)], byType, rng))
                .ToList();
        }

        private static LevelObject[] ResolveGrid(int[] grid, Dictionary<LevelObjectType, LevelObject[]> byType, Random rng) =>
            grid.Select(cell =>
                cell >= 0 && byType.TryGetValue((LevelObjectType)cell, out var candidates) && candidates.Length > 0
                    ? candidates[rng.Next(candidates.Length)]
                    : null)
            .ToArray();

        private static int[] FlipGrid(int[] grid)
        {
            uint lines = GameManager.Lines;
            return Enumerable.Range(0, grid.Length)
                .Select(i => grid[i / lines * lines + (lines - 1 - i % lines)])
                .ToArray();
        }
    }
}

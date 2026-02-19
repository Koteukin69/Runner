using UnityEngine;

namespace Level
{
    public class LevelChunk
    {
        public struct Cell
        {
            public LevelObject Template;
            public GameObject SpawnedObject;
        }

        public Cell[] Cells { get; }
        public uint Position { get; }

        public LevelChunk(LevelObject[] template, uint position)
        {
            Cells = new Cell[template.Length];
            for (int i = 0; i < template.Length; i++)
                Cells[i].Template = template[i];
            Position = position;
        }
    }
}
using System;

namespace Level
{
    [Serializable]
    public class LevelTemplate
    {
        public int[] Grid = Array.Empty<int>();

        public void Resize(uint oldLanes, uint oldRows, uint newLanes, uint newRows)
        {
            int newSize = (int)(newLanes * newRows);
            int[] newGrid = new int[newSize];
            for (int i = 0; i < newSize; i++)
                newGrid[i] = -1;

            uint minLanes = Math.Min(oldLanes, newLanes);
            uint minRows = Math.Min(oldRows, newRows);

            for (uint row = 0; row < minRows; row++)
            {
                for (uint lane = 0; lane < minLanes; lane++)
                {
                    int oldIndex = (int)(row * oldLanes + lane);
                    int newIndex = (int)(row * newLanes + lane);
                    if (oldIndex < Grid.Length)
                        newGrid[newIndex] = Grid[oldIndex];
                }
            }

            Grid = newGrid;
        }
    }
}

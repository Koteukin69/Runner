using System.Collections.Generic;

namespace Level
{
    public interface ILevelProvider
    {
        List<LevelObject[]> GetTemplates();
    }
}

using System;
using System.Collections.Generic;

namespace Runtime.Save
{
    [Serializable]
    public class GameSave
    {
        public int Version;
        
        public Dictionary<int, SaveData> SaveData;
    }
}
using UnityEngine;

namespace Runtime.Save
{
    public abstract class Savable : MonoBehaviour
    {
        public abstract int SaveId { get; }
        
        public abstract SaveData Save();
        public abstract void Load(SaveData saveData);
    }
}
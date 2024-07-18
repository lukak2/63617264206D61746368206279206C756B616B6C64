using UnityEngine;

namespace Runtime.Game
{
    [CreateAssetMenu(fileName = "GameConfigurationData", menuName = "Game/Game Configuration Data", order = 0)]
    public class GameConfigurationData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        
        [field: SerializeField] public int ScorePerMatch { get; private set; }
        
        [field: SerializeField] public int RowCount { get; private set; }
        [field: SerializeField] public int ColumnCount { get; private set; }

        private void OnValidate()
        {
            if (RowCount * ColumnCount % 2 != 0)
            {
                Debug.LogError("Row count * Column count must be even number!");
            }
        }
    }
}
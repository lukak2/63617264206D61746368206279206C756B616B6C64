using UnityEngine;

namespace Runtime.Cards
{
    [CreateAssetMenu(fileName = "CardData", menuName = "Game/Card Data", order = 0)]
    public class CardData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite BackImage { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Color BackgroundColor { get; private set; }
    }
}
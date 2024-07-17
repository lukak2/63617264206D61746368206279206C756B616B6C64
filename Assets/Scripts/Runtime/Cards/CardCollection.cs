using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Cards
{
    [CreateAssetMenu(fileName = "CardCollection", menuName = "Game/Card Collection", order = 0)]
    public class CardCollection : ScriptableObject
    {
        [field: SerializeField] public List<CardData> Cards { get; private set; }
    }
}
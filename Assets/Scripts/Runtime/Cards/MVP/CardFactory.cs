using System;
using Runtime.MVP;

namespace Runtime.Cards.MVP
{
    // Limitation of the Unity serialization system
    [Serializable]
    public class CardFactory : Factory<CardPresenter, CardModel> { }
}
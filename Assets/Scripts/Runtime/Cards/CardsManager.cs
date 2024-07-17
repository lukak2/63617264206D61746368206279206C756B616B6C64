using System;
using Runtime.Cards.MVP;
using Runtime.Utilities;
using UnityEngine;

namespace Runtime.Cards
{
    public class CardsManager : MonoBehaviour
    {
        [SerializeField] private CardFactory factory = new();

        [SerializeField] private CardCollection cardCollection;

        private void Awake()
        {
            Initalize(2, 2);
        }

        public void Initalize(int rows, int columns)
        {
            var presenter = factory.Create(new CardModel()
            {
                CardData = cardCollection.Cards.GetRandomElement()
            }, transform);
            
            presenter.HideCard();
            presenter.OnBackSideClick += OnPresenterOnOnBackSideClick;
        }

        private async void OnPresenterOnOnBackSideClick(CardPresenter presenter)
        {
            await presenter.ShowCard();
            
            // TODO: ready to invoke matching logic
        }
    }
}

using System;
using System.Threading.Tasks;
using Runtime.MVP;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Cards.MVP
{
    public class CardPresenter : Presenter<CardModel>
    {
        [Header("Objects")]
        [SerializeField] private GameObject cardFrontSide;
        [SerializeField] private GameObject cardBackSide;
        
        [Header("Buttons")]
        [SerializeField] private Button cardBackSideButton;
        
        [Header("Images")]
        [SerializeField] private Image frontIconImage;
        [SerializeField] private Image frontBackgroundImage;
        [SerializeField] private Image backImage;

        public event Action<CardPresenter> OnBackSideClick;
        
        public override void Initialize(CardModel model)
        {
            base.Initialize(model);
            
            Subscribe();
            RefreshView();
        }

        protected override void Subscribe()
        {
            cardBackSideButton.onClick.AddListener(OnBackSideButtonClicked);
        }
        
        protected override void Unsubscribe()
        {
            cardBackSideButton.onClick.RemoveListener(OnBackSideButtonClicked);
        }

        protected override void RefreshView()
        {
            frontIconImage.sprite = Model.CardData.Icon;
            frontBackgroundImage.color = Model.CardData.BackgroundColor;
            backImage.sprite = Model.CardData.BackImage;
        }
        
        private void OnBackSideButtonClicked()
        {
            OnBackSideClick?.Invoke(this);
        }

        public Task ShowCard()
        {
            cardFrontSide.SetActive(true);
            cardBackSide.SetActive(false);
            
            return Task.CompletedTask;
        }

        public Task HideCard()
        {
            cardFrontSide.SetActive(false);
            cardBackSide.SetActive(true);
            
            return Task.CompletedTask;
        }
    }
}

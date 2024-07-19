using System;
using System.Threading.Tasks;
using Runtime.MVP;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Cards.MVP
{
    public class CardPresenter : Presenter<CardModel>
    {
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("Objects")]
        [SerializeField] private GameObject cardFrontSide;
        [SerializeField] private GameObject cardBackSide;
        
        [Header("Buttons")]
        [SerializeField] private Button cardBackSideButton;
        
        [Header("Images")]
        [SerializeField] private Image frontIconImage;
        [SerializeField] private Image frontBackgroundImage;
        [SerializeField] private Image backImage;

        public string CardName => Model.CardData.Name;
        
        public event Action<CardPresenter> OnBackSideClick;
        
        public override void Initialize(CardModel model)
        {
            base.Initialize(model);
            
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
        
        public void SetParentAndSiblingIndex(Transform parent, int index)
        {
            transform.SetParent(parent, false);
            transform.SetSiblingIndex(index);
        }
        
        private void OnBackSideButtonClicked()
        {
            OnBackSideClick?.Invoke(this);
        }

        /// <summary>
        /// Reveal performs the action of showing the front side of the card.
        /// </summary>
        /// <returns></returns>
        public Task Reveal()
        {
            SetInteractable(false);
            
            cardFrontSide.gameObject.SetActive(true);
            cardBackSide.gameObject.SetActive(false);
            
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Conceal performs the action of hiding front side and showing the back side of the card.
        /// </summary>
        /// <returns></returns>
        public Task Conceal()
        {
            SetInteractable(true);
            
            cardFrontSide.SetActive(false);
            cardBackSide.SetActive(true);
            
            return Task.CompletedTask;
        }

        public void SetInteractable(bool isInteractable)
        {
            cardBackSideButton.interactable = isInteractable;
        }

        public void Show()
        {
            canvasGroup.alpha = 1;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0;
        }
        
        public void Activate()
        {
            gameObject.SetActive(true);
        }
        
        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public bool Matches(CardPresenter other)
        {
            return Model.CardData == other.Model.CardData;
        }
    }
}

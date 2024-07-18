using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Runtime.Cards.MVP;
using Runtime.Game;
using Runtime.Utilities;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace Runtime.Cards
{
    public class CardsManager : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private CardFactory factory = new();
        [SerializeField] private RectTransform rowPrefab;
        
        [SerializeField] private CardCollection cardCollection;

        [Header("References")]
        [SerializeField] private RectTransform cardsContainer;
        
        [Header("Parameters")]
        [SerializeField] private float successfulMatchDuration = 2f;
        [SerializeField] private float failedMatchDuration = 1f;
        
        private IObjectPool<CardPresenter> _cardPool;
        private List<CardPresenter> _unmatchedCards = new();
        private List<CardPresenter> _activeCards = new();
        private List<RectTransform> _rows = new();
        
        private CardPresenter _lastClickedPresenter;

        public event Action OnCardDeplete;
        public event Action OnFailedMatch;
        public event Action OnSuccessfulMatch;

        private void Awake()
        {
            Setup();
        }

        public void Initalize(int rows, int columns)
        {
            Assert.IsTrue(rows * columns % 2 == 0, "Rows times Columns must be an even number");
            
            for (int i = 0; i < rows; i++)
            {
                RectTransform row = Instantiate(rowPrefab, cardsContainer);
                row.name = $"Row {i}";
                
                _rows.Add(row);
                
                for (int j = 0; j < columns; j++)
                {
                    CardPresenter presenter = _cardPool.Get();
                    presenter.transform.SetParent(row, false);
                    presenter.transform.SetSiblingIndex(j);
                    
                    _unmatchedCards.Add(presenter);
                }
            }
        }

        public void Reset()
        {
            for (var i = _activeCards.Count - 1; i >= 0; i--)
            {
                var activePresenter = _activeCards[i];
                // _cardPool.Release(activePresenter);
            }

            for (int i = _rows.Count - 1; i >= 0; i--)
            {
                Destroy(_rows[i].gameObject);
            }
            
            _rows.Clear();
            _unmatchedCards.Clear();
        }

        private void Setup()
        {
            _cardPool = new ObjectPool<CardPresenter>(
                () =>
            {
                CardPresenter presenter = factory.Create(new CardModel()
                {
                    CardData = cardCollection.Cards.GetRandomElement()
                }, transform);
                
                presenter.Deactivate();
                presenter.Conceal();
                
                presenter.OnBackSideClick += OnPresenterOnOnBackSideClickAsync;

                return presenter;
            }, 
                presenter =>
            {
                _activeCards.Add(presenter);
                
                presenter.Show();
                presenter.Activate();
            }, 
                actionOnRelease: presenter =>
            {
                _activeCards.Remove(presenter);
                
                presenter.Deactivate();
            });
        }

        private async void OnPresenterOnOnBackSideClickAsync(CardPresenter presenter)
        {
            presenter.SetInteractable(false);
            
            AudioFeedbackProvider.Instance.PlayClip(AudioFeedbackProvider.Instance.RevealSound);
            
            var lastPresenter = _lastClickedPresenter;
            
            if (lastPresenter != null)
            {
                if (presenter.Matches(lastPresenter) && presenter != lastPresenter)
                {
                    CompleteMatchAsync(presenter, lastPresenter);
                }
                else
                {
                    FailedMatchAsync(presenter, lastPresenter);
                }
                
                _lastClickedPresenter = null;
                
                return;
            }
            
            _lastClickedPresenter = presenter;
            
            await presenter.Reveal();
        }

        private async void CompleteMatchAsync(CardPresenter current, CardPresenter last)
        {
            AudioFeedbackProvider.Instance.PlayClip(AudioFeedbackProvider.Instance.MatchSound);
            
            // Combining reveal and delay so that duration is not affected by the time it takes to reveal the card
            await Task.WhenAll(current.Reveal(), Task.Delay(TimeSpan.FromSeconds(successfulMatchDuration)));
            
            await Task.WhenAll(current.Conceal(), last.Conceal());
            
            current.Hide();
            last.Hide();
            
            _unmatchedCards.Remove(current);
            _unmatchedCards.Remove(last);
            
            OnSuccessfulMatch?.Invoke();
            
            if (_unmatchedCards.Count == 0)
            {
                OnCardDeplete?.Invoke();
            }
            
        }

        private async void FailedMatchAsync(CardPresenter current, CardPresenter last)
        {
            AudioFeedbackProvider.Instance.PlayClip(AudioFeedbackProvider.Instance.MismatchSound);
            
            await Task.WhenAll(current.Reveal(), Task.Delay(TimeSpan.FromSeconds(failedMatchDuration)));
            
            await Task.WhenAll(current.Conceal(), last.Conceal());
            
            OnFailedMatch?.Invoke();
        }
    }
}

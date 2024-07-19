using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Runtime.Cards.MVP;
using Runtime.Feedback;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Runtime.Cards
{
    public class CardsManager : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private CardFactory factory = new();

        [Header("References")]
        [SerializeField] private GridLayoutGroup cardsGridLayoutGroup;
        [SerializeField] private RectTransform cardsContainer;
        
        [Header("Parameters")]
        [SerializeField] private float cardsPreviewDuration = 2f;
        [SerializeField] private float successfulMatchDuration = 2f;
        [SerializeField] private float failedMatchDuration = 1f;
        
        private IObjectPool<CardPresenter> _cardPool;
        private List<CardPresenter> _unmatchedCards = new();
        private List<CardPresenter> _activeCards = new();

        private CardPresenter _lastClickedPresenter;

        public event Action OnCardDeplete;
        public event Action OnFailedMatch;
        public event Action OnSuccessfulMatch;

        private void Awake()
        {
            Setup();
        }

        public void Initalize(int rows, int columns, List<CardData> orderedCards, int[] alreadyMatched)
        {
            Assert.IsTrue(rows * columns % 2 == 0, "Rows times Columns must be an even number");
            
            var gridLayoutTransform = AdjustGridLayout(columns, rows);
            
            var cardCount = rows * columns;
            
            for (int i = 0; i < cardCount; i++)
            {
                CardPresenter presenter = _cardPool.Get();
                    
                presenter.Initialize(new CardModel()
                {
                    CardData = orderedCards[i]
                });
                    
                presenter.SetParentAndSiblingIndex(gridLayoutTransform, i);
                
                if (alreadyMatched.Contains(i))
                {
                    presenter.Hide();
                }
                else
                {
                    _unmatchedCards.Add(presenter);
                }
            }
        }

        public void Reset()
        {
            for (var i = _activeCards.Count - 1; i >= 0; i--)
            {
                var activePresenter = _activeCards[i];
                _cardPool.Release(activePresenter);
            }

            _unmatchedCards.Clear();
        }
        
        public Dictionary<int, string> GetUnmatchedCards()
        {
            var unmatchedCards = new Dictionary<int, string>();
            
            for (var i = 0; i < _unmatchedCards.Count; i++)
            {
                CardPresenter card = _unmatchedCards[i];
                unmatchedCards.Add(_activeCards.IndexOf(card), card.CardName);
            }

            return unmatchedCards;
        }

        public async void PreviewAllCards()
        {
            // All Task usages would benefit from being replaced by UniTask and considering object lifetimes
            await Task.WhenAll(_activeCards.ConvertAll(card => card.Reveal()));

            await Task.Delay(TimeSpan.FromSeconds(cardsPreviewDuration));
            
            await Task.WhenAll(_activeCards.ConvertAll(card => card.Conceal()));
        }

        private RectTransform AdjustGridLayout(int columns, int rows)
        {
            var gridLayoutTransform = cardsGridLayoutGroup.GetComponent<RectTransform>();
            
            cardsGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            cardsGridLayoutGroup.constraintCount = columns;

            var gridRect = gridLayoutTransform.rect;
            float width = gridRect.width / columns;
            float height = gridRect.height / rows;

            float cellSize = Mathf.Min(width, height);
            cardsGridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);

            return gridLayoutTransform;
        }

        private void Setup()
        {
            _cardPool = new ObjectPool<CardPresenter>(
                () =>
            {
                CardPresenter presenter = factory.Create(transform);
                
                presenter.Deactivate();
                presenter.Conceal();
                
                presenter.OnBackSideClick += OnCardReveal;

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

        private async void OnCardReveal(CardPresenter presenter)
        {
            presenter.SetInteractable(false);
            
            AudioFeedbackProvider.Instance.PlayClip(AudioFeedbackProvider.Instance.AudioLibrary.CardFlip);
            
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
            AudioFeedbackProvider.Instance.PlayClip(AudioFeedbackProvider.Instance.AudioLibrary.CardMatch);
            
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
            AudioFeedbackProvider.Instance.PlayClip(AudioFeedbackProvider.Instance.AudioLibrary.CardMismatch);
            
            await Task.WhenAll(current.Reveal(), Task.Delay(TimeSpan.FromSeconds(failedMatchDuration)));
            
            await Task.WhenAll(current.Conceal(), last.Conceal());
            
            OnFailedMatch?.Invoke();
        }
    }
}

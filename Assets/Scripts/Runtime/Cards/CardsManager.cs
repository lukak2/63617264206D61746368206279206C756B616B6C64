using System;
using System.Threading.Tasks;
using Runtime.Cards.MVP;
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

        [SerializeField] private int rowCount = 4;
        [SerializeField] private int columnCount = 4;

        
        private ObjectPool<CardPresenter> _cardPool;
        
        private CardPresenter _lastClickedPresenter;

        private void Awake()
        {
            Setup();
            Initalize(rowCount, columnCount);
        }

        public void Initalize(int rows, int columns)
        {
            Assert.IsTrue(rows * columns % 2 == 0, "Rows times Columns must be an even number");
            
            for (int i = 0; i < rows; i++)
            {
                RectTransform row = Instantiate(rowPrefab, cardsContainer);
                row.name = $"Row {i}";
                
                for (int j = 0; j < columns; j++)
                {
                    CardPresenter presenter = _cardPool.Get();
                    presenter.transform.SetParent(row, false);
                    presenter.transform.SetSiblingIndex(j);
                }
            }
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
                presenter.Show();
                presenter.Activate();
            }, 
                actionOnRelease: presenter =>
            {
                presenter.Deactivate();
            });
        }

        private async void OnPresenterOnOnBackSideClickAsync(CardPresenter presenter)
        {
            presenter.SetInteractable(false);
            
            var lastPresenter = _lastClickedPresenter;
            
            if (lastPresenter != null)
            {
                if (presenter.Matches(lastPresenter) && presenter != lastPresenter)
                {
                    Debug.Log("Match success!");
                    CompleteMatchAsync(presenter, lastPresenter);
                }
                else
                {
                    Debug.Log("Match failed!");
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
            // Combining reveal and delay so that duration is not affected by the time it takes to reveal the card
            await Task.WhenAll(current.Reveal(), Task.Delay(TimeSpan.FromSeconds(successfulMatchDuration)));
            
            await Task.WhenAll(current.Conceal(), last.Conceal());
            
            current.Hide();
            last.Hide();
        }

        private async void FailedMatchAsync(CardPresenter current, CardPresenter last)
        {
            await Task.WhenAll(current.Reveal(), Task.Delay(TimeSpan.FromSeconds(failedMatchDuration)));
            
            await Task.WhenAll(current.Conceal(), last.Conceal());
        }
    }
}

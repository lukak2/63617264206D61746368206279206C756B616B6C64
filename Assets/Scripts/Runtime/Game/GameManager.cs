using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Runtime.Cards;
using Runtime.Save;
using Runtime.UserInterface;
using Runtime.Utilities;
using UnityEngine;

namespace Runtime.Game
{
    public class GameManager : Savable
    {
        // Avoiding the use of the Singleton pattern, DI solution would be beneficial
        
        [SerializeField] private List<GameConfigurationData> gameConfigurations;
        [SerializeField] private CardCollection cardCollection;
        
        [Header("References")]
        [SerializeField] private ScoreManager _scoreManager;
        [SerializeField] private CardsManager _cardsManager;
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private SaveManager _saveManager;

        [SerializeField] private GameObject _mainMenuCanvas;
        [SerializeField] private GameObject _gameCanvas;
        
        private GameConfigurationData _selectedGameConfiguration;
        
        private bool IsGameRunning => _gameCanvas.activeSelf;
        
        public override int SaveId => Animator.StringToHash("GameManager");
        
        private void Awake()
        {
            _mainMenu.Initalize(gameConfigurations);
            
            _mainMenu.OnGameConfigurationSelect += StartGame;
            
            _cardsManager.OnSuccessfulMatch += _scoreManager.SuccessfulMatch;
            _cardsManager.OnFailedMatch += _scoreManager.FailedMatch;
            _cardsManager.OnCardDeplete += GameOver;
        }

        private void OnDestroy()
        {
            _mainMenu.OnGameConfigurationSelect -= StartGame;
            
            _cardsManager.OnSuccessfulMatch -= _scoreManager.SuccessfulMatch;
            _cardsManager.OnFailedMatch -= _scoreManager.FailedMatch;
            _cardsManager.OnCardDeplete -= GameOver;
        }
        
        public override SaveData Save()
        {
            if (!IsGameRunning)
            {
                return null;
            }
            
            Dictionary<int,string> unmatchedCards = _cardsManager.GetUnmatchedCards();
            
            var saveData = new SaveData();
            saveData.SetString("SelectedConfiguration", _selectedGameConfiguration.Name);
            saveData.SetString("unmatchedCards", JsonUtility.ToJson(unmatchedCards));

            return saveData;
        }

        public override void Load(SaveData saveData)
        {
            if (saveData == null)
            {
                FocusMainMenuCanvas();
                return;
            }
            
            var selectedGameConfiguration = saveData.GetString("SelectedConfiguration");
            var unmatchedCards = JsonUtility.FromJson<Dictionary<int,string>>(saveData.GetString("unmatchedCards"));

            var alreadyMatched = Enumerable
                .Range(0, _selectedGameConfiguration.RowCount * _selectedGameConfiguration.ColumnCount)
                .Where(i => !unmatchedCards.ContainsKey(i))
                .ToArray();
            
            var cards = Enumerable
                .Range(0, _selectedGameConfiguration.RowCount * _selectedGameConfiguration.ColumnCount)
                .Where(i => cardCollection.Cards.Any(card => card.Name == unmatchedCards[i]))
                .Select(i => cardCollection.Cards.First(card => card.Name == unmatchedCards[i]))
                .ToList();
            
            _selectedGameConfiguration = gameConfigurations.First(config => config.Name == selectedGameConfiguration);
            _cardsManager.Initalize(_selectedGameConfiguration.RowCount, _selectedGameConfiguration.ColumnCount, cards, alreadyMatched);
        }


        // Entry point of the game
        private void Start()
        {
            if (_saveManager.TryLoadGame())
            {
                StartWithLoad();
            }
            else
            {
                FocusMainMenuCanvas();
            }
        }

        private async void GameOver()
        {
            _scoreManager.ShowFinalScore();
            
            await Task.Delay(TimeSpan.FromSeconds(2));
            
            FinishGame();
        }
        
        public void StartGame(GameConfigurationData gameConfigurationData)
        {
            _selectedGameConfiguration = gameConfigurationData;
            
            _scoreManager.Reset();

            var testingCards = Enumerable
                .Range(0, (gameConfigurationData.RowCount * gameConfigurationData.ColumnCount) / 2)
                .Select(i => cardCollection.Cards.GetRandomElement())
                .ToList();
            
            testingCards.AddRange(testingCards);

            testingCards.Shuffle();
            
            _cardsManager.Initalize(gameConfigurationData.RowCount, gameConfigurationData.ColumnCount, testingCards, Array.Empty<int>());

            FocusGameCanvas();
            
            _cardsManager.PreviewAllCards();
        }

        public void FinishGame()
        {
            _cardsManager.Reset();
            FocusMainMenuCanvas();
        }

        private void StartWithLoad()
        {
            FocusGameCanvas();
        }
        
        // Avoiding using Scene management, since that is not the focus of the task, so GameManger will handle state
        private void FocusGameCanvas()
        {
            _mainMenuCanvas.SetActive(false);
            _gameCanvas.SetActive(true);
        }

        private void FocusMainMenuCanvas()
        {
            _mainMenuCanvas.SetActive(true);
            _gameCanvas.SetActive(false);
        }
    }
}

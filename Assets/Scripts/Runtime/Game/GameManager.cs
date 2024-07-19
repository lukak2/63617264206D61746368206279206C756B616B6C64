using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        
        private const string SavedGameConfigurationKey = "SavedGameConfiguration";
        private const string SavedCardsKey = "SavedCards";

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

            _mainMenu.OnGameConfigurationSelect += OnGameConfigurationSelected;

            _cardsManager.OnSuccessfulMatch += _scoreManager.SuccessfulMatch;
            _cardsManager.OnFailedMatch += _scoreManager.FailedMatch;
            _cardsManager.OnCardDeplete += GameOver;
        }

        private void OnDestroy()
        {
            _mainMenu.OnGameConfigurationSelect -= OnGameConfigurationSelected;

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

            List<string> cards = _cardsManager.GetOrderedMatchCardNames();

            var saveData = new SaveData();
            saveData.SetString(SavedGameConfigurationKey, _selectedGameConfiguration.Name);
            saveData.SetString(SavedCardsKey, JsonConvert.SerializeObject(cards));

            return saveData;
        }

        public override void Load(SaveData saveData)
        {
            if (saveData == null)
            {
                FocusMainMenuCanvas();
                return;
            }

            var savedGameConfigurationName = saveData.GetString(SavedGameConfigurationKey);
            var savedGameConfiguration = gameConfigurations.First(config => config.Name == savedGameConfigurationName);
            
            var savedCards = JsonConvert.DeserializeObject<List<string>>(saveData.GetString(SavedCardsKey));

            var cards = new List<CardData>();

            foreach (var cardName in savedCards)
            {
                cards.Add(cardCollection.Cards.FirstOrDefault(card => card.Name == cardName));
            }
            
            StartGame(savedGameConfiguration, cards, true);
        }


        // Entry point of the game
        private void Start()
        {
            if (_saveManager.TryLoadGame())
            {
                FocusGameCanvas();
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

        public void StartGame(GameConfigurationData gameConfigurationData, List<CardData> cards, bool isLoaded)
        {
            _selectedGameConfiguration = gameConfigurationData;

            _scoreManager.Reset(isLoaded);
            
            _cardsManager.Initalize(gameConfigurationData.RowCount, gameConfigurationData.ColumnCount, cards);

            FocusGameCanvas();

            _cardsManager.PreviewAllCards();
        }

        public void FinishGame()
        {
            _cardsManager.Reset();
            FocusMainMenuCanvas();
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

        private void OnGameConfigurationSelected(GameConfigurationData gameConfigurationData)
        {
            var generatedCards = Enumerable
                .Range(0, (gameConfigurationData.RowCount * gameConfigurationData.ColumnCount) / 2)
                .Select(i => cardCollection.Cards.GetRandomElement())
                .ToList();

            // Simplify generation of pairs
            generatedCards.AddRange(generatedCards);

            // Randomize card order
            generatedCards.Shuffle();

            StartGame(gameConfigurationData, generatedCards, false);
        }
    }
}
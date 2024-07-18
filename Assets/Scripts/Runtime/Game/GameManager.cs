using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Runtime.Cards;
using Runtime.UserInterface;
using UnityEngine;

namespace Runtime.Game
{
    public class GameManager : MonoBehaviour
    {
        // Avoiding the use of the Singleton pattern, DI solution would be beneficial
        
        [SerializeField] private List<GameConfigurationData> gameConfigurations;
        
        [Header("References")]
        [SerializeField] private ScoreManager _scoreManager;
        [SerializeField] private CardsManager _cardsManager;
        [SerializeField] private MainMenu _mainMenu;

        [SerializeField] private GameObject _mainMenuCanvas;
        [SerializeField] private GameObject _gameCanvas;
        
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


        // Entry point of the game
        private void Start()
        {
            FocusMainMenuCanvas();
        }

        private async void GameOver()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            
            FinishGame();
        }
        
        public void StartGame(GameConfigurationData gameConfigurationData)
        {
            _scoreManager.Reset();
            
            _cardsManager.Initalize(gameConfigurationData.RowCount, gameConfigurationData.ColumnCount);

            FocusGameCanvas();
        }

        public void FinishGame()
        {
            _cardsManager.Reset();
            FocusMainMenuCanvas();
        }
        
        // Avoiding using Scene management, since that is not the focus of the task, so GameManger will handle state
        public void FocusGameCanvas()
        {
            _mainMenuCanvas.SetActive(false);
            _gameCanvas.SetActive(true);
        }
        
        public void FocusMainMenuCanvas()
        {
            _mainMenuCanvas.SetActive(true);
            _gameCanvas.SetActive(false);
        }
    }
}

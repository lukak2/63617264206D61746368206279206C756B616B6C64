using System;
using System.Collections.Generic;
using Runtime.Game;
using UnityEngine;

namespace Runtime.UserInterface
{
    public class MainMenu : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform gameListContainer;
        
        [Header("Resources")]
        [SerializeField] private GameStartButton gameStartButtonPrefab;

        public event Action<GameConfigurationData> OnGameConfigurationSelect;
        
        public void Initalize(List<GameConfigurationData> gameConfigurations)
        {
            foreach (var gameConfigurationData in gameConfigurations)
            {
                var newButton = Instantiate(gameStartButtonPrefab, gameListContainer);
                
                var buttonText = $"Play {gameConfigurationData.Name}";
                
                newButton.Initailize(buttonText, () => OnGameConfigurationSelect?.Invoke(gameConfigurationData));
            }
        }
    }
}
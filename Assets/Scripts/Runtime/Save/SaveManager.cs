using System;
using System.Collections.Generic;
using Runtime.Save.Provider;
using Runtime.Save.Serializer;
using UnityEngine;

namespace Runtime.Save
{
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private List<Savable> savables;

        private ISerializer _serializer;
        private IProvider _provider;

        private void Awake()
        {
            _serializer = new JsonSerializer();
            _provider = new PlayerPrefsProvider();
        }
        
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveGame();
            }
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        public bool TryLoadGame()
        {
            var json = _provider.Load("game_save");
            
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            var gameSave = _serializer.Deserialize<GameSave>(json);
            
            if (gameSave == null)
            {
                return false;
            }
            
            LoadGameSave(gameSave);
            return true;
        }
        
        public void SaveGame()
        {
            var gameSave = GetGameSave();
            var json = _serializer.Serialize(gameSave);
            _provider.Save("game_save", json);
        }
        
        public void ClearSave()
        {
            _provider.Save("game_save", String.Empty);
        }
        
        private void LoadGameSave(GameSave gameSave)
        {
            foreach (var savable in savables)
            {
                savable.Load(gameSave.SaveData[savable.SaveId]);
            }
        }
        
        private GameSave GetGameSave()
        {
            var saveData = new Dictionary<int, SaveData>();
            
            foreach (var savable in savables)
            {
                saveData[savable.SaveId] = savable.Save();
            }
            
            return new GameSave
            {
                Version = 1,
                SaveData = saveData
            };
        }
    }
}
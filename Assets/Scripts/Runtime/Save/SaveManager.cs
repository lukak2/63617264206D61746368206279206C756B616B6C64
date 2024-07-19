using System;
using System.Collections.Generic;
using Runtime.Save.Provider;
using Runtime.Save.Serializer;
using UnityEngine;

namespace Runtime.Save
{
    public class SaveManager : MonoBehaviour
    {
        private const string GameSaveKey = "game_save";
        
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
            try
            {
                var json = _provider.Load(GameSaveKey);

                var gameSave = _serializer.Deserialize<GameSave>(json);

                return LoadGameSave(gameSave);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ClearSave();

                return false;
            }
        }

        public void SaveGame()
        {
            try
            {
                var gameSave = GetGameSave();
                var json = _serializer.Serialize(gameSave);
                _provider.Save(GameSaveKey, json);
                // Debug.Log(json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ClearSave();
            }
        }

        public void ClearSave()
        {
            _provider.Save(GameSaveKey, String.Empty);
        }

        private bool LoadGameSave(GameSave gameSave)
        {
            bool foundInvalidData = false;

            foreach (var savable in savables)
            {
                if (gameSave.SaveData == null)
                {
                    foundInvalidData = true;
                    continue;
                }

                savable.Load(gameSave.SaveData[savable.SaveId]);
            }

            if (foundInvalidData)
            {
                ClearSave();
            }

            return !foundInvalidData;
        }

        private GameSave GetGameSave()
        {
            var saveData = new Dictionary<int, SaveData>();

            foreach (var savable in savables)
            {
                saveData.Add(savable.SaveId, savable.Save());
            }

            return new GameSave
            {
                Version = 1,
                SaveData = saveData
            };
        }
    }
}
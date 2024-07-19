using UnityEngine;

namespace Runtime.Save.Provider
{
    public class PlayerPrefsProvider : IProvider
    {
        public void Save(string key, string data)
        {
            PlayerPrefs.SetString(key, data);
        }

        public string Load(string key)
        {
            return PlayerPrefs.GetString(key);
        }
    }
}
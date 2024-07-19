using System;
using System.Collections.Generic;

namespace Runtime.Save
{
    [Serializable]
    public class SaveData
    {
        private Dictionary<string, string> StringData = new();
        private Dictionary<string, int> IntData = new();

        public void SetInt(string key, int value)
        {
            IntData[key] = value;
        }
        
        public int GetInt(string key)
        {
            return IntData[key];
        }
        
        public void SetString(string key, string value)
        {
            StringData[key] = value;
        }
        
        public string GetString(string key)
        {
            return StringData[key];
        }
    }
}
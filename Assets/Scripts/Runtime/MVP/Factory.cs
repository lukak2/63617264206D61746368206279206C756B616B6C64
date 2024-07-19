using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.MVP
{
    [Serializable]
    public abstract class Factory<T, TJ> where T : Presenter<TJ> where TJ : Model
    {
        [SerializeReference] private T viewPrefab;
        
        public T Create(Transform parent)
        {
            T instance = Object.Instantiate(viewPrefab, parent);
            
            return instance;
        }
        
        public T Create(TJ model, Transform parent)
        {
            T instance = Object.Instantiate(viewPrefab, parent);
            instance.Initialize(model);

            return instance;
        }
    }
}
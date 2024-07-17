using UnityEngine;

namespace Runtime.MVP
{
    public abstract class Presenter<T> : MonoBehaviour where T : Model
    {
        protected T Model { get; private set; }

        public virtual void Initialize(T model)
        {
            Model = model;
            
            Subscribe();
        }

        protected virtual void OnDestroy()
        {
            Unsubscribe();
        }
        
        protected abstract void Subscribe();
        protected abstract void Unsubscribe();
        protected abstract void RefreshView();
    }
}
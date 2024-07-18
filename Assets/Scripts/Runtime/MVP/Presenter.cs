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
        
        /// <summary>
        /// Subscribe is automatically invoked by base class, call it only when not using base.Initialize().
        /// </summary>
        protected abstract void Subscribe();
        
        /// <summary>
        /// Unsubscribe is automatically invoked by base class, call it only when not using base.Initialize().
        /// </summary>
        protected abstract void Unsubscribe();
        protected abstract void RefreshView();
    }
}
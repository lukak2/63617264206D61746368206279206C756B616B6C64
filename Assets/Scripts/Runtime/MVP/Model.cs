using System;

namespace Runtime.MVP
{
    // Follows the Dispose pattern
    [Serializable]
    public abstract class Model : IDisposable
    {
        private bool _disposed;
        
        ~Model() => Dispose(false);
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}
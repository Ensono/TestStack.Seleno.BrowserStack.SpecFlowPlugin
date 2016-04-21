using System;

namespace TestStack.Seleno.BrowserStack.Core
{
    public abstract class Disposable : IDisposable
    {
        /// <summary>
        ///     Finalizer.
        /// </summary>
        ~Disposable()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Is this instance disposed?
        /// </summary>
        protected bool Disposed { get; private set; }

        /// <summary>
        ///     Implementation of IDisposable.Dispose method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Dispose worker method. See http://coding.abel.nu/2012/01/disposable
        /// </summary>
        /// <param name="disposing">
        ///     Are we disposing?
        ///     Otherwise we're finalizing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
        }
    }
}
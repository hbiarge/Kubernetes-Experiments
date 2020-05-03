using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Mvc.Infrastructure
{
    public sealed class ConfigMapFileProviderChangeToken : IChangeToken, IDisposable
    {
        private sealed class CallbackRegistration : IDisposable
        {
            private Action<object> _callback;
            private object _state;
            private Action<CallbackRegistration> _unRegister;

            public CallbackRegistration(Action<object> callback, object state, Action<CallbackRegistration> unRegister)
            {
                _callback = callback;
                _state = state;
                _unRegister = unRegister;
            }

            public void Notify()
            {
                var localState = _state;
                var localCallback = _callback;

                localCallback?.Invoke(localState);
            }

            public void Dispose()
            {
                var localUnRegister = Interlocked.Exchange(ref _unRegister, null);

                if (localUnRegister != null)
                {
                    localUnRegister(this);
                    _callback = null;
                    _state = null;
                }
            }
        }

        private readonly object _timerLock = new object();
        private readonly string _root;
        private readonly string _filter;
        private readonly int _detectChangeIntervalMs;

        List<CallbackRegistration> _registeredCallbacks;
        private Timer _timer;
        private string _lastFileChecksum;

        public ConfigMapFileProviderChangeToken(string root, string filter, int detectChangeIntervalMs = 30_000)
        {
            _registeredCallbacks = new List<CallbackRegistration>();
            _root = root;
            _filter = filter;
            _detectChangeIntervalMs = detectChangeIntervalMs;
        }

        public bool HasChanged { get; private set; }

        public bool ActiveChangeCallbacks => true;

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            var localRegisteredCallbacks = _registeredCallbacks;
            if (localRegisteredCallbacks == null)
            {
                throw new ObjectDisposedException(nameof(_registeredCallbacks));
            }

            var cbRegistration = new CallbackRegistration(callback, state, cb => localRegisteredCallbacks.Remove(cb));
            localRegisteredCallbacks.Add(cbRegistration);

            return cbRegistration;
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _registeredCallbacks, null);

            Timer localTimer = null;
            lock (_timerLock)
            {
                localTimer = Interlocked.Exchange(ref _timer, null);
            }

            localTimer.Dispose();
        }

        internal void EnsureStarted()
        {
            lock (_timerLock)
            {
                if (_timer != null)
                {
                    return;
                }

                var fullPath = Path.Combine(_root, _filter);
                
                if (File.Exists(fullPath))
                {
                    _timer = new Timer(
                        CheckForChanges,
                        null,
                        TimeSpan.Zero,
                        TimeSpan.FromMilliseconds(_detectChangeIntervalMs));
                }
            }
        }

        private static string GetFileChecksum(string filename)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filename);
            return BitConverter.ToString(md5.ComputeHash(stream));
        }

        private void CheckForChanges(object state)
        {
            var fullPath = Path.Combine(_root, _filter);

            var currentFileChecksum = GetFileChecksum(fullPath);
            var currentFileHasChanges = false;
            if (_lastFileChecksum != null && _lastFileChecksum != currentFileChecksum)
            {
                // changed
                NotifyChanges();

                currentFileHasChanges = true;
            }

            HasChanged = currentFileHasChanges;

            _lastFileChecksum = currentFileChecksum;
        }

        private void NotifyChanges()
        {
            var localRegisteredCallbacks = _registeredCallbacks;
            if (localRegisteredCallbacks != null)
            {
                var count = localRegisteredCallbacks.Count;
                for (var i = 0; i < count; i++)
                {
                    localRegisteredCallbacks[i].Notify();
                }
            }
        }
    }
}

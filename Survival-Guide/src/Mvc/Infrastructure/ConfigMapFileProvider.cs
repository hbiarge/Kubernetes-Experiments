using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;

namespace Mvc.Infrastructure
{
    /// <summary>
    /// Simple <see cref="IFileProvider"/> implementation using config maps as source
    /// Config maps volumes in Linux/Kubernetes are implemented as symlink files.
    /// Once reloaded their Last modified date does not change. This implementation uses a check sum to verify
    /// </summary>
    public class ConfigMapFileProvider : IFileProvider
    {
        readonly ConcurrentDictionary<string, ConfigMapFileProviderChangeToken> _watchers;

        private ConfigMapFileProvider(string root)
        {
            if (string.IsNullOrWhiteSpace(root))
            {
                throw new ArgumentException("Invalid root path", nameof(root));
            }

            Root = root;
            _watchers = new ConcurrentDictionary<string, ConfigMapFileProviderChangeToken>();
        }

        public string Root { get; }

        public static IFileProvider FromRelativePath(string subpath)
        {
            var executableLocation = Assembly.GetEntryAssembly().Location;
            var executablePath = Path.GetDirectoryName(executableLocation);
            var configPath = Path.Combine(executablePath, subpath);

            if (Directory.Exists(configPath))
            {
                return new ConfigMapFileProvider(configPath);
            }

            return null;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new PhysicalDirectoryContents(Path.Combine(Root, subpath));
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var fi = new FileInfo(Path.Combine(Root, subpath));
            return new PhysicalFileInfo(fi);
        }

        public IChangeToken Watch(string filter)
        {
            var watcher = _watchers.AddOrUpdate(
                key: filter,
                addValueFactory: f => new ConfigMapFileProviderChangeToken(Root, filter),
                updateValueFactory: (f, e) =>
                {
                    e.Dispose();
                    return new ConfigMapFileProviderChangeToken(Root, filter);
                });

            watcher.EnsureStarted();
            return watcher;
        }
    }
}
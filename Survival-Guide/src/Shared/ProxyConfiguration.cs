using Microsoft.Extensions.Configuration;

namespace Shared
{
    public class ProxyConfiguration
    {
        public const string DefaultPathBase = "/";

        private ProxyConfiguration(ProxyBehavior proxyBehavior, string pathBase)
        {
            ProxyBehavior = proxyBehavior;

            if (string.IsNullOrWhiteSpace(pathBase) || DefaultPathBase.Equals(pathBase))
            {
                IsDefaultPathBase = true;
                DefaultOrCurrentPathBaseWithoutTrailingSlash = DefaultPathBase;
            }
            else
            {
                IsDefaultPathBase = false;
                DefaultOrCurrentPathBaseWithoutTrailingSlash = pathBase.TrimEnd('*').TrimEnd('/');
            }
        }

        public ProxyBehavior ProxyBehavior { get; }

        public bool IsDefaultPathBase { get; }

        public string DefaultOrCurrentPathBaseWithoutTrailingSlash { get; }

        public static ProxyConfiguration FromConfiguration(IConfiguration configuration)
        {
            // ForwardedHeaders is managed by this environment variable (or configuration: ASPNETCORE_FORWARDEDHEADERS_ENABLED)

            // Can be environment variables in the format [ASPNETCORE|DOTNET]_PATHBASE and [ASPNETCORE|DOTNET]_PROXYBEHAVIOR
            // Or included in the appsettings.json in the root configuration or in a section

            var pathBase = configuration.GetValue<string>("PathBase", defaultValue: DefaultPathBase);
            var proxyBehavior = configuration.GetValue<ProxyBehavior>("ProxyBehavior", defaultValue: ProxyBehavior.NoProxy);

            return new ProxyConfiguration(proxyBehavior, pathBase);
        }
    }

    public enum ProxyBehavior
    {
        NoProxy,
        MaintainProxyPath,
        RemoveProxyPath
    }
}

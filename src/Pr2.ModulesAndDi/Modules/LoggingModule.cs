using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pr2.ModulesAndDi.Core;
using Pr2.ModulesAndDi.Services;

namespace Pr2.ModulesAndDi.Modules;

public sealed class LoggingModule : IAppModule
{
    public string Name => "Logging";

    public IReadOnlyCollection<string> Requires => new[] { "Core" };

    public void RegisterServices(IServiceCollection services)
    {
        services.AddLogging(b => b.AddConsole());
        services.AddSingleton<IAppLogger, ConsoleAppLogger>();
        services.AddSingleton<IAppAction, LoggingAction>();
    }

    public Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        => Task.CompletedTask;

    private sealed class LoggingAction : IAppAction
    {
        private readonly IAppLogger _appLogger;

        public LoggingAction(IAppLogger appLogger) => _appLogger = appLogger;

        public string Title => "Проверка журнала событий";

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _appLogger.LogMessage("Сообщение из модуля журналирования через IAppLogger");
            return Task.CompletedTask;
        }
    }
}

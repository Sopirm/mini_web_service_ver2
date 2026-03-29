using Microsoft.Extensions.DependencyInjection;
using Pr2.ModulesAndDi.Core;
using Pr2.ModulesAndDi.Services;

namespace Pr2.ModulesAndDi.Modules;

public sealed class ValidationModule : IAppModule
{
    public string Name => "Validation";

    public IReadOnlyCollection<string> Requires => new[] { "Core" };

    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IAppAction, ValidationAction>();
    }

    public Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        => Task.CompletedTask;

    private sealed class ValidationAction : IAppAction
    {
        private readonly IPartRepository _partRepository;
        private readonly IAppLogger _appLogger;

        public ValidationAction(IPartRepository partRepository, IAppLogger appLogger)
        {
            _partRepository = partRepository;
            _appLogger = appLogger;
        }

        public string Title => "Проверка правил данных автозапчастей";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _appLogger.LogMessage("Начало проверки правил данных автозапчастей");

            var newPart = new Part(Guid.NewGuid(), "ART-001", "Масляный фильтр", "Фильтр для двигателя", 15.50m);
            
            // Пример валидации: проверка уникальности артикула
            var existingPart = await _partRepository.GetPartByArticleAsync(newPart.Article);
            if (existingPart != null)
            {
                _appLogger.LogMessage($"Ошибка валидации: Запчасть с артикулом {newPart.Article} уже существует. (ID: {existingPart.Id})");
                // throw new InvalidOperationException($"Запчасть с артикулом {newPart.Article} уже существует.");
            }
            else
            {
                await _partRepository.AddPartAsync(newPart);
                _appLogger.LogPartAdded(newPart);
            }

            _appLogger.LogMessage("Проверка правил данных автозапчастей завершена");
        }
    }
}

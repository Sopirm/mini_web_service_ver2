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

            var partsToCreate = new[]
            {
                new Part(Guid.NewGuid(), "ART-001", "Масляный фильтр", "Фильтр для двигателя", 15.50m),
                new Part(Guid.NewGuid(), "ART-002", "Тормозные колодки", "Передние тормозные колодки", 45.00m),
                new Part(Guid.NewGuid(), "ART-003", "Свеча зажигания", "Иридиевая свеча зажигания", 12.80m)
            };

            foreach (var part in partsToCreate)
            {
                var existingPart = await _partRepository.GetPartByArticleAsync(part.Article);
                if (existingPart != null)
                {
                    _appLogger.LogMessage($"Запчасть с артикулом {part.Article} уже существует.");
                }
                else
                {
                    await _partRepository.AddPartAsync(part);
                    _appLogger.LogPartAdded(part);
                }
            }

            _appLogger.LogMessage("Проверка правил данных автозапчастей завершена");
        }
    }
}

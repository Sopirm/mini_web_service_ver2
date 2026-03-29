using Microsoft.Extensions.DependencyInjection;
using Pr2.ModulesAndDi.Core;
using Pr2.ModulesAndDi.Services;

namespace Pr2.ModulesAndDi.Modules;

public sealed class ReportModule : IAppModule
{
    public string Name => "Report";

    public IReadOnlyCollection<string> Requires => new[] { "Core", "Export" };

    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IAppAction, ReportAction>();
    }

    public Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        => Task.CompletedTask;

    private sealed class ReportAction : IAppAction
    {
        private readonly IClock _clock;
        private readonly IPartRepository _partRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IAppLogger _appLogger;

        public ReportAction(IClock clock, IPartRepository partRepository, IStockRepository stockRepository, IAppLogger appLogger)
        {
            _clock = clock;
            _partRepository = partRepository;
            _stockRepository = stockRepository;
            _appLogger = appLogger;
        }

        public string Title => "Формирование отчёта по складу";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _appLogger.LogMessage($"Начало формирования отчёта по складу, время {_clock.Now}");

            var parts = await _partRepository.GetAllPartsAsync();
            var stockItems = await _stockRepository.GetAllStockItemsAsync();

            Console.WriteLine("\n--- Отчёт по складу ---");
            foreach (var part in parts)
            {
                var stockItem = stockItems.FirstOrDefault(s => s.PartId == part.Id);
                var quantity = stockItem?.Quantity ?? 0;
                var location = stockItem?.Location ?? "N/A";
                Console.WriteLine($"Запчасть: {part.Name} (Артикул: {part.Article}), Количество: {quantity}, Расположение: {location}");
            }
            Console.WriteLine("-----------------------\n");

            _appLogger.LogMessage($"Отчёт по складу успешно сформирован, количество уникальных запчастей: {parts.Count}");
        }
    }
}

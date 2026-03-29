using Microsoft.Extensions.DependencyInjection;
using Pr2.ModulesAndDi.Core;
using Pr2.ModulesAndDi.Services;
using System.Text;

namespace Pr2.ModulesAndDi.Modules;

public sealed class ExportModule : IAppModule
{
    public string Name => "Export";

    public IReadOnlyCollection<string> Requires => new[] { "Core", "Validation" };

    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IAppAction, ExportAction>();
    }

    public Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        => Task.CompletedTask;

    private sealed class ExportAction : IAppAction
    {
        private readonly IPartRepository _partRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IAppLogger _appLogger;

        public ExportAction(IPartRepository partRepository, IStockRepository stockRepository, IAppLogger appLogger)
        {
            _partRepository = partRepository;
            _stockRepository = stockRepository;
            _appLogger = appLogger;
        }

        public string Title => "Экспорт данных склада в CSV";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var parts = await _partRepository.GetAllPartsAsync();
            var stockItems = await _stockRepository.GetAllStockItemsAsync();

            var lines = new List<string>
            {
                "ID Запчасти;Артикул;Название;Описание;Цена;Количество на складе;Расположение"
            };

            foreach (var part in parts)
            {
                var stockItem = stockItems.FirstOrDefault(s => s.PartId == part.Id);
                var quantity = stockItem?.Quantity ?? 0;
                var location = stockItem?.Location ?? "N/A";
                lines.Add($"{part.Id};{part.Article};{part.Name};{part.Description};{part.Price};{quantity};{location}");
            }

            var path = Path.Combine(AppContext.BaseDirectory, "stock_export.csv");
            await File.WriteAllLinesAsync(path, lines, Encoding.UTF8, cancellationToken);
            _appLogger.LogMessage($"Данные склада успешно экспортированы в {path}");
        }
    }
}

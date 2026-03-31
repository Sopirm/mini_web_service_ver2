using Microsoft.Extensions.DependencyInjection;
using Pr2.ModulesAndDi.Core;
using Pr2.ModulesAndDi.Services;

namespace Pr2.ModulesAndDi.Modules;

/// <summary>
/// Базовый модуль приложения.
/// </summary>
public sealed class CoreModule : IAppModule
{
    public string Name => "Core";

    public IReadOnlyCollection<string> Requires => Array.Empty<string>();

    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IStorage, InMemoryStorage>();
        services.AddSingleton<IPartRepository, InMemoryPartRepository>();
        services.AddSingleton<IStockRepository, InMemoryStockRepository>();
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var partRepo = serviceProvider.GetRequiredService<IPartRepository>();
        var stockRepo = serviceProvider.GetRequiredService<IStockRepository>();

        // Предварительное наполнение данными для демонстрации
        var p1 = new Part(Guid.NewGuid(), "ART-001", "Масляный фильтр", "Фильтр для двигателя", 15.50m);
        var p2 = new Part(Guid.NewGuid(), "ART-002", "Тормозные колодки", "Передние тормозные колодки", 45.00m);
        var p3 = new Part(Guid.NewGuid(), "ART-003", "Свеча зажигания", "Иридиевая свеча зажигания", 12.80m);

        await partRepo.AddPartAsync(p1);
        await partRepo.AddPartAsync(p2);
        await partRepo.AddPartAsync(p3);

        await stockRepo.AddStockItemAsync(new StockItem(p1.Id, 10, "Стеллаж A-1"));
        await stockRepo.AddStockItemAsync(new StockItem(p2.Id, 5, "Стеллаж B-2"));
        await stockRepo.AddStockItemAsync(new StockItem(p3.Id, 24, "Стеллаж C-3"));
    }
}

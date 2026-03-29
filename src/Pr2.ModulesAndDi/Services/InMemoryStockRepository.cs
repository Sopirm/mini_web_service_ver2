using System.Collections.Concurrent;

namespace Pr2.ModulesAndDi.Core;

/// <summary>
/// Простая реализация репозитория склада в памяти.
/// </summary>
public sealed class InMemoryStockRepository : IStockRepository
{
    private readonly ConcurrentDictionary<Guid, StockItem> _stockItems = new();

    public Task AddStockItemAsync(StockItem item)
    {
        _stockItems.TryAdd(item.PartId, item);
        return Task.CompletedTask;
    }

    public Task<StockItem?> GetStockItemByPartIdAsync(Guid partId)
    {
        _stockItems.TryGetValue(partId, out var item);
        return Task.FromResult(item);
    }

    public Task<IReadOnlyList<StockItem>> GetAllStockItemsAsync()
    {
        return Task.FromResult<IReadOnlyList<StockItem>>(_stockItems.Values.ToArray());
    }

    public Task UpdateStockItemAsync(StockItem item)
    {
        _stockItems.AddOrUpdate(item.PartId, item, (key, oldValue) => item);
        return Task.CompletedTask;
    }

    public Task RemoveStockItemAsync(Guid partId)
    {
        _stockItems.TryRemove(partId, out _);
        return Task.CompletedTask;
    }

    public async Task IncreaseStockAsync(Guid partId, int quantity)
    {
        _stockItems.AddOrUpdate(partId,
            _ => new StockItem(partId, quantity, "Unknown"), // Если нет, создаем новый элемент
            (key, oldValue) => oldValue with { Quantity = oldValue.Quantity + quantity });
        await Task.CompletedTask;
    }

    public async Task DecreaseStockAsync(Guid partId, int quantity)
    {
        _stockItems.AddOrUpdate(partId,
            _ => throw new InvalidOperationException("Невозможно уменьшить количество: товар отсутствует на складе."),
            (key, oldValue) =>
            {
                if (oldValue.Quantity < quantity)
                    throw new InvalidOperationException($"Недостаточно товара на складе для PartId: {partId}. Доступно: {oldValue.Quantity}, требуется: {quantity}.");
                return oldValue with { Quantity = oldValue.Quantity - quantity };
            });
        await Task.CompletedTask;
    }
}

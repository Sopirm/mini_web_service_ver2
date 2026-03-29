using System.Collections.Concurrent;

namespace Pr2.ModulesAndDi.Core;

/// <summary>
/// Простая реализация репозитория запчастей в памяти.
/// </summary>
public sealed class InMemoryPartRepository : IPartRepository
{
    private readonly ConcurrentDictionary<Guid, Part> _parts = new();
    private readonly ConcurrentDictionary<string, Part> _partsByArticle = new(StringComparer.OrdinalIgnoreCase);

    public Task AddPartAsync(Part part)
    {
        _parts.TryAdd(part.Id, part);
        _partsByArticle.TryAdd(part.Article, part);
        return Task.CompletedTask;
    }

    public Task<Part?> GetPartByIdAsync(Guid id)
    {
        _parts.TryGetValue(id, out var part);
        return Task.FromResult(part);
    }

    public Task<Part?> GetPartByArticleAsync(string article)
    {
        _partsByArticle.TryGetValue(article, out var part);
        return Task.FromResult(part);
    }

    public Task<IReadOnlyList<Part>> GetAllPartsAsync()
    {
        return Task.FromResult<IReadOnlyList<Part>>(_parts.Values.ToArray());
    }

    public Task UpdatePartAsync(Part part)
    {
        _parts.AddOrUpdate(part.Id, part, (key, oldValue) => part);
        _partsByArticle.AddOrUpdate(part.Article, part, (key, oldValue) => part);
        return Task.CompletedTask;
    }

    public Task DeletePartAsync(Guid id)
    {
        _parts.TryRemove(id, out _);
        // Нужно найти артикул удаляемой запчасти, чтобы удалить ее из _partsByArticle
        var partToRemove = _parts.Values.FirstOrDefault(p => p.Id == id);
        if (partToRemove != null)
        {
            _partsByArticle.TryRemove(partToRemove.Article, out _);
        }
        return Task.CompletedTask;
    }
}

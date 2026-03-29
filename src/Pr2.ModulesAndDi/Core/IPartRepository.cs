namespace Pr2.ModulesAndDi.Core;

/// <summary>
/// Интерфейс для работы с запчастями.
/// </summary>
public interface IPartRepository
{
    Task AddPartAsync(Part part);
    Task<Part?> GetPartByIdAsync(Guid id);
    Task<Part?> GetPartByArticleAsync(string article);
    Task<IReadOnlyList<Part>> GetAllPartsAsync();
    Task UpdatePartAsync(Part part);
    Task DeletePartAsync(Guid id);
}

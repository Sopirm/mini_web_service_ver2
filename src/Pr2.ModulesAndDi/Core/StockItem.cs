namespace Pr2.ModulesAndDi.Core;

/// <summary>
/// Модель элемента склада.
/// </summary>
public record StockItem(Guid PartId, int Quantity, string Location);

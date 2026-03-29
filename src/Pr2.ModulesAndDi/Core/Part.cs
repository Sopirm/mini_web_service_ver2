namespace Pr2.ModulesAndDi.Core;

/// <summary>
/// Модель автозапчасти.
/// </summary>
public record Part(Guid Id, string Article, string Name, string Description, decimal Price);

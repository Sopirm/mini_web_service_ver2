using Microsoft.Extensions.Logging;
using Pr2.ModulesAndDi.Core;

namespace Pr2.ModulesAndDi.Services;

/// <summary>
/// Реализация IAppLogger для вывода в консоль.
/// </summary>
public sealed class ConsoleAppLogger : IAppLogger
{
    private readonly ILogger<ConsoleAppLogger> _logger;

    public ConsoleAppLogger(ILogger<ConsoleAppLogger> logger)
    {
        _logger = logger;
    }

    public void LogPartAdded(Part part)
    {
        _logger.LogInformation($"Добавлена запчасть: {part.Name} (Артикул: {part.Article}, ID: {part.Id})");
    }

    public void LogStockIncreased(Guid partId, int quantity, int newQuantity)
    {
        _logger.LogInformation($"Увеличен запас запчасти ID {partId} на {quantity}. Новое количество: {newQuantity}");
    }

    public void LogStockDecreased(Guid partId, int quantity, int newQuantity)
    {
        _logger.LogInformation($"Уменьшен запас запчасти ID {partId} на {quantity}. Новое количество: {newQuantity}");
    }

    public void LogMessage(string message)
    {
        _logger.LogInformation(message);
    }
}

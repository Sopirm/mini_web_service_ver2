using Pr2.ModulesAndDi.Core;

namespace Pr2.ModulesAndDi.Services;

/// <summary>
/// Сервис для логирования событий приложения, специфичных для предметной области.
/// </summary>
public interface IAppLogger
{
    void LogPartAdded(Part part);
    void LogStockIncreased(Guid partId, int quantity, int newQuantity);
    void LogStockDecreased(Guid partId, int quantity, int newQuantity);
    void LogMessage(string message);
}

using Microsoft.Extensions.DependencyInjection;
using Pr2.ModulesAndDi.Core;
using Xunit;

namespace Pr2.ModulesAndDi.Tests;

public sealed class ModuleCatalogTests
{
    [Fact]
    public void Порядок_запуска_учитывает_зависимости()
    {
        var a = new FakeModule("A", Array.Empty<string>());
        var b = new FakeModule("B", new[] { "A" });
        var c = new FakeModule("C", new[] { "B" });

        var all = new Dictionary<string, IAppModule>(StringComparer.OrdinalIgnoreCase)
        {
            [a.Name] = a,
            [b.Name] = b,
            [c.Name] = c
        };

        var order = ModuleCatalog.BuildExecutionOrder(all, new[] { "A", "B", "C" });

        Assert.Equal(new[] { "A", "B", "C" }, order.Select(m => m.Name).ToArray());
    }

    [Fact]
    public void Отсутствующий_модуль_даёт_понятную_ошибку()
    {
        var a = new FakeModule("A", Array.Empty<string>());

        var all = new Dictionary<string, IAppModule>(StringComparer.OrdinalIgnoreCase)
        {
            [a.Name] = a
        };

        var ex = Assert.Throws<ModuleLoadException>(() => ModuleCatalog.BuildExecutionOrder(all, new[] { "A", "B" }));
        Assert.Contains("Модуль не найден", ex.Message);
    }

    [Fact]
    public void Цикл_зависимостей_обнаруживается()
    {
        var a = new FakeModule("A", new[] { "B" });
        var b = new FakeModule("B", new[] { "A" });

        var all = new Dictionary<string, IAppModule>(StringComparer.OrdinalIgnoreCase)
        {
            [a.Name] = a,
            [b.Name] = b
        };

        var ex = Assert.Throws<ModuleLoadException>(() => ModuleCatalog.BuildExecutionOrder(all, new[] { "A", "B" }));
        Assert.Contains("циклическая", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Внедрение_зависимостей_работает()
    {
        var services = new ServiceCollection();
        services.AddSingleton<MarkerService>();

        var provider = services.BuildServiceProvider();

        var module = new FakeModule("A", Array.Empty<string>())
        {
            OnInit = sp =>
            {
                var s = sp.GetService<MarkerService>();
                Assert.NotNull(s);
            }
        };

        await module.InitializeAsync(provider, CancellationToken.None);
    }

    [Fact]
    public void Сложный_порядок_запуска_учитывает_разветвленные_зависимости()
    {
        // Структура: Core <- Logging, Core <- Validation, Validation <- Export, Export <- Report
        var core = new FakeModule("Core", Array.Empty<string>());
        var logging = new FakeModule("Logging", new[] { "Core" });
        var validation = new FakeModule("Validation", new[] { "Core" });
        var export = new FakeModule("Export", new[] { "Core", "Validation" });
        var report = new FakeModule("Report", new[] { "Core", "Export" });

        var all = new Dictionary<string, IAppModule>(StringComparer.OrdinalIgnoreCase)
        {
            ["Core"] = core,
            ["Logging"] = logging,
            ["Validation"] = validation,
            ["Export"] = export,
            ["Report"] = report
        };

        var enabled = new[] { "Report", "Export", "Validation", "Logging", "Core" };
        var order = ModuleCatalog.BuildExecutionOrder(all, enabled).Select(m => m.Name).ToList();

        // Проверяем относительный порядок:
        Assert.True(order.IndexOf("Core") < order.IndexOf("Logging"));
        Assert.True(order.IndexOf("Core") < order.IndexOf("Validation"));
        Assert.True(order.IndexOf("Validation") < order.IndexOf("Export"));
        Assert.True(order.IndexOf("Export") < order.IndexOf("Report"));
    }

    private sealed class MarkerService { }

    private sealed class FakeModule : IAppModule
    {
        public FakeModule(string name, IReadOnlyCollection<string> requires)
        {
            Name = name;
            Requires = requires;
        }

        public string Name { get; }

        public IReadOnlyCollection<string> Requires { get; }

        public Action<IServiceProvider>? OnInit { get; init; }

        public void RegisterServices(IServiceCollection services) { }

        public Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            OnInit?.Invoke(serviceProvider);
            return Task.CompletedTask;
        }
    }
}

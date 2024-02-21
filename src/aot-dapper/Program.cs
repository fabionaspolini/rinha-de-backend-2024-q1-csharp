using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using RinhaBackend_2024_q1_aot_dapper.Api;
using RinhaBackend_2024_q1_aot_dapper.Domain;

[module: DapperAot]

PrintStartupInfo();

var builder = WebApplication.CreateSlimBuilder(args);
var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:Rinha");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.AddScoped<DbConnection>(services => new NpgsqlConnection(connectionString));
builder.Services.AddKeyedScoped<DbConnection>("conn2", (services, key) => new NpgsqlConnection(connectionString));

#if USE_PROBLEM_DETAILS_EXCEPTION_HANDLER
builder.Services.AddProblemDetails();
builder.Services.Configure<RouteHandlerOptions>(o => o.ThrowOnBadRequest = true); // Para executar exeption handler em ambiente produtivo
#endif

#if RELEASE
builder.Logging.ClearProviders();
#endif

var app = builder.Build();

app.MapPost("/clientes/{id}/transacoes", ApiHandler.PostTransacoes);
app.MapGet("/clientes/{id}/extrato", ApiHandler.GetExtrato);
app.MapGet("/exception", context => throw new Exception("Teste exception."));

app.UseExceptionHandler(exceptionHandlerApp =>
    exceptionHandlerApp.Run(async context =>
    {
#if USE_PROBLEM_DETAILS_EXCEPTION_HANDLER
        var exception = context.Features.Get<IExceptionHandlerFeature>();
        var title = exception?.Error.Message;
        var details = exception?.Error.ToString();
        await Results.Problem(statusCode: 422, detail: details, title: title).ExecuteAsync(context);
#else
        await Results.UnprocessableEntity().ExecuteAsync(context);
#endif
    }));

await WarmUpAsync(app.Services);

app.Run();

void PrintStartupInfo()
{
#if DEBUG
    const string buildConfiguration = "Debug";
#else
    const string buildConfiguration = "Release";
#endif

#if ASYNC_METHODS
    const bool asyncMethods = true;
#else
    const bool asyncMethods = false;
#endif

#if USE_PROBLEM_DETAILS_EXCEPTION_HANDLER
    const bool useProblemDetailsExceptionHandler = true;
#else
    const bool useProblemDetailsExceptionHandler = false;
#endif

    Console.WriteLine("Rinha Backend 2024 Q1");
    Console.WriteLine($"Build configuration: {buildConfiguration}");
    Console.WriteLine($"Async Methods: {asyncMethods}");
    Console.WriteLine($"Use ProblemDetails Exception Handler: {useProblemDetailsExceptionHandler}");
    Console.WriteLine(new string('-', 60));
}

async Task WarmUpAsync(IServiceProvider services)
{
    Console.WriteLine("Warming Up app");

    var errorCount = 0;
    var ok = false;
    const int MaxRetry = 10;
    while (!ok && errorCount < MaxRetry)
    {
        try
        {
            using var scope = services.CreateScope();
            using var conn = scope.ServiceProvider.GetRequiredService<DbConnection>();
            using var conn2 = scope.ServiceProvider.GetRequiredKeyedService<DbConnection>("conn2");
            await conn.OpenAsync();
            await conn2.OpenAsync();
            using var trans = await conn.BeginTransactionAsync();
            try
            {
                var saldo = await conn.GetSaldoClienteAsync(1);
                var extrato = await conn.GetExtratoAsync(1);
                var result1 = await conn.InserirTransacaoCreditoAsync(1, 1, "teste");
                var result2 = await conn.InserirTransacaoDebitoAsync(2, 2, "teste");
            }
            finally
            {
                await trans.RollbackAsync();
            }

            var req = new TransacaoPostRequest(1, "c", "Teste");
            var valida = req.IsValid();
            ok = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error [{errorCount + 1}]: {ex.Message}");
            await Task.Delay(1000);
            errorCount++;
        }
    }

    if (errorCount == MaxRetry)
        throw new Exception("Falha no Warm Up da aplicação, bye...");

    Console.WriteLine("Warn Up OK");
    Console.WriteLine(new string('-', 60));
}

// Otimização para serializador JSON AOT
[JsonSerializable(typeof(TransacaoPostRequest))]
[JsonSerializable(typeof(TransacaoPostResponse))]
[JsonSerializable(typeof(ExtratoResponse))]
[JsonSerializable(typeof(ProblemDetails))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}

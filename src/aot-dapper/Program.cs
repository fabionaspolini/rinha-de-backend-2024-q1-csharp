using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using RinhaBackend_2024_q1_aot_dapper.Api;

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

#if !DEBUG
builder.Logging.ClearProviders();
#endif

var app = builder.Build();

app.MapPost("/clientes/{id}/transacoes", ApiHandler.HandlePostTransacoes);
app.MapGet("/clientes/{id}/extrato", ApiHandler.HandleGetExtrato);
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
    Console.WriteLine($"Build configuration: {buildConfiguration.ToUpper()}");
    Console.WriteLine($"Using {(asyncMethods ? "ASYNC" : "SYNC")} methods");
    Console.WriteLine($"UseProblemDetailsExceptionHandler: {useProblemDetailsExceptionHandler}");
    Console.WriteLine(new string('-', 60));
}

// Otimização para serializador JSON AOT
[JsonSerializable(typeof(TransacaoPostRequest))]
[JsonSerializable(typeof(TransacaoPostResponse))]
[JsonSerializable(typeof(ExtratoResponse))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(ProblemDetails))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}

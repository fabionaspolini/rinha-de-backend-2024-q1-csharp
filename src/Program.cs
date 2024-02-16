using System.Data.Common;
using System.Text.Json.Serialization;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using RinhaBackend_2024_q1.ApiModels;
using RinhaBackend_2024_q1.Domain;

[module: DapperAot]

var builder = WebApplication.CreateSlimBuilder(args);
var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:Rinha");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.AddScoped<DbConnection>(services => new NpgsqlConnection(connectionString));
builder.Services.AddKeyedScoped<DbConnection>("conn2", (services, key) => new NpgsqlConnection(connectionString));

var app = builder.Build();

app.MapPost("/clientes/{id}/transacoes", HandlePostTransacoesAsync);
app.MapGet("/clientes/{id}/extrato", HandleGetExtratoAsync);

app.Run();

async Task<IResult> HandlePostTransacoesAsync(HttpContext context, int id, [FromBody] TransacaoPostRequest request, [FromServices] DbConnection conn)
{
    if (string.IsNullOrEmpty(request.Descricao) || request.Descricao.Length > 10)
        return Results.UnprocessableEntity(new ErrorResponse("Descrição deve ter entre 1 e 10 caracteres."));
    if (!Constants.TiposTrasações.Contains(request.Tipo))
        return Results.UnprocessableEntity(new ErrorResponse("Tipo de transalção inválida."));

    var result = await conn.InserirTransacaoAsync(id, request.Tipo, request.Valor, request.Descricao);
    return result.Code switch
    {
        CriarTransacaoResultCode.Ok => Results.Ok(new TransacaoPostResponse(result.Limite!.Value, result.Saldo!.Value)),
        CriarTransacaoResultCode.ClienteInvalido => Results.NotFound(new ErrorResponse(result.Message)),
        _ => Results.UnprocessableEntity(new ErrorResponse(result.Message))
    };
}

async Task<IResult> HandleGetExtratoAsync(HttpContext context, int id,
    [FromServices] DbConnection conn,
    [FromKeyedServices("conn2")] DbConnection conn2)
{
    var saldoAtualTask = conn.GetSaldoClienteAsync(id);
    var extratoTask = conn2.GetExtratoAsync(id);

    var saldoAtual = await saldoAtualTask;
    if (saldoAtual == null)
        return Results.NotFound(new ErrorResponse("Cliente inválido."));

    return Results.Ok(new ExtratoResponse(
        Saldo: saldoAtual,
        UltimasTransacoes: await extratoTask));
}

// Otimização para serializador JSON AOT
[JsonSerializable(typeof(TransacaoPostRequest))]
[JsonSerializable(typeof(TransacaoPostResponse))]
[JsonSerializable(typeof(ExtratoResponse))]
[JsonSerializable(typeof(ErrorResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}

using System.Data;
using System.Data.Common;
using System.Text.Json.Serialization;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using RinhaBackend_2024_q1.Models;

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
app.MapGet("/clientes/{id}/extrato", HandleGetExtrato);

async Task<IResult> HandlePostTransacoesAsync(HttpContext context, int id, [FromBody] TransacaoPostRequest request, [FromServices] DbConnection conn)
{
    if (string.IsNullOrEmpty(request.Descricao) || request.Descricao.Length > 10)
        return Results.UnprocessableEntity(new ErrorResponse("Descrição deve ter entre 1 e 10 caracteres."));
    if (!Constants.TiposTrasações.Contains(request.Tipo))
        return Results.UnprocessableEntity(new ErrorResponse("Tipo de transalção inválida."));

    if (conn.State == ConnectionState.Closed)
        await conn.OpenAsync();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = $"select * from criar_transacao(@cliente_id, @tipo, @valor, @descricao)";
    cmd.AddParameter("@cliente_id", id);
    cmd.AddParameter("@tipo", request.Tipo);
    cmd.AddParameter("@valor", request.Valor);
    cmd.AddParameter("@descricao", request.Descricao);
    using var reader = await cmd.ExecuteReaderAsync();
    if (!reader.Read())
        return Results.UnprocessableEntity();

    var result = reader.ToDto()!;
    return result.Code switch
    {
        CriarTransacaoResultCode.Ok => Results.Ok(new TransacaoPostResponse(result.Limite!.Value, result.Saldo!.Value)),
        CriarTransacaoResultCode.ClienteInvalido => Results.NotFound(new ErrorResponse(result.Message)),
        _ => Results.UnprocessableEntity(new ErrorResponse(result.Message))
    };
}

async Task<IResult> HandleGetExtrato(HttpContext context, int id, 
    [FromServices] DbConnection conn,
    [FromKeyedServices("conn2")] DbConnection conn2)
{
    var saldoAtualTask = ApiQueries.GetSaldoClienteAsync(conn, id);
    var extratoTask = ApiQueries.GetExtratoAsync(conn2, id);

    return Results.Ok(new ExtratoResponse(
        Saldo: await saldoAtualTask,
        UltimasTransacoes: await extratoTask));
}

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
[JsonSerializable(typeof(TransacaoPostRequest))]
[JsonSerializable(typeof(TransacaoPostResponse))]
[JsonSerializable(typeof(ExtratoResponse))]
[JsonSerializable(typeof(ErrorResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}

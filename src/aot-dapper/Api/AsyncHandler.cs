#if ASYNC_METHODS
using Microsoft.AspNetCore.Mvc;
using RinhaBackend_2024_q1_aot_dapper.Domain;
using System.Data.Common;

namespace RinhaBackend_2024_q1_aot_dapper.Api;

public static class ApiHandler
{
    public static async Task<IResult> PostTransacoes(HttpContext context, int id, [FromBody] TransacaoPostRequest request, [FromServices] DbConnection conn)
    {
        var validacao = request.IsValid();
        if (!validacao.Valid)
            return Results.Problem(statusCode: 422, title: validacao.ErrorMessage);

        var result = request.TipoEnum == TipoTransacao.Credito
            ? await conn.InserirTransacaoCreditoAsync(id, request.ValorInt, request.Descricao)
            : await conn.InserirTransacaoDebitoAsync(id, request.ValorInt, request.Descricao);
        return result.Code switch
        {
            CriarTransacaoResultCode.Ok => Results.Ok(new TransacaoPostResponse(result.Limite!.Value, result.Saldo!.Value)),
            CriarTransacaoResultCode.ClienteInvalido => Results.Problem(statusCode: 404, title: result.Message),
            _ => Results.Problem(statusCode: 422, title: result.Message)
        };
    }

    public static async Task<IResult> GetExtrato(HttpContext context, int id,
        [FromServices] DbConnection conn,
        [FromKeyedServices("conn2")] DbConnection conn2)
    {
        var extratoTask = conn2.GetExtratoAsync(id);
        var saldoAtualTask = conn.GetSaldoClienteAsync(id);

        var saldoAtual = await saldoAtualTask;
        if (saldoAtual == null)
            return Results.Problem(statusCode: 404, title: "Cliente inválido.");

        return Results.Ok(new ExtratoResponse(
            Saldo: saldoAtual,
            UltimasTransacoes: await extratoTask));
    }
}
#endif
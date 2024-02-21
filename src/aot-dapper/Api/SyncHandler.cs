#if !ASYNC_METHODS
using Microsoft.AspNetCore.Mvc;
using RinhaBackend_2024_q1_aot_dapper.Domain;
using System.Data.Common;

namespace RinhaBackend_2024_q1_aot_dapper.Api;

public static class ApiHandler
{
    public static IResult PostTransacoes(HttpContext context, int id, [FromBody] TransacaoPostRequest request, [FromServices] DbConnection conn)
    {
        var validacao = request.IsValid();
        if (!validacao.Valid)
            return Results.Problem(statusCode: 422, title: validacao.ErrorMessage);

        var result = request.TipoEnum == TipoTransacao.Credito
            ? conn.InserirTransacaoCredito(id, request.ValorInt, request.Descricao)
            : conn.InserirTransacaoDebito(id, request.ValorInt, request.Descricao);
        return result.Code switch
        {
            CriarTransacaoResultCode.Ok => Results.Ok(new TransacaoPostResponse(result.Limite!.Value, result.Saldo!.Value)),
            CriarTransacaoResultCode.ClienteInvalido => Results.Problem(statusCode: 404, title: result.Message),
            _ => Results.Problem(statusCode: 422, title: result.Message)
        };
    }

    public static IResult GetExtrato(HttpContext context, int id,
        [FromServices] DbConnection conn)
    {
        var saldoAtual = conn.GetSaldoCliente(id);
        if (saldoAtual == null)
            return Results.Problem(statusCode: 404, title: "Cliente inválido.");

        var extrato = conn.GetExtrato(id);
        return Results.Ok(new ExtratoResponse(
            Saldo: saldoAtual,
            UltimasTransacoes: extrato));
    }
}
#endif
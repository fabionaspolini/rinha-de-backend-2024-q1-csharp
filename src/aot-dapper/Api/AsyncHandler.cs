﻿#if ASYNC_METHODS
using Microsoft.AspNetCore.Mvc;
using RinhaBackend_2024_q1_aot_dapper.Domain;
using System.Data.Common;

namespace RinhaBackend_2024_q1_aot_dapper.Api;

public static class ApiHandler
{
    public static async Task<IResult> HandlePostTransacoes(HttpContext context, int id, [FromBody] TransacaoPostRequest request, [FromServices] DbConnection conn)
    {
        if (string.IsNullOrEmpty(request.Descricao) || request.Descricao.Length > 10)
            return Results.UnprocessableEntity(new ErrorResponse("Descrição deve ter entre 1 e 10 caracteres."));
        if (!Constants.TiposTrasacoes.Contains(request.Tipo))
            return Results.UnprocessableEntity(new ErrorResponse("Tipo de transação inválida."));
        if (!int.TryParse(request.Valor?.ToString(), out var valorInt))
            return Results.Problem(statusCode: 422, title: "Valor deve ser um inteiro válido.");

        var result = request.Tipo == "c"
            ? await conn.InserirTransacaoCreditoAsync(id, valorInt, request.Descricao)
            : await conn.InserirTransacaoDebitoAsync(id, valorInt, request.Descricao);
        return result.Code switch
        {
            CriarTransacaoResultCode.Ok => Results.Ok(new TransacaoPostResponse(result.Limite!.Value, result.Saldo!.Value)),
            CriarTransacaoResultCode.ClienteInvalido => Results.NotFound(new ErrorResponse(result.Message)),
            _ => Results.UnprocessableEntity(new ErrorResponse(result.Message))
        };
    }

    public static async Task<IResult> HandleGetExtrato(HttpContext context, int id,
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
}
#endif
using Dapper;
using System.Data.Common;

namespace RinhaBackend_2024_q1.Models;

public static class Constants
{
    public static readonly string[] TiposTrasações = ["c", "d"];
}

public enum CriarTransacaoResultCode
{
    Ok = 0,
    ClienteInvalido = 1,
    SaldoInsuficiente = 2
}

public record CriarTransacaoResult(
    int CodeInt,
    string? Message,
    int? Saldo,
    int? Limite)
{
    public CriarTransacaoResultCode Code { get; } = (CriarTransacaoResultCode)CodeInt;
};

public static class DatabaseFunctions
{
    public static Task<CriarTransacaoResult> CriarTransacaoAsync(this DbConnection conn, int clienteId, string tipo, int valor, string descricao)
    {
        const string sql =
        """
            select result_code as code_int, result_message as message, saldo, limite
            from criar_transacao(@cliente_id, @tipo, @valor, @descricao)
        """;
        return conn.QueryFirstAsync<CriarTransacaoResult>(sql, new
        {
            cliente_id = clienteId,
            tipo,
            valor,
            descricao
        });
    }
}
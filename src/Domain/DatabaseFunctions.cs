using Dapper;
using System.Data.Common;

namespace RinhaBackend_2024_q1.Domain;

public static class DatabaseFunctions
{
    public static Task<CriarTransacaoResult> InserirTransacaoAsync(this DbConnection conn, int clienteId, string tipo, int valor, string descricao)
    {
        const string sql =
        """
            select result_code as code_int, result_message as message, saldo, limite
            from inserir_transacao(@cliente_id, @tipo, @valor, @descricao)
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
using Dapper;
using System.Data.Common;

namespace RinhaBackend_2024_q1_aot_dapper.Domain;

public static class DatabaseFunctions
{
    private const string InserirTransacaoSql =
    """
        select result_code as code_int, result_message as message, saldo, limite
        from inserir_transacao(@cliente_id, @tipo, @valor, @descricao)
    """;

#if ASYNC_METHODS
    public static Task<CriarTransacaoResult> InserirTransacaoAsync(this DbConnection conn, int clienteId, string tipo, int valor, string descricao) =>
        conn.QueryFirstAsync<CriarTransacaoResult>(InserirTransacaoSql, new
        {
            cliente_id = clienteId,
            tipo,
            valor,
            descricao
        });
#else
    public static CriarTransacaoResult InserirTransacao(this DbConnection conn, int clienteId, string tipo, int valor, string descricao) =>
        conn.QueryFirst<CriarTransacaoResult>(InserirTransacaoSql, new
        {
            cliente_id = clienteId,
            tipo,
            valor,
            descricao
        });
#endif
}
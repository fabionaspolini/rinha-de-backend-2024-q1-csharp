using Dapper;
using System.Data.Common;

namespace RinhaBackend_2024_q1_aot_dapper.Domain;

public static class DatabaseFunctions
{
    private const string InserirTransacaoCreditoSql =
        """
        select result_code as code_int, result_message as message, saldo, limite
        from inserir_transacao_credito(@cliente_id, @valor, @descricao)
        """;

    private const string InserirTransacaoDebitoSql =
        """
        select result_code as code_int, result_message as message, saldo, limite
        from inserir_transacao_debito(@cliente_id, @valor, @descricao)
        """;

#if ASYNC_METHODS
    public static Task<CriarTransacaoResult> InserirTransacaoCreditoAsync(this DbConnection conn, int clienteId, int valor, string descricao) =>
        conn.QueryFirstAsync<CriarTransacaoResult>(InserirTransacaoCreditoSql, new
        {
            cliente_id = clienteId,
            valor,
            descricao
        });

    public static Task<CriarTransacaoResult> InserirTransacaoDebitoAsync(this DbConnection conn, int clienteId, int valor, string descricao) =>
        conn.QueryFirstAsync<CriarTransacaoResult>(InserirTransacaoDebitoSql, new
        {
            cliente_id = clienteId,
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
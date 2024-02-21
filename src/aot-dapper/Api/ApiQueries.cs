using System.Data.Common;
using Dapper;

namespace RinhaBackend_2024_q1_aot_dapper.Api;

public static class ApiQueries
{
    private const string SaldoClienteSql =
        """
        select saldo as total, limite
        from cliente
        where id = @id
        """;

    private const string ExtratoSql =
        """
        select t.valor, t.tipo, t.descricao, t.realizada_em
        from transacao t
        where t.cliente_id = @cliente_id
        order by t.realizada_em desc
        limit 10
        """;

#if ASYNC_METHODS
    public static Task<SaldoExtratoModel?> GetSaldoClienteAsync(this DbConnection conn, int clienteId)
        => conn.QueryFirstOrDefaultAsync<SaldoExtratoModel>(SaldoClienteSql, new { id = clienteId });

    public static Task<IEnumerable<TransacaoModel>> GetExtratoAsync(this DbConnection conn, int clienteId) =>
        conn.QueryAsync<TransacaoModel>(ExtratoSql, new { cliente_id = clienteId });
#else
    public static SaldoExtratoModel? GetSaldoCliente(this DbConnection conn, int clienteId)
        => conn.QueryFirstOrDefault<SaldoExtratoModel>(SaldoClienteSql, new { id = clienteId });

    public static IEnumerable<TransacaoModel> GetExtrato(this DbConnection conn, int clienteId) =>
        conn.Query<TransacaoModel>(ExtratoSql, new { cliente_id = clienteId });
#endif
}
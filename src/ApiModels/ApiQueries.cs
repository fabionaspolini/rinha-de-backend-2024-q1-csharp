using System.Data.Common;
using Dapper;

namespace RinhaBackend_2024_q1.ApiModels;

public static class ApiQueries
{
    public static Task<SaldoExtratoModel?> GetSaldoClienteAsync(this DbConnection conn, int clienteId)
    {
        const string sql =
        """
            select saldo as total, limite
            from cliente
            where id = @id
        """;
        return conn.QueryFirstOrDefaultAsync<SaldoExtratoModel>(sql, new { id = clienteId });
    }

    public static Task<IEnumerable<TransacaoModel>> GetExtratoAsync(this DbConnection conn, int clienteId)
    {
        const string sql =
        """
            select t.valor, t.tipo, t.descricao, t.realizada_em
            from transacao t
            where t.cliente_id = @cliente_id
            order by t.realizada_em desc
            limit 10
        """;
        return conn.QueryAsync<TransacaoModel>(sql, new { cliente_id = clienteId });
    }
}
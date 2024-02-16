using System.Data.Common;
using Dapper;

namespace RinhaBackend_2024_q1.Models;

public record class TransacaoPostRequest(int Valor, string Tipo, string Descricao);
public record class TransacaoPostResponse(int Limite, int Saldo);

public record class SaldoExtratoModel(int Total, int Limite)
{
    public DateTime DataExtrato { get; } = DateTime.Now;
};

public record class TransacaoModel(int Valor, string Tipo, string Descricao, DateTime RealizadaEm);

public record class ExtratoResponse(SaldoExtratoModel Saldo, IEnumerable<TransacaoModel> UltimasTransacoes);

public record class ErrorResponse(string? Message);


public static class ApiQueries
{
    public static Task<SaldoExtratoModel?> GetSaldoClienteAsync(this DbConnection conn, int clienteId)
    {
        const string sql =
        """
            select saldo as total, limite
            from cliente where id = @id
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
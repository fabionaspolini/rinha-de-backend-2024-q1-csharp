using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;

[module: DapperAot]

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
    public static Task<SaldoExtratoModel> GetSaldoClienteAsync(DbConnection conn, int clienteId) =>
        conn.QueryFirstAsync<SaldoExtratoModel>(
            @"select saldo as total, limite from cliente where id = @id", new { id = clienteId });

    /*public static async Task<SaldoExtratoModel?> GetSaldoClienteNativeAsync(DbConnection conn, int clienteId)
    {
        if (conn.State == ConnectionState.Closed)
            await conn.OpenAsync();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "select saldo as total, limite from cliente where id = @id";
        cmd.AddParameter("@id", clienteId);
        using var reader = await cmd.ExecuteReaderAsync();
        if (reader.Read())
            return new SaldoExtratoModel(reader.GetInt32(0), reader.GetInt32(1));
        return null;
    }*/

    public static Task<IEnumerable<TransacaoModel>> GetExtratoAsync(DbConnection conn, int clienteId) =>
        conn.QueryAsync<TransacaoModel>(
            @"select t.valor, t.tipo, t.descricao, t.realizada_em
from transacao t
where t.cliente_id = @clienteId
order by t.realizada_em desc
limit 10", new { clienteId });
}
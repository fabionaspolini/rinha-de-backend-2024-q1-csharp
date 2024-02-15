using Npgsql;
using System.Data;
using System.Data.Common;

namespace RinhaBackend_2024_q1.Models;

public enum CriarTransacaoResultCode
{
    Ok = 0,
    ClienteInvalido = 1,
    SaldoInsuficiente = 2
}

public record CriarTransacaoResult(
    CriarTransacaoResultCode Code,
    string? Message,
    int? Saldo,
    int? Limite);

public static class ModelsExtensions
{
    public static CriarTransacaoResult ToDto(this IDataReader reader) =>
        new CriarTransacaoResult(
            Code: (CriarTransacaoResultCode)reader.GetInt32(0),
            Message: reader.IsDBNull(1) ? null : reader.GetString(1),
            Saldo: reader.IsDBNull(2) ? null : reader.GetInt32(2),
            Limite: reader.IsDBNull(3) ? null : reader.GetInt32(3));
}
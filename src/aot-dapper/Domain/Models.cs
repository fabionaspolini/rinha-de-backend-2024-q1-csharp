namespace RinhaBackend_2024_q1_aot_dapper.Domain;

public static class Constants
{
    public static readonly string[] TiposTrasacoes = ["c", "d"];
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
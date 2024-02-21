namespace RinhaBackend_2024_q1_aot_dapper.Domain;

public enum TipoTransacao
{
    Invalida,
    Credito = 'c',
    Debito = 'd'
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

namespace RinhaBackend_2024_q1_aot_dapper.Api;

public record class TransacaoPostRequest(object Valor, string Tipo, string Descricao);
public record class TransacaoPostResponse(int Limite, int Saldo);

public record class SaldoExtratoModel(int Total, int Limite)
{
    public DateTime DataExtrato { get; } = DateTime.UtcNow;
};

public record class TransacaoModel(int Valor, string Tipo, string Descricao, DateTime RealizadaEm);

public record class ExtratoResponse(SaldoExtratoModel Saldo, IEnumerable<TransacaoModel> UltimasTransacoes);

public record class ErrorResponse(string? Message);

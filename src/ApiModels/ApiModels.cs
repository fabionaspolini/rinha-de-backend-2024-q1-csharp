namespace RinhaBackend_2024_q1.ApiModels;

public record class TransacaoPostRequest(int Valor, string Tipo, string Descricao);
public record class TransacaoPostResponse(int Limite, int Saldo);

public record class SaldoExtratoModel(int Total, int Limite)
{
    public DateTime DataExtrato { get; } = DateTime.Now;
};

public record class TransacaoModel(int Valor, string Tipo, string Descricao, DateTime RealizadaEm);

public record class ExtratoResponse(SaldoExtratoModel Saldo, IEnumerable<TransacaoModel> UltimasTransacoes);

public record class ErrorResponse(string? Message);

using RinhaBackend_2024_q1_aot_dapper.Domain;

namespace RinhaBackend_2024_q1_aot_dapper.Api;

public record class TransacaoPostRequest(object Valor, string Tipo, string Descricao)
{
    private int _valorInt;
    private TipoTransacao _tipoEnum;

    public int ValorInt => _valorInt;
    public TipoTransacao TipoEnum => _tipoEnum;

    public (bool Valid, string? ErrorMessage) IsValid()
    {
        if (string.IsNullOrEmpty(Descricao) || Descricao.Length > 10)
            return (false, "Descri��o deve ter entre 1 e 10 caracteres.");
        if (!int.TryParse(Valor?.ToString(), out _valorInt))
            return (false, "Valor deve ser um inteiro v�lido.");

        _tipoEnum = Tipo switch
        {
            "c" => TipoTransacao.Credito,
            "d" => TipoTransacao.Debito,
            _ => TipoTransacao.Invalida
        };
        if (_tipoEnum == TipoTransacao.Invalida)
            return (false, "Tipo de transa��o inv�lida.");
        return (true, null);
    }
}
public record class TransacaoPostResponse(int Limite, int Saldo);

public record class SaldoExtratoModel(int Total, int Limite)
{
    public DateTime DataExtrato { get; } = DateTime.UtcNow;
};

public record class TransacaoModel(int Valor, string Tipo, string Descricao, DateTime RealizadaEm);

public record class ExtratoResponse(SaldoExtratoModel Saldo, IEnumerable<TransacaoModel> UltimasTransacoes);

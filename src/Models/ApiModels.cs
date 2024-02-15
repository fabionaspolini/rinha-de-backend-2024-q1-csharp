namespace RinhaBackend_2024_q1.Models;

public record class TransacaoPostRequest(int Valor, string Tipo, string Descricao);
public record class TransacaoPostResponse(int Limite, int Saldo);

public record class SaldoModel(int Total, DateTime DataExtrato, int Limite);

public record class TransacaoModel(int Valor, string Tipo, string Descricao, DateTime RealizadaEm);

public record class ExtratoResponse(SaldoModel Saldo, TransacaoModel[] UltimasTransacoes);

public record class ErrorResponse(string? Message);
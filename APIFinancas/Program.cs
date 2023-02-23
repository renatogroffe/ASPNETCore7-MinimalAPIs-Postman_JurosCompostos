using Microsoft.OpenApi.Models;
using APIFinancas.Models;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "APIFinancas",
            Description = "API para calculo de Juros Compostos", 
            Version = "v1",
            Contact = new OpenApiContact()
            {
                Name = "Renato Groffe",
                Url = new Uri("https://github.com/renatogroffe"),
            },
            License = new OpenApiLicense()
            {
                Name = "MIT",
                Url = new Uri("http://opensource.org/licenses/MIT"),
            }
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "APITemperaturas v1");
});

app.MapGet("/calculofinanceiro/juroscompostos",
    Results<Ok<Emprestimo>, BadRequest<FalhaCalculo>> 
    (double valorEmprestimo, int numMeses, double percTaxa) =>
    {
        app.Logger.LogInformation(
            "Recebida nova requisicao|" +
           $"Valor do emprestimo: {valorEmprestimo}|" +
           $"%Taxa de Juros: {percTaxa}");

        if (valorEmprestimo <= 0 || numMeses <= 0 || percTaxa <= 0)
            throw new Exception("Parâmetros para cálculo inválidos!");
        // FIXME: Codigo comentado para simulacaoo de falhas em testes automatizados
        /*if (valorEmprestimo <= 0)
            return GerarResultParamInvalido("Valor do Emprestimo");
        if (numMeses <= 0)
            return GerarResultParamInvalido("Numero de Meses");
        if (percTaxa <= 0)
            return GerarResultParamInvalido("Percentual da Taxa de Juros");*/

        // FIXME: Simulação de falha
        var valorFinalJuros =
            valorEmprestimo * Math.Pow(1 + (percTaxa / 100), numMeses);
        //var valorFinalJuros = Math.Round(
        //    valorEmprestimo * Math.Pow(1 + (percTaxa / 100), numMeses), 2);
        app.Logger.LogInformation($"Valor Final com Juros: {valorFinalJuros}");

        return TypedResults.Ok(new Emprestimo()
        {
            ValorEmprestimo = valorEmprestimo,
            NumMeses = numMeses,
            TaxaPercentual = percTaxa,
            ValorFinalComJuros = valorFinalJuros
        });
    });

app.Run();

BadRequest<FalhaCalculo> GerarResultParamInvalido(string nomeCampo)
{
    var erro = $"O {nomeCampo} deve ser maior do que zero!";
    app!.Logger.LogError(erro);
    return TypedResults.BadRequest(
        new FalhaCalculo() { Mensagem = erro });
}
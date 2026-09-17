using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class DiasUteisCalculator
{
    public static DateTime hoje = DateTime.Today;
    public static int anoAtual = hoje.Year;
    public static int mesAtual = hoje.Month;
    public static int diasTrabalhados = 0;
    public static int diasNoMes;

    public static (int uteis, int naoUteis, List<int> arrayDiasUteis) CalcularDiasUteis()
    {
        int uteis = 0;
        int naoUteis = 0;
        var arrayDiasUteis = new List<int>();
        diasNoMes = DateTime.DaysInMonth(anoAtual, mesAtual);

        var feriados = new List<DateTime>
        {
            new DateTime(anoAtual, 1, 1),   // Confraternização Universal
            new DateTime(anoAtual, 4, 18),  // Sexta-feira Santa
            new DateTime(anoAtual, 4, 21),  // Tiradentes
            new DateTime(anoAtual, 3, 4),   // Carnaval
            new DateTime(anoAtual, 5, 1),   // Dia do Trabalho
            new DateTime(anoAtual, 9, 7),   // Independência
            new DateTime(anoAtual, 10, 12), // Nossa Senhora Aparecida
            new DateTime(anoAtual, 11, 2),  // Finados
            new DateTime(anoAtual, 11, 15), // Proclamação da República
            new DateTime(anoAtual, 12, 25), // Natal
            new DateTime(anoAtual, 11, 20), // Consciência Negra
        };

        for (int dia = 1; dia <= diasNoMes; dia++)
        {
            var data = new DateTime(anoAtual, mesAtual, dia);
            var diaSemana = data.DayOfWeek;

            bool fimDeSemana = diaSemana == DayOfWeek.Saturday || diaSemana == DayOfWeek.Sunday;
            bool feriado = feriados.Any(f => f.Date == data.Date);

            if (!fimDeSemana && !feriado)
            {
                arrayDiasUteis.Add(dia);
                uteis++;
            }
            else
            {
                naoUteis++;
            }
        }

        return (uteis, naoUteis, arrayDiasUteis);
    }

    public static int calcDiasTrabalhados()
    {

        var calcDias = CalcularDiasUteis().uteis;
        var totalDiasUteisArray = CalcularDiasUteis().arrayDiasUteis;
        int diasTrabalhados = 0;

        // Se for o mês atual → conta apenas os dias úteis anteriores ao dia de hoje

        foreach (var dia in totalDiasUteisArray)
        {
            if (dia < hoje.Day)
            {
                diasTrabalhados++;
            }
        }

        return diasTrabalhados;
    }
}

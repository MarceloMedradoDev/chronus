using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.Interfaces;
using Chronos.Structs;

namespace Chronos.Utils
{
    public class CalcDate
    {
        private readonly ICalcSugestions _calcSugestions;
        public DateTime hoje = DateTime.Today;
        public KeyValuePair<string, string> acumulados;
        public string metasJR;
        public CalcDate(ICalcSugestions calcSugestions)
        {
            _calcSugestions = calcSugestions;
        }
        public int calcularSugestoes()
        {
            var diasUteis = DiasUteisCalculator.CalcularDiasUteis();
            var trabalhados = DiasUteisCalculator.calcDiasTrabalhados();
            EmployeeHours employeeHours = new();


            int periodoSugerido = (int)Math.Floor(diasUteis.uteis / 2.0);

            int diasFaltantes = diasUteis.uteis - trabalhados;

            if (hoje.Day >= periodoSugerido)
            {
                var calc = _calcSugestions.calculateSugestions();

                foreach (var item in calc.Result)
                {
                    acumulados = item;

                    if (acumulados.Key == "Acc JR")
                    {

                    }
                }
            }


            return periodoSugerido;
        }
    }
}

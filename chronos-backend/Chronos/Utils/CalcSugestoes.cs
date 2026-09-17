using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.Interfaces;
using Chronos.Structs;
using DocumentFormat.OpenXml.Office.Word;

namespace Chronos.Utils
{
    public class CalcSugestoes : ICalcSugestions
    {
        private readonly IEmployeesRepository _employeeRepository;
        public CalcSugestoes(IEmployeesRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task<Dictionary<string, string>> calculateSugestions()
        {

            EmployeeHours employeeHours = new();

            Dictionary<string, string> acumuladosList = new Dictionary<string, string>();

            var metaJR = employeeHours.MetaJR;
            var metaPL = employeeHours.MetaPL;
            var metaSR = employeeHours.MetaSR;


            TimeSpan horaAcumulada = TimeSpan.Zero;

            string acumuladosJR = string.Empty;
            string acumuladosPL = string.Empty;
            string acumuladosSR = string.Empty;


            var employee = await _employeeRepository.GetTotalHours();

            foreach (var item in employee)
            {
                string horasStr = item.TotalHours?.Trim();

                if (!string.IsNullOrEmpty(horasStr))
                {
                    var partes = horasStr.Split(':');
                    if (partes.Length == 2 &&
                        int.TryParse(partes[0], out int horas) &&
                        int.TryParse(partes[1], out int minutos))
                    {
                        horaAcumulada += new TimeSpan(horas, minutos, 0);
                        string horasFormatadas = $"{(int)horaAcumulada.TotalHours:D2}:{horaAcumulada.Minutes:D2}";

                        if (item.Role == "ANL TI JR")
                        {
                            acumuladosJR = horasFormatadas;
                        }
                        else if (item.Role == "ANL TI PL")
                        {
                            acumuladosPL = horasFormatadas;
                        }
                        else if (item.Role == "ANL TI SR")
                        {
                            acumuladosSR = horasFormatadas;
                        }

                    }
                }

            }
            acumuladosList.Add("Acc JR", acumuladosJR);
            acumuladosList.Add("Acc PL", acumuladosPL);
            acumuladosList.Add("Acc SR", acumuladosSR);


            return acumuladosList;
        }
    }
}

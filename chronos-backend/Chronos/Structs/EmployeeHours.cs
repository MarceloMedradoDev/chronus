using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronos.Structs
{
    public struct EmployeeHours
    {
        public int MetaJR { get; private set; } = 880;
        public int MetaPL { get; private set; } = 528;
        public int MetaSR { get; private set; } = 352;
        public int DiarioJR { get; private set; } = 44;
        public int DiarioPL { get; private set; } = 26;
        public int DiarioSR { get; private set; } = 17;

        public int DiasUteis { get; private set; }

        public EmployeeHours()
        {
            DiasUteis = DiasUteisCalculator.CalcularDiasUteis().uteis;
        }

        public Dictionary<string, int> CalcHoras()
        {
            Dictionary<string, int> horasPrevistas = new Dictionary<string, int>();

            horasPrevistas.Add("Horas previstas JR", this.DiarioJR * this.DiasUteis);
            horasPrevistas.Add("Horas previstas PL", this.DiarioPL * this.DiasUteis);
            horasPrevistas.Add("Horas previstas SR", this.DiarioSR * this.DiasUteis);

            horasPrevistas.Add("Horas Totais JR", this.MetaJR);
            horasPrevistas.Add("Horas totais PL", this.MetaPL);
            horasPrevistas.Add("Horas totais SR", this.MetaSR);

            return horasPrevistas;
        }


    }
}

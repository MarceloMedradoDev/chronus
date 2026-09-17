using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chronos.Interfaces
{
    public interface ICalcSugestions
    {
        Task<Dictionary<string, string>> calculateSugestions();
    }
}

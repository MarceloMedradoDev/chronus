using Chronos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Interfaces
{
    public interface IPontoMaisService
    {
        Task StartPontoMaisService();
        Task<bool> StartPontoMaisService(bool isReprocess);
        Task AuthPontoMais();
        Task GetEmployees();
        Task WorkDays();
    }
}

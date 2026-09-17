using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Chronos.DTOs;
using Chronos.Models;

namespace Chronos.Interfaces
{
    public interface IEmployeesRepository
    {
        Task<IEnumerable<EmployeeModel>> GetTotalHours();
        Task<IEnumerable<EmployeeModel>> getIndividual(int matricula);
        Task InsertEmployeesAsync(IEnumerable<EmployeeModel> employee);
    }
}

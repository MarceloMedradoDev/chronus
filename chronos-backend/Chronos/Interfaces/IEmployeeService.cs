using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.DTOs;
using Chronos.Models;

namespace Chronos.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<ResponseModel<EmployeeReturnDTO>>> GetTotalHours();
        Task<IEnumerable<ResponseModel<EmployeeModel>>> reports();
        Task<IEnumerable<ResponseModel<EmployeeModel>>> getIndividual(int id);
        Task<IEnumerable<ResponseModel<EmployeeModel>>> InsertEmployeesAsync(IFormFile file);
    }
}

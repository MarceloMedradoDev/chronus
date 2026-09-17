using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.Models;

namespace Chronos.Interfaces
{
    public interface IFileService
    {
        IEnumerable<EmployeeModel> ReadFile(IFormFile file);
    }
}

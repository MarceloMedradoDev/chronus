using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.Context;
using Chronos.DTOs;
using Chronos.Interfaces;
using Chronos.Models;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Repositories
{
	public class EmployeesRepository : IEmployeesRepository
	{
		private readonly AppDbContext _context;
		public EmployeesRepository(AppDbContext context)
		{
			_context = context;
		}

        public async Task<IEnumerable<EmployeeModel>> getIndividual(int matricula)
        {
            var a = await _context.employees.Where(x => x.Registration == matricula).ToListAsync();
            return await _context.employees.Where(x => x.Registration == matricula).ToListAsync();
        }

        public async Task<IEnumerable<EmployeeModel>> GetTotalHours()
		{
			return await _context.employees.ToListAsync();
		}

		public async Task InsertEmployeesAsync(IEnumerable<EmployeeModel> employees)
{
    try
    {
        foreach (var employee in employees)
        {
            var existingEmployee = await _context.employees
                .FirstOrDefaultAsync(e => e.Id == employee.Id);

            if (existingEmployee != null)
            {
                        // Atualiza os campos
                        //existingEmployee.Name = employee.Name;
                        existingEmployee.Id = employee.Id;
                        existingEmployee.TotalHours = employee.TotalHours;
                        existingEmployee.IsCompleteDay = employee.IsCompleteDay;
                        existingEmployee.Day = employee.Day;

                _context.employees.Update(existingEmployee);
            }
            else
            {
                // Adiciona novo
                _context.employees.Add(employee);
            }
        }

        await _context.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
        var innerMessage = ex.InnerException?.Message;
        Console.WriteLine("Erro ao salvar no banco: " + innerMessage);
        throw;
    }
}


	}
}

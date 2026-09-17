using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chronos.DTOs;
using Chronos.Interfaces;
using Chronos.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;

namespace Chronos.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeesRepository _repository;
        private readonly IFileService _fileService;
        private readonly IMemoryCache _memoryCache;
        private const string CACHE = "employees_list";

        public EmployeeService(IEmployeesRepository repository, IFileService fileService, IMemoryCache memoryCache)
        {
            _repository = repository;
            _fileService = fileService;
            _memoryCache = memoryCache;
        }
        public async Task<IEnumerable<ResponseModel<EmployeeReturnDTO>>> GetTotalHours()
        {
            var result = new List<ResponseModel<EmployeeReturnDTO>>();


           if (_memoryCache.TryGetValue(CACHE, out IEnumerable<EmployeeModel>? employees))
            {
                return employees.Select(item => new ResponseModel<EmployeeReturnDTO>
                {
                    Success = true,
                    Message = "Busca em cache realizada com sucesso.",
                    Data = new EmployeeReturnDTO
                    {
                        Role = item.Role,
                        TotalHours = item.TotalHours,
                        IsCompleteDay = item.IsCompleteDay
                    }
                });
            }

            var response = await _repository.GetTotalHours();

            if (response == null || !response.Any())
            {
                return new List<ResponseModel<EmployeeReturnDTO>>
                {
                    new ResponseModel<EmployeeReturnDTO>
                    {
                        Data = null,
                        Message = "Nenhum funcionário encontrado",
                        Success = false
                    }
                };
            }

            foreach (var item in response)
            {
                result.Add(new ResponseModel<EmployeeReturnDTO>
                {
                    Success = true,
                    Message = "Busca realizada com sucesso.",
                    Data = new EmployeeReturnDTO
                    {
                        Role = item.Role,
                        TotalHours = item.TotalHours,
                        IsCompleteDay = item.IsCompleteDay
                    }
                });
            }

             var memoryCacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(14),
                    SlidingExpiration = TimeSpan.FromHours(8)
                };

                _memoryCache.Set(CACHE, response, memoryCacheEntryOptions);

            return result;
        }

        public async Task<IEnumerable<ResponseModel<EmployeeModel>>> InsertEmployeesAsync(IFormFile file)
        {
            var result = new List<ResponseModel<EmployeeModel>>();

            try
            {
                var data = _fileService.ReadFile(file);

                await _repository.InsertEmployeesAsync(data);

                foreach (var item in data)
                {
                    result.Add(new ResponseModel<EmployeeModel>
                    {
                        Success = true,
                        Message = "Inserido com sucesso.",
                        Data = new EmployeeModel
                        {
                            Role = item.Role,
                            TotalHours = item.TotalHours,
                            Registration = item.Registration,
                            Name = item.Name
                        }
                    });

                }
            }
            catch (Exception e)
            {
                result.Add(new ResponseModel<EmployeeModel>
                {
                    Success = false,
                    Message = e.Message,
                    Data = null
                });
            }

            return result;

        }

        public async Task<IEnumerable<ResponseModel<EmployeeModel>>> reports()
        {

            var result = new List<ResponseModel<EmployeeModel>>();

            var response = await _repository.GetTotalHours();

            foreach(var item in response)
            {
                result.Add(new ResponseModel<EmployeeModel>
                {
                    Success = true,
                    Message = "Busca realizada com sucesso.",
                    Data = new EmployeeModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Registration = item.Registration,
                        Role = item.Role,
                        TotalHours = item.TotalHours,
                        Day = item.Day,
                        IsCompleteDay = item.IsCompleteDay,
                    }
                });
            }

            return result;

        }

        public async Task<IEnumerable<ResponseModel<EmployeeModel>>> getIndividual(int matricula)
        {
            var result = new List<ResponseModel<EmployeeModel>>();

            var response = await _repository.getIndividual(matricula);

            foreach (var item in response)
            {
                result.Add(new ResponseModel<EmployeeModel>
                {
                    Success = true,
                    Message = "Busca realizada com sucesso.",
                    Data = new EmployeeModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Registration = item.Registration,
                        Role = item.Role,
                        TotalHours = item.TotalHours,
                    }
                });
            }

            return result;
        }
    }
}

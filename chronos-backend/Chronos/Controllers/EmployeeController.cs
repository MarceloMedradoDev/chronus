using Chronos.DTOs;
using Chronos.Interfaces;
using Chronos.Models;
using Chronos.Services.PontoMais;
using Chronos.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Chronos.Controllers
{
    [Route("[controller]")]
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeService _service;
        private readonly IFileService _file;
        private readonly CalcDate _date;
        private readonly IPontoMaisService _pontoMaisService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService service, IFileService file, CalcDate date, IPontoMaisService pontoMaisService)
        {
            _logger = logger;
            _service = service;
            _file = file;
            _date = date;
            _pontoMaisService = pontoMaisService;
        }

        [HttpGet("get-total")]
        public async Task<ActionResult> GetTotalHours()
        {
            await _pontoMaisService.StartPontoMaisService();

            var hours = await _service.GetTotalHours();
            var date = _date.calcularSugestoes();


            return Ok(hours);
        }

        [HttpGet("reports")]
        public async Task<ActionResult> getReports()
        {

            var hours = await _service.reports();
            var date = _date.calcularSugestoes();

            return Ok(hours);
        }

        [Authorize]
        [HttpGet("get-individual")]
        public async Task<ActionResult> GetIndividual()
        {
            var userIdClaim = User.FindFirst("Registration")?.Value;
            
            if (userIdClaim == null)
                return Unauthorized("Token inválido ou sem ID.");

            int userId = int.Parse(userIdClaim);

            var hours = await _service.getIndividual(userId);
            var date = _date.calcularSugestoes();

            return Ok(hours);
        }

        [Authorize]
        [HttpPost("reprocess")]
        public async Task<ActionResult> Reprocess()
        {
            var result = await _pontoMaisService.StartPontoMaisService(true);

            if (result) return Ok(new { mensagem = "Reprocessado com sucesso" });
            else return BadRequest(new { mensagem = "Erro ao reprocessar" });

        }
    }
}

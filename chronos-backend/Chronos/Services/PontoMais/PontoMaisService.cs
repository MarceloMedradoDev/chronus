using Azure;
using Chronos.DTOs;
using Chronos.Interfaces;
using Chronos.Models;
using Chronos.Utils;
using DocumentFormat.OpenXml.Office.CustomUI;
using DotNetEnv;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chronos.Services.PontoMais
{
    public class PontoMaisService : IPontoMaisService
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string baseUrl = "https://atma-api.pontomais.com.br";
        private readonly IEmployeesRepository _employeesRepository;
        private readonly IMemoryCache _memoryCache;

        private const string CACHE = "employees_list";



        private static string? _accessToken;
        private static string? _clientHeader;
        private static string? _uidHeader;

        private List<EmployeeModel> employees = new List<EmployeeModel>();

        public PontoMaisService(IEmployeesRepository employeesRepository, IMemoryCache memoryCache)
        {
            _employeesRepository = employeesRepository;
            _memoryCache = memoryCache;
        }

        public async Task StartPontoMaisService()
        {
            if (!_memoryCache.TryGetValue(CACHE, out List<EmployeeModel> employees))
            {
                await AuthPontoMais();
            }
        }

        public async Task<bool> StartPontoMaisService(bool isReprocess)
        {
            try
            {
                await AuthPontoMais();
                return true;
            }
            catch 
            {
                return false;
            }
        }

        #region Auth Ponto Mais
        public async Task AuthPontoMais()
        {
            var finalEndPoint = @$"{baseUrl}/api/auth/sign_in";
            var json = new
            {
                login = Environment.GetEnvironmentVariable("AUTH_PM_LOGIN"),
                password = Environment.GetEnvironmentVariable("AUTH_PM_PASS")
            };

            string jsonPayload = JsonSerializer.Serialize(json);

            var request = new HttpRequestMessage(HttpMethod.Post, finalEndPoint);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            request.Headers.Accept.ParseAdd("application/json, text/plain, */*");
            request.Headers.Add("api-version", "2");
            request.Headers.Add("Origin", "https://atma2.pontomais.com.br");
            request.Headers.Add("Referer", "https://atma2.pontomais.com.br/");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            if (response.Headers.TryGetValues("access-token", out var tokenValues))
                _accessToken = tokenValues.FirstOrDefault();

            if (response.Headers.TryGetValues("client", out var clientValues))
                _clientHeader = clientValues.FirstOrDefault();

            if (response.Headers.TryGetValues("uid", out var uidValues))
                _uidHeader = uidValues.FirstOrDefault();

            string result = await response.Content.ReadAsStringAsync();

            await GetEmployees();
        }
        #endregion

        #region Buscar os funcionarios listados no PontoMais
        public async Task GetEmployees()
        {
            if (string.IsNullOrEmpty(_accessToken)) await AuthPontoMais();

            var finalEndPoint = $"{baseUrl}/api/v3/employees" +
                                $"?attributes=id,name,business_unit,department,team,picture" +
                                $"&count=true" +
                                $"&has_initial_date=true" +
                                $"&only_actives=true" +
                                $"&sort_direction=asc" +
                                $"&sort_property=first_name,last_name" +
                                $"&page={1}" +
                                $"&per_page={20}";

            var request = new HttpRequestMessage(HttpMethod.Get, finalEndPoint);

            request.Headers.Accept.ParseAdd("application/json, text/plain, */*");
            request.Headers.Add("Accept-Language", "pt-PT,pt;q=0.9,pt-BR;q=0.8,en;q=0.7,en-US;q=0.6,en-GB;q=0.5");

            request.Headers.TryAddWithoutValidation("Access-Token", _accessToken);
            request.Headers.TryAddWithoutValidation("Client", _clientHeader);
            request.Headers.TryAddWithoutValidation("Uid", _uidHeader);
            request.Headers.TryAddWithoutValidation("Token-Type", "Bearer");

            request.Headers.Add("Api-Version", "2");
            request.Headers.Add("Origin", "https://atma2.pontomais.com.br");
            request.Headers.Add("Referer", "https://atma2.pontomais.com.br/");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0");
            try
            {
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(result))
                {
                    JsonElement root = doc.RootElement;

                    // LINHA DO BREAKPOINT AQUI:
                    JsonElement employeesArray = root.ValueKind == JsonValueKind.Array ? root : root.GetProperty("employees");

                    foreach (JsonElement element in employeesArray.EnumerateArray())
                    {
                        int id = element.GetProperty("id").GetInt32();
                        string name = element.GetProperty("name").GetString();

                        employees.Add(new EmployeeModel()
                        {
                            Id = id,
                            Name = name,
                        });
                    }
                }

                await WorkDays();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erro no GET: {e.Message}");
            }
        }
        #endregion

        public async Task WorkDays()
        {
            if (string.IsNullOrEmpty(_accessToken)) await AuthPontoMais();

            DateTime hoje = DateTime.Now;

            DateTime primeiroDiaDoMes = new DateTime(hoje.Year, hoje.Month, 1);

            DateTime diaMenosUm = hoje.AddDays(-1);

            var culture = new CultureInfo("en-US");
            string startDate = primeiroDiaDoMes.ToString("ddd MMM dd yyyy", culture);
            string endDate = diaMenosUm.ToString("ddd MMM dd yyyy", culture);

            
            foreach (var item in employees)
            {
                var funcionarioId = item.Id;

                //if (item.Name != "MARCELO ERIC MARINHO COSTA BRITTO") continue;

                var finalEndPoint = $@"{baseUrl}/api/time_card_control/{funcionarioId}/work_days" +
                                    $"?end_date={Uri.EscapeDataString(endDate)}" +
                                    $"&sort_direction=asc" +
                                    $"&sort_property=date" +
                                    $"&start_date={Uri.EscapeDataString(startDate)}" +
                                    $"&with_employee=false";

                var request = new HttpRequestMessage(HttpMethod.Get, finalEndPoint);

                request.Headers.TryAddWithoutValidation("Access-Token", _accessToken);
                request.Headers.TryAddWithoutValidation("Client", _clientHeader);
                request.Headers.TryAddWithoutValidation("Uid", _uidHeader);
                request.Headers.TryAddWithoutValidation("Token-Type", "Bearer");
                request.Headers.TryAddWithoutValidation("Api-Version", "2");

                request.Headers.Accept.ParseAdd("application/json, text/plain, */*");
                request.Headers.Add("Origin", "https://atma2.pontomais.com.br");
                request.Headers.Add("Referer", "https://atma2.pontomais.com.br/");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36 Edg/151.0.0.0");

                TimeSpan journey = TimeSpan.Zero;

                try
                {
                    HttpResponseMessage response = await client.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                    }

                    response.EnsureSuccessStatusCode();

                    string result = await response.Content.ReadAsStringAsync();


                    using (JsonDocument doc = JsonDocument.Parse(result))
                    {
                        JsonElement root = doc.RootElement;

                        JsonElement dataArray = root.ValueKind == JsonValueKind.Array ? root : root.GetProperty("work_days");


                        foreach (JsonElement dayItem in dataArray.EnumerateArray())
                        {
                            string? date = dayItem.GetProperty("date").GetString();

                            JsonElement timeCardsArray = dayItem.GetProperty("time_cards");


                            if (timeCardsArray.ValueKind == JsonValueKind.Array && timeCardsArray.GetArrayLength() > 0)
                            {
                                int count = Math.Min(4, timeCardsArray.GetArrayLength());

                                var day = new List<string>();

                                for (int i = 0; i < count; i++)
                                {
                                    JsonElement card = timeCardsArray[i];
                                    string? timeValue = card.GetProperty("time").GetString();

                                    day.Add(timeValue);
                                }

                                if (day.Count != 4)
                                {
                                    item.IsCompleteDay = false;
                                    item.Day = DateTime.Parse(date);
                                }

                                if (day.Count < 4) day.Add("00:00");

                                journey += CalculatorJourneyDay.CalculateJourney(day);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Erro: {e.Message}");
                    throw e;
                }
                int horasInteiras = (int)Math.Floor(journey.TotalHours);

                int minutosRestantes = journey.Minutes;

                item.TotalHours = $"{horasInteiras}:{minutosRestantes:D2}";

            }
            await _employeesRepository.InsertEmployeesAsync(employees);
        }
    }
}
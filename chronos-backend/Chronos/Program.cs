using Chronos.Context;
using Chronos.Interfaces;
using Chronos.Repositories;
using Chronos.Services;
using Chronos.Services.PontoMais;
using Chronos.Services.Token;
using Chronos.Utils;
using DocumentFormat.OpenXml.Bibliography;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1) Carregar variáveis de ambiente (.env)
// ============================================================
Env.Load();

var secretKey = builder.Configuration["jwt:Key"];

var issuer = builder.Configuration["jwt:issuer"];

var audience = builder.Configuration["jwt:audience"];

var connectionString = builder.Configuration["ConnectionStrings:SecondConnection"];



// ============================================================
// 2) Configurar CORS
// ============================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// ============================================================
// 3) Configurar banco de dados
// ============================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));


// ============================================================
// 4) JWT Authentication
// ============================================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // sem tolerância extra no vencimento
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("ADM"));
});


// ============================================================
// 5) Injeção de dependências
// ============================================================
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEmployeesRepository, EmployeesRepository>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICalcSugestions, CalcSugestoes>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<CalcDate>();
builder.Services.AddScoped<IPontoMaisService, PontoMaisService>();


// ============================================================
// 6) Configurações adicionais
// ============================================================
builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Chronos API",
        Version = "v1"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Digite: Bearer {seu_token}",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("LoginLimiter", HttpContext =>
    {
        var clientIp = HttpContext.Connection.RemoteIpAddress.ToString() ?? "unknow";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIp,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(15),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }
            );
    });

    options.AddPolicy("GlobalLimiter", httpContext =>
    {
        var clientIp = httpContext.Connection?.RemoteIpAddress?.ToString() ?? "unknown";

        
        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: clientIp,
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 10,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6
            }
        );
    });
}
    );

builder.Services.AddHostedService<RotinaBuscaPontoMais>();

var app = builder.Build();


// ============================================================
// 7) Aplicar migrações automaticamente ao iniciar
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


// ============================================================
// 8) Pipeline HTTP (ordem importa!)
// ============================================================
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

// ATIVA JWT AQUI
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

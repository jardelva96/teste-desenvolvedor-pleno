using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductManagementAPI.Data;
using ProductManagementAPI.Configurations;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging(); // Apenas para desenvolvimento
});

// Configuração para lidar com ciclos de referência no JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // Resolve loops de referência
        options.JsonSerializerOptions.WriteIndented = true; // Melhora a legibilidade do JSON
    });

// Configuração de CORS para permitir requisições de qualquer origem
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuração de autenticação JWT
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>();

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret))
    };
});

// Configuração do Swagger para documentação da API com suporte a autenticação JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Management API",
        Version = "v1",
        Description = "API para gestão de produtos, categorias e fornecedores.",
        Contact = new OpenApiContact
        {
            Name = "Seu Nome",
            Email = "seuemail@exemplo.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Adicionando os controladores para os endpoints
builder.Services.AddControllers();  // Certifique-se de que os controladores estão registrados

var app = builder.Build();

// Configuração para ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Management API v1");
    });
}

// Middleware global de tratamento de erros
app.UseMiddleware<GlobalExceptionHandler>();

// Middleware para verificar a conexão com o banco de dados
app.Use(async (context, next) =>
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.CloseConnectionAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao conectar ao banco de dados: {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Erro ao conectar ao banco de dados.");
        return;
    }

    await next();
});

// Configurações globais
app.UseCors("AllowAll");

// Middleware de autenticação, mas somente para controladores com [Authorize]
app.UseAuthentication(); // Middleware de autenticação
app.UseAuthorization();   // Middleware de autorização

// Mapeamento dos controladores
app.MapControllers();
app.Run();

// ----------------------- Classes Adicionais -----------------------

// Classe para tratamento global de erros
public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Processa a solicitação normalmente
        }
        catch (Exception ex)
        {
            // Resposta personalizada para erros
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync($"Erro interno no servidor: {ex.Message}");
        }
    }
}

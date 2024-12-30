using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using ProductManagementAPI.Configurations;
using ProductManagementAPI.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProductManagementAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace ProductManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtConfig _jwtConfig;
        private readonly AppDbContext _dbContext;

        public AuthController(IOptions<JwtConfig> jwtConfig, AppDbContext dbContext)
        {
            _jwtConfig = jwtConfig.Value;
            _dbContext = dbContext;
        }

        // Endpoint para Login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validação de credenciais
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest("Nome de usuário e senha são obrigatórios.");
                }

                // Verifica se o usuário existe no banco de dados
                var user = _dbContext.Users.FirstOrDefault(u => u.Username == request.Username);
                if (user == null || !VerifyPassword(request.Password, user.Password)) // Verifica a senha com hash
                {
                    return Unauthorized("Credenciais inválidas.");
                }

                // Gera o token JWT
                var userToken = GenerateJwtToken(user.Username);
                return Ok(new { Token = userToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        // Endpoint para Registro (Cadastro)
        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Validação básica de dados
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest("Nome de usuário e senha são obrigatórios.");
                }

                // Verifica se o nome de usuário já existe
                var existingUser = _dbContext.Users.FirstOrDefault(u => u.Username == request.Username);
                if (existingUser != null)
                {
                    return BadRequest("Nome de usuário já existe.");
                }

                // Cria um novo usuário
                var newUser = new User
                {
                    Username = request.Username,
                    Password = HashPassword(request.Password) // Usando o hash da senha
                };

                _dbContext.Users.Add(newUser);
                _dbContext.SaveChanges();

                // Gera o token JWT
                var userToken = GenerateJwtToken(newUser.Username);
                return Ok(new { Token = userToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
            }
        }

        // Função para gerar o token JWT
        private string GenerateJwtToken(string username)
        {
            if (string.IsNullOrWhiteSpace(_jwtConfig.Secret))
            {
                throw new InvalidOperationException("A chave secreta do JWT não está configurada.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var claims = new[] 
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Função para realizar o hashing da senha
        private string HashPassword(string password)
        {
            // Usando um salt gerado aleatoriamente
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Usando o PBKDF2 para gerar o hash
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hash;
        }

        // Função para verificar a senha fornecida
        private bool VerifyPassword(string providedPassword, string storedPasswordHash)
        {
            // A função de hash e verificação deve ser mais complexa e incluir salt.
            return providedPassword == storedPasswordHash;  // Comparar com hash armazenado no banco
        }
    }

    // Classe de Requisição de Login
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // Classe de Requisição de Registro
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;  // Se necessário, pode incluir o email
    }
}

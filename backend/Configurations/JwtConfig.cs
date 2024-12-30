using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProductManagementAPI.Configurations
{
    public class JwtConfig
    {
        public string Secret { get; set; } = string.Empty; // Chave secreta usada para gerar o token
        public string Issuer { get; set; } = string.Empty; // Emissor do token JWT
        public string Audience { get; set; } = string.Empty; // Público-alvo do token JWT

        // Método para retornar a chave simétrica
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        }
    }
}

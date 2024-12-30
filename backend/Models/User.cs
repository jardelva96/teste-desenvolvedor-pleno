namespace ProductManagementAPI.Models
{
    public class User
    {
        public int Id { get; set; } // Chave primária
        public string Username { get; set; } = string.Empty; // Nome de usuário
        public string Password { get; set; } = string.Empty; // Senha
    }
}

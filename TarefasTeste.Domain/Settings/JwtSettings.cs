namespace TarefasTeste.Domain.Settings
{
    /// <summary>
    /// Configurações para JWT Bearer Token
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Chave secreta para validar a assinatura do token (Base64)
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Emissor esperado do token (deve ser idêntico ao da API de Usuários)
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Público esperado do token (deve ser idêntico ao da API de Usuários)
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Tempo de expiração do token em minutos
        /// </summary>
        public int ExpirationMinutes { get; set; } = 30;
    }
}

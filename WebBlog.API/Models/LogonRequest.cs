namespace WebBlog.Models
{
    /// <summary>
    /// Класс представляющий данные в запросе идентифиувции
    /// </summary>
    public class LogonRequest
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string Username { get; set; } = null!;
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; } = null!;
    }
}
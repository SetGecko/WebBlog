namespace WebBlog.Models
{
    /// <summary>
    /// Уласс представления информации об ошибке
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// ID запроса
        /// </summary>
        public string? RequestId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
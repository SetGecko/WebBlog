namespace WebBlog.Models
{
    /// <summary>
    /// ����� ������������� ���������� �� ������
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// ID �������
        /// </summary>
        public string? RequestId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
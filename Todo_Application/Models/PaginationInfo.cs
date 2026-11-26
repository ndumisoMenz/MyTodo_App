namespace Todo_Application.Models
{
    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchString {  get; set; }

        public string StatusFilter { get; set; }
        public string ActionName { get; set; }

        public string ControllerName { get; set; }
    }
}

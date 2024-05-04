namespace WebApp.Models
{
    public abstract class ResultBase
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
    }
}

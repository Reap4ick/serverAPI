namespace ApiStore.Models.Post
{
    public class PostItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = String.Empty;
        public string Body { get; set; } = String.Empty;
        public DateTime DateCreated { get; set; }
    }
}

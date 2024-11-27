namespace ApiStore.Models.Post
{
    public class PostEditDto
    {
        public int Id { get; set; } // Ідентифікатор поста
        public string Title { get; set; } = String.Empty;
        public string Body { get; set; } = String.Empty;
        /*public DateTime DateCreated { get; set; } // Дата створення*/
    }

}

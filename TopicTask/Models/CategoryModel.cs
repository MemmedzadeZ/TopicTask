namespace TopicTask.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CategoryModel> SubCategories { get; set; } = new List<CategoryModel>();

    }
}

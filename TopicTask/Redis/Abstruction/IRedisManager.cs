using TopicTask.Models;

namespace TopicTask.Redis.Abstruction
{
    public interface IRedisManager
    {
        void SaveList(string name, string parentName);
        List<CategoryModel> GetCategoryModelList();
    }
}

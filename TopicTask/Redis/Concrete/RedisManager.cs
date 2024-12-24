using StackExchange.Redis;
using System.Text.Json;
using TopicTask.Models;
using TopicTask.Redis.Abstruction;

namespace TopicTask.Redis.Concrete
{
    public  class RedisManager : IRedisManager
    {

        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly string nameDivision = "divisions";


        public RedisManager()
        {
            _redis = ConnectionMultiplexer.Connect(
               new ConfigurationOptions
               {
                   EndPoints = { { "redis-16071.c114.us-east-1-4.ec2.redns.redis-cloud.com", 16071 } },
                   User = "default",
                   Password = "FKYHW66StW-A21THXjGj_sIzy7Kn14ue",

               }
           );

            _database = _redis.GetDatabase();
        }
        public List<CategoryModel> GetCategoryModelList()
        {
            throw new NotImplementedException();
        }

        public void SaveList(string name, string parentName)
        {
            // Redis-dən mövcud siyahını əldə edin
            var json = _database.StringGet(nameDivision);

            // Mövcud siyahını deserialize edin və ya yeni bir siyahı yaradın
            var list = !string.IsNullOrEmpty(json)
                ? JsonSerializer.Deserialize<List<CategoryModel>>(json)
                : new List<CategoryModel>();

            CategoryModel FindAndAddDivision(List<CategoryModel> divisions, string parent)
            {
                if (string.IsNullOrEmpty(parent))
                    return null;

                var arr = parent.Split('.', 2);

                var division = divisions.Find(d => d.Name == arr[0]);
                if (division == null)
                {
                    // Valideyn tapılmadısa, yeni bir qeyd yaradılır
                    division = new CategoryModel { Name = arr[0], SubCategories = new List<CategoryModel>() };
                    divisions.Add(division);
                }

                // Alt bölmə yoxdursa, tapılana qədər davam edir
                return arr.Length == 1
                    ? division
                    : FindAndAddDivision(division.SubCategories, arr[1]);
            }

            // Əsas valideyni tapın və ya yaradın
            var parentDivision = FindAndAddDivision(list, parentName);

            if (parentDivision == null)
            {
                // Valideyn mövcud deyil, əsas siyahıya əlavə edin
                list.Add(new CategoryModel { Name = name, SubCategories = new List<CategoryModel>() });
            }
            else
            {
                // Valideyn mövcuddursa, alt bölməni əlavə edin
                parentDivision.SubCategories.Add(new CategoryModel { Name = name, SubCategories = new List<CategoryModel>() });
            }

            // Yenilənmiş siyahını serialize edin və Redis-ə geri yazın
            var jsonData = JsonSerializer.Serialize(list);
            _database.StringSet(nameDivision, jsonData);
        }
    }
}

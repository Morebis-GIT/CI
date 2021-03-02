using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ImagineCommunications.GamePlan.Infrastructure.Json.Core.Converters;
using Newtonsoft.Json;

namespace xgcore.auditevents.Repository.File
{
    /// <summary>
    /// Base class for file (JSON) based repository.
    /// </summary>
    public abstract class FileRepositoryBase
    {
        protected readonly string _folder;
        protected readonly string _type;

        protected FileRepositoryBase(string folder, string type)
        {
            _folder = folder;
            _type = type;
        }

        /// <summary>
        /// Deserializes content body string to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentString"></param>
        /// <returns></returns>
        protected T DeserializeContentBody<T>(string contentString) =>
            JsonConvert.DeserializeObject<T>(contentString);

        protected List<string> GetFilesByType(string folder, string type)
        {
            if (!Directory.Exists(folder))
            {
                return new List<string>();
            }

            var files = new List<string>();
            string pattern = $"*.{type}.json";

            foreach (string file in Directory.GetFiles(folder, pattern))
            {
                files.Add(file);
            }

            return files;
        }

        protected void InsertItems<T>(string folder, string type, List<T> items, List<string> ids)
        {
            for (int index = 0; index < ids.Count; index++)
            {
                UpdateOrInsertItem(folder, type, items[index], ids[index]);
            }
        }

        protected void UpdateOrInsertItem<T>(string folder, string type, T item, string id)
        {
            string file = Path.Combine(folder, $"{id}.{type}.json");

            if (Directory.Exists(folder))
            {
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }
            else
            {
                _ = Directory.CreateDirectory(folder);
            }

            SaveItem(item, file);
        }

        protected void DeleteItem<T>(string folder, string type, string id)
        {
            if (!Directory.Exists(folder))
            {
                return;
            }

            string file = Path.Combine(folder, $"{id}.{type}.json");
            if (System.IO.File.Exists(file))
            {
                DeleteItem(file);
            }
        }

        protected void DeleteAllItems<T>(string folder, string type, Expression<Func<T, bool>> filter)
        {
            foreach (string file in GetFilesByType(folder, type))
            {
                T item = LoadItem<T>(file);

                // Check if item meets criteria
                var itemsToFilter = new List<T>() { item };
                IQueryable<T> queryableItemsToFilter = itemsToFilter.AsQueryable<T>();
                var results = queryableItemsToFilter.Where(filter);
                if (results.Any())
                {
                    DeleteItem(file);
                }
            }
        }

        protected void DeleteAllItems<T>(string folder, string type)
        {
            foreach (string file in GetFilesByType(folder, type))
            {
                DeleteItem(file);
            }
        }

        protected T GetItemByID<T>(string folder, string type, string id)
        {
            if (!Directory.Exists(folder))
            {
                return default;
            }

            string file = Path.Combine(folder, $"{id}.{type}.json");
            if (System.IO.File.Exists(file))
            {
                return LoadItem<T>(file);
            }

            return default;
        }

        protected int CountAll<T>(string folder, string type) =>
            GetFilesByType(folder, type).Count;

        protected List<T> GetAllByType<T>(string folder, string type)
        {
            var items = new List<T>();

            foreach (string file in GetFilesByType(folder, type))
            {
                T item = LoadItem<T>(file);
                items.Add(item);
            }

            return items;
        }

        protected List<T> GetAllByType<T>(string folder, string type, Expression<Func<T, bool>> filter)
        {
            var items = new List<T>();

            foreach (string file in GetFilesByType(folder, type))
            {
                T item = LoadItem<T>(file);

                // Check if item meets criteria
                var itemsToFilter = new List<T>() { item };
                IQueryable<T> queryableItemsToFilter = itemsToFilter.AsQueryable<T>();
                var results = queryableItemsToFilter.Where(filter);
                items.AddRange(results);
            }

            return items;
        }

        private T LoadItem<T>(string file) =>
            JsonConvert.DeserializeObject<T>(
                System.IO.File.ReadAllText(file),
                DefaultJsonSerializerSettings);

        private static JsonSerializerSettings DefaultJsonSerializerSettings
        {
            get
            {
                var jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                jsonSerializerSettings.Converters.Add(new NodaTimeDurationTicksJsonConverter());

                return jsonSerializerSettings;
            }
        }

        private void SaveItem<T>(T item, string file)
        {
            string contentString = JsonConvert.SerializeObject(
                item,
                Formatting.Indented,
                DefaultJsonSerializerSettings);

            System.IO.File.WriteAllBytes(file, Encoding.UTF8.GetBytes(contentString));
        }

        private void DeleteItem(string file)
        {
            System.IO.File.Delete(file);
        }
    }
}

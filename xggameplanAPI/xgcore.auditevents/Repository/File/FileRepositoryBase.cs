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

        private static readonly JsonSerializerSettings _defaultJsonSerializerSettings;

        static FileRepositoryBase()
        {
            _defaultJsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            _defaultJsonSerializerSettings.Converters.Add(
                new NodaTimeDurationTicksJsonConverter()
                );
        }

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

        protected string[] GetFilesByType(string folder, string type)
        {
            return Directory.Exists(folder)
                ? Directory.GetFiles(folder, $"*.{type}.json")
                : Array.Empty<string>();
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

        protected void DeleteItem(string folder, string type, string id)
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

        protected void DeleteAllItems(string folder, string type)
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

        protected int CountAll(string folder, string type) =>
            GetFilesByType(folder, type).Length;

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

        private static T LoadItem<T>(string file) =>
            JsonConvert.DeserializeObject<T>(
                System.IO.File.ReadAllText(file),
                _defaultJsonSerializerSettings);

        private static void SaveItem<T>(T item, string file)
        {
            string contentString = JsonConvert.SerializeObject(
                item,
                Formatting.Indented,
                _defaultJsonSerializerSettings);

            System.IO.File.WriteAllBytes(file, Encoding.UTF8.GetBytes(contentString));
        }

        private static void DeleteItem(string file)
        {
            System.IO.File.Delete(file);
        }
    }
}

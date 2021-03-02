using System;
using System.Collections.Generic;

namespace xggameplan.Extensions
{
    /// <summary>
    /// Extentions to Collection and ICollections object.
    /// </summary>
    /// <remarks>
    /// There are extensions in this project and the xggameplan.core project.
    /// They need to be consolidated into a single .NET Standard 2.x project.
    /// </remarks>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Add an item to the collection if it is not already in the collection.
        /// </summary>
        /// <param name="collection">
        /// The collection to add the item to. An exception will be thrown if
        /// the collection object is null.
        /// </param>
        /// <param name="item">The item to add to the collection.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the collection is null.
        /// </exception>
        public static void AddDistinct<T>(
            this ICollection<T> collection,
            T item)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (collection.Contains(item))
            {
                return;
            }

            collection.Add(item);
        }

        /// <summary>
        /// Add an item to the collection if it is not already in the collection.
        /// </summary>
        /// <param name="collection">
        /// The collection to add the item to. An exception will be thrown if
        /// the collection object is null.
        /// </param>
        /// <param name="item">The item to add to the collection.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the collection is null.
        /// </exception>
        /// <remarks>
        /// A specific version for <see cref="HashSet{T}"/> as its
        /// <see cref="HashSet{T}.Add(T)"/> method already deduplicates.
        /// </remarks>
        public static void AddDistinct<T>(
            this HashSet<T> collection,
            T item)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _ = collection.Add(item);
        }

        /// <summary>
        /// <para>
        /// Adds the items in the source collection to the destination
        /// collection. Does not add an item to the destination if it already exists.
        /// </para>
        /// <para>
        /// This is a special case because HashSet implements both ICollection
        /// and IReadOnlyCollection and the compiler doesn't know which to use.
        /// </para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyDistinctTo<T>(
            this HashSet<T> source,
            ICollection<T> destination)
        {
            foreach (T item in source)
            {
                destination.AddDistinct<T>(item);
            }
        }

        /// <summary>
        /// Adds the items in the source collection to the destination
        /// collection. Does not add an item to the destination if it already exists.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyDistinctTo<T>(
            this ICollection<T> source,
            ICollection<T> destination)
        {
            foreach (T item in source)
            {
                destination.AddDistinct<T>(item);
            }
        }

        /// <summary>
        /// Adds the items in the source collection to the destination
        /// collection. Does not add an item to the destination if it already exists.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyDistinctTo<T>(
            this IReadOnlyCollection<T> source,
            ICollection<T> destination)
        {
            foreach (T item in source)
            {
                destination.AddDistinct<T>(item);
            }
        }
    }
}

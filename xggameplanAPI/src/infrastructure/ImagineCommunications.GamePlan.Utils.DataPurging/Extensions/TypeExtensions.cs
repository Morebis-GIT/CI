using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the array of base types hierarchy for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">type</exception>
        public static Type[] GetBaseTypes(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var typeCollection = new List<Type>();
            var baseType = type.BaseType;
            while (!(baseType is null))
            {
                typeCollection.Add(baseType);
                baseType = baseType.BaseType;
            }

            return typeCollection.ToArray();
        }
    }
}

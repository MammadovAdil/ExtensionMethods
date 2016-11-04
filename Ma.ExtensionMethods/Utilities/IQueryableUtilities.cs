using System;
using System.Linq;
using Ma.ExtensionMethods.Reflection;
using System.Linq.Expressions;
using System.Reflection;

namespace Ma.ExtensionMethods.Utilities
{
    public static class IQueryableUtilities
    {
        /// <summary>
        /// Order by source according to provided property name and order.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When source or propertyName is null.
        /// </exception>
        /// <typeparam name="TEntity">Type of entity.</typeparam>
        /// <param name="source">Source to order.</param>
        /// <param name="propertyName">Name of property to order according to.</param>
        /// <param name="isDescending">Order by descending if set to true / Otherwise ascenting</param>
        /// <returns>Ordered query.</returns>
        public static IQueryable<TEntity> OrderBy<TEntity>(
            this IQueryable<TEntity> source,
            string propertyName,
            bool isDescending)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            string orderCommand = isDescending ? "OrderByDescending" : "OrderBy";

            Type entityType = typeof(TEntity);
            PropertyInfo orderProperty = entityType.GetPropertyInfo(propertyName);

            LambdaExpression memberAccessExpression = entityType
                .GenerateMemberAccessExpression(propertyName);

            MethodCallExpression resultExpression = Expression.Call(
                typeof(Queryable),
                orderCommand,
                new Type[] { entityType, orderProperty.PropertyType },
                source.Expression,
                Expression.Quote(memberAccessExpression));

            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}

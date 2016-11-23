using System;
using System.Linq;
using Ma.ExtensionMethods.Reflection;
using System.Linq.Expressions;
using System.Reflection;
using Ma.ExtensionMethods.Models;
using System.Collections.Generic;

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

        /// <summary>
        /// Select properties according to property names from source.
        /// </summary>
        /// <remarks>
        /// This method is copied from StackOverflow answer (http://stackoverflow.com/a/723018/1380428).
        /// Alterations and additions made by Adil Mammadov.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// When source or propertyNames is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When there is no item in propertyNames.
        /// </exception>
        /// <param name="source">Source to select from.</param>
        /// <param name="propertyNames">Name of properties to get data according to.</param>
        /// <returns>The result as IQueryable.</returns>
        public static IQueryable<object> SelectDynamic(
            this IQueryable source,
            IEnumerable<string> propertyNames)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (propertyNames == null)
                throw new ArgumentNullException(nameof(propertyNames));

            if (propertyNames.Count() == 0)
                throw new ArgumentOutOfRangeException("propertyNames", "There is no propertyName in propertyNames");

            Dictionary<string, PropertyInfo> sourceProperties = propertyNames
                .ToDictionary(name => name, name => source.ElementType.GetProperty(name));

            // Create anonymous type at runtime
            Type dynamicType = LinqRuntimeTypeBuilder.GetDynamicType(sourceProperties.Values);

            ParameterExpression sourceItem = Expression.Parameter(source.ElementType, "m");
            IEnumerable<MemberBinding> bindings = dynamicType.GetProperties()
                .Select(p => Expression.Bind(p, Expression.Property(sourceItem, sourceProperties[p.Name])))
                .OfType<MemberBinding>();

            Expression selector = Expression.Lambda(Expression.MemberInit(
                Expression.New(dynamicType.GetConstructor(Type.EmptyTypes)), bindings), sourceItem);

            IQueryable queryResult = source.Provider.CreateQuery(
                Expression.Call(typeof(Queryable),
                                "Select",
                                new Type[] { source.ElementType, dynamicType },
                                source.Expression,
                                selector));

            // Cast to IQueryable<object> to be able to use Linq
            return (IQueryable<object>)queryResult;
        }
    }
}

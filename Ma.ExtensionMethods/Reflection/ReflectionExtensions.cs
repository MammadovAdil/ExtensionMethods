using Ma.ExtensionMethods.Models.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Ma.ExtensionMethods.Reflection
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Get list of properties from lambda expression.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When property lambda is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// When property lambda does not refer to a property.
        /// </exception>
        /// <typeparam name="TSource">Type of mapped entity.</typeparam>
        /// <typeparam name="TProperty">Typeof requested property.</typeparam>
        /// <param name="propertyLambda">Lambda expression to request property.</param>
        /// <returns>List of property infos.</returns>
        public static List<PropertyInfo> GetPropertyInfoList<TSource, TProperty>(
            this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (propertyLambda == null)
                throw new ArgumentNullException("propertyLambda");

            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
            PropertyInfo propertyInfo;

            MemberExpression memberExpression = propertyLambda.Body as MemberExpression;

            if (memberExpression != null)
            {
                propertyInfo = memberExpression.Member.GetPropertyInfo();
                propertyInfoList.Add(propertyInfo);
            }
            else
            {
                NewExpression newExpression = propertyLambda.Body as NewExpression;
                if (newExpression != null)
                {
                    foreach (MemberInfo member in newExpression.Members)
                    {
                        propertyInfo = member.GetPropertyInfo();
                        propertyInfoList.Add(propertyInfo);
                    }
                }
                else
                {
                    throw new ArgumentException(string.Format(
                        "Expression '{0}' refers to a methot, not a property.",
                        propertyLambda.ToString()));

                }
            }

            return propertyInfoList;
        }

        /// <summary>
        /// Get property from lambda expression.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When property lambda is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// When property lambda does not refer to a property.
        /// </exception>
        /// <typeparam name="TSource">Type of mapped entity.</typeparam>
        /// <typeparam name="TProperty">Typeof requested property.</typeparam>
        /// <param name="propertyLambda">Lambda expression to request property.</param>
        /// <returns>Property info.</returns>
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (propertyLambda == null)
                throw new ArgumentNullException("propertyLambda");

            PropertyInfo propertyInfo;
            MemberExpression memberExpression = propertyLambda.Body as MemberExpression;

            if (memberExpression != null)
                propertyInfo = memberExpression.Member.GetPropertyInfo();
            else
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method or to a new expression, not a property.",
                    propertyLambda.ToString()));

            return propertyInfo;
        }

        /// <summary>
        /// Get property info from member.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// When member does not refer to a property.
        /// </exception>
        /// <typeparam name="TSource">Type of container source.</typeparam>
        /// <param name="member">Member to get property from.</param>
        /// <returns>Property info.</returns>
        public static PropertyInfo GetPropertyInfo(this MemberInfo member)
        {
            PropertyInfo propertyInfo = member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException(string.Format(
                    "Member '{0}' refers to a field, not a property.",
                    member.Name));

            return propertyInfo;
        }

        /// <summary>
        /// Check if type is built in type.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When type to check is null.
        /// </exception>
        /// <param name="typeToCheck">Type to check.</param>
        /// <returns>True if built-in type/False otherwise.</returns>
        public static bool IsBuiltinType(this Type typeToCheck)
        {
            if (typeToCheck == null)
                throw new ArgumentNullException("typeToCheck");

            return typeToCheck.Module
                .ScopeName.Equals("CommonLanguageRuntimeLibrary");
        }

        /// <summary>
        /// Check if type is dynamic proxy type
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When type to check is null
        /// </exception>
        /// <param name="typeToCheck">Type to check</param>
        /// <returns>If type is dynamic proxy type</returns>
        public static bool IsDynamicProxyType(this Type typeToCheck)
        {
            if (typeToCheck == null)
                throw new ArgumentNullException("typeToCheck");

            return typeToCheck.FullName.StartsWith("System.Data.Entity.DynamicProxies");
        }

        /// <summary>
        /// Check if type is collection type.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When type to check is null.
        /// </exception>
        /// <param name="typeToCheck">Type to check.</param>
        /// <returns>True if collection type/False otherwise.</returns>
        public static bool IsCollectionType(this Type typeToCheck)
        {
            if (typeToCheck == null)
                throw new ArgumentNullException("typeToCheck");

            return
                typeof(IEnumerable).IsAssignableFrom(typeToCheck)
                && !typeToCheck.Equals(typeof(string));
        }

        /// <summary>
        /// Check if type is nullable type.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When type to check is null.
        /// </exception>
        /// <param name="typeToCheck">Type to check if it is nullable type.</param>
        /// <returns>True if type is nullable/False otherwise.</returns>
        public static bool IsNullable(this Type typeToCheck)
        {
            if (typeToCheck == null)
                throw new ArgumentNullException("typeToCheck");

            return Nullable.GetUnderlyingType(typeToCheck) != null;
        }

        /// <summary>
        /// Check if type is nullable enum type.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When type to check is null.
        /// </exception>
        /// <param name="typeToCheck">Type to check if it is nullable type.</param>
        /// <returns>True if type is nullable enum/False otherwise.</returns>
        public static bool IsNullableEnum(this Type typeToCheck)
        {
            if (typeToCheck == null)
                throw new ArgumentNullException("typeToCheck");

            Type underlyingType = Nullable.GetUnderlyingType(typeToCheck);
            return (underlyingType != null) && underlyingType.IsEnum;
        }

        /// <summary>
        /// Get property info from source by name.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When source or propery name is null.
        /// </exception>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <param name="source">Source to get property from.</param>
        /// <param name="propertyName">Name to get property according to.</param>
        /// <returns>Property info.</returns>
        public static PropertyInfo GetProperty<TSource>(
            this TSource source,
            string propertyName)
            where TSource : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            return source.GetType().GetProperty(propertyName);
        }

        /// <summary>
        /// Get property value from source according to property name.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When source or propertyName is null.
        /// </exception>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <typeparam name="TProperty">Type of property.</typeparam>
        /// <param name="source">Source to get value from.</param>
        /// <param name="propertyName">Name of property to get value according to.</param>
        /// <returns>Value of property.</returns>
        public static object GetPropertyValue<TSource>(
            this TSource source, string propertyName)
            where TSource : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            return source.GetType()
                .GetProperty(propertyName)
                .GetValue(source, null);
        }

        /// <summary>
        /// Get property value from source according to property.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When source or property is null.
        /// </exception>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <typeparam name="TProperty">Type of property.</typeparam>
        /// <param name="source">Source to get value from.</param>
        /// <param name="property">Property to get value according to.</param>
        /// <returns>Value of property.</returns>
        public static object GetPropertyValue<TSource>(
            this TSource source, PropertyInfo property)
            where TSource : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (property == null)
                throw new ArgumentNullException("property");

            //return source.GetPropertyValue(property.Name);
            return property.GetValue(source, null);
        }

        /// <summary>
        /// Set value of property with specified name of target to specified value.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When target or property name is null.
        /// </exception>
        /// <typeparam name="TTarget">Type of target.</typeparam>
        /// <param name="target">Target to set value.</param>
        /// <param name="propertyName">Name of proeprty to set value.</param>
        /// <param name="propertyValue">Value to set to property.</param>
        public static void SetPropertyValue<TTarget>(
            this TTarget target,
            string propertyName,
            object propertyValue)
            where TTarget : class
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            target.GetProperty(propertyName).SetValue(target, propertyValue);
        }

        /// <summary>
        /// Check if property of source with porpertyName has default value.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When source of propertyName is null.
        /// </exception>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <param name="source">Source to check has property with default value.</param>
        /// <param name="propertyName">Name of property to check.</param>
        /// <returns>If property of source with porpertyName has default value.</returns>
        public static bool HasDefaultValue<TSource>(
            this TSource source,
            string propertyName)
            where TSource : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            return source.HasDefaultValues(new List<string> { propertyName });
        }

        /// <summary>
        /// Check if any of properties of source in propertyNames has default value.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If source or proeprtyNames is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If there is no element in propertyNames.
        /// </exception>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <param name="source">Source to check</param>
        /// <param name="propertyNames">Names of properties.</param>
        /// <returns>If any of properties of source in propertyNames has default value.</returns>
        public static bool HasDefaultValues<TSource>(
            this TSource source,
            IEnumerable<string> propertyNames)
            where TSource : class
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (propertyNames == null)
                throw new ArgumentNullException(nameof(propertyNames));

            if (propertyNames.Count() == 0)
                throw new ArgumentException("There is no element in propertyNames");

            bool hasDefaultValue = false;
            foreach (string propertyName in propertyNames)
            {
                PropertyInfo property = source.GetProperty(propertyName);
                hasDefaultValue = object.Equals(
                    source.GetPropertyValue(property), 
                    property.PropertyType.GetDefaultValue());

                // Exit if any of properties has default value
                if (hasDefaultValue)
                    break;
            }

            return hasDefaultValue;      
        }

        /// <summary>
        /// Get deault value for the type.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When type is null.
        /// </exception>
        /// <param name="type">Type to get default value of.</param>
        /// <returns>Default value accroding to type.</returns>
        public static object GetDefaultValue(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }


        /// <summary>
        /// Construct equality expression for source according to property name.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When parameterExp or source or propertyName is null.
        /// </exception>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <param name="parameterExp">Parameter expression.</param>
        /// <param name="source">Source to construct expression on.</param>
        /// <param name="propertyName">Name of property to construct expression according to.</param>
        /// <returns>Equality expression.</returns>
        public static Expression ConstructEqualityExpression<TSource>(
            this ParameterExpression parameterExp,
            TSource source,
            string propertyName)
            where TSource : class
        {
            if (parameterExp == null)
                throw new ArgumentNullException("parameterExp");
            if (source == null)
                throw new ArgumentNullException("source");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            Expression left = Expression.Property(parameterExp,
                source.GetProperty(propertyName));
            Expression right = Expression.Constant(source.GetPropertyValue(propertyName));

            // If any of types is Nullable type, convert the other to Nullable type
            // to be able to compare them
            if (left.Type.IsNullable() && !right.Type.IsNullable())
                right = Expression.Convert(right, left.Type);
            else if (!left.Type.IsNullable() && right.Type.IsNullable())
                left = Expression.Convert(left, right.Type);

            return Expression.Equal(left, right);
        }

        /// <summary>
        /// Construct equality expression for source according to property.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When parameterExp or source or property is null.
        /// </exception>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <param name="parameterExp">Parameter expression.</param>
        /// <param name="source">Source to construct expression on.</param>
        /// <param name="property">Property to construct expression according to.</param>
        /// <returns>Equality expression.</returns>
        public static Expression ConstructEqualityExpression<TSource>(
            this ParameterExpression parameterExp,
            TSource source,
            PropertyInfo property)
            where TSource : class
        {
            if (parameterExp == null)
                throw new ArgumentNullException("parameterExp");
            if (source == null)
                throw new ArgumentNullException("source");
            if (property == null)
                throw new ArgumentNullException("property");

            Expression left = Expression.Property(parameterExp, property);
            Expression right = Expression.Constant(source.GetPropertyValue(property));

            return Expression.Equal(left, right);
        }

        /// <summary>
        /// Construct equality expressions for source according to list of property names.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When parameterExp or source or propertyNames is null.
        /// </exception>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <param name="parameterExp">Parameter expression.</param>
        /// <param name="source">Source to construct expression on.</param>
        /// <param name="propertyNames">List of property names.</param>
        /// <returns>List of equality expressions.</returns>
        public static IEnumerable<Expression> ConstructEqualityExpressions<TSource>(
            this ParameterExpression parameterExp,
            TSource source,
            IEnumerable<string> propertyNames)
            where TSource : class
        {
            if (parameterExp == null)
                throw new ArgumentNullException("parameterExp");
            if (source == null)
                throw new ArgumentNullException("source");
            if (propertyNames == null)
                throw new ArgumentNullException("propertyNames");

            foreach (string propertyName in propertyNames)
                yield return parameterExp.ConstructEqualityExpression(
                    source,
                    propertyName);
        }

        /// <summary>
        /// Construct equality expressions for source according to list of properties.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When parameterExp or source or properties is null.
        /// </exception>
        /// <typeparam name="TSource">Type of source.</typeparam>
        /// <param name="parameterExp">Parameter expression.</param>
        /// <param name="source">Source to construct expression on.</param>
        /// <param name="properties">List of properties.</param>
        /// <returns>List of equality expressions.</returns>
        public static IEnumerable<Expression> ConstructEqualityExpressions<TSource>(
            this ParameterExpression parameterExp,
            TSource source,
            IEnumerable<PropertyInfo> properties)
            where TSource : class
        {
            if (parameterExp == null)
                throw new ArgumentNullException("parameterExp");
            if (source == null)
                throw new ArgumentNullException("source");
            if (properties == null)
                throw new ArgumentNullException("properties");

            foreach (PropertyInfo property in properties)
                yield return parameterExp.ConstructEqualityExpression(
                    source,
                    property);
        }

        /// <summary>
        /// Chain all expressions in the list by And expression.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When expressions is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When there is no expression in expressions.
        /// </exception>
        /// <typeparam name="T">Type of model.</typeparam>
        /// <param name="expressions">List of expressions to chain.</param>
        /// <returns>Constructed expression.</returns>
        public static Expression<Func<T, bool>> ConstructAndChain<T>(
            this IEnumerable<Expression<Func<T, bool>>> expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException("expressions");
            if (expressions.Count() == 0)
                throw new ArgumentOutOfRangeException(
                    "expressions",
                    "There must be at least one expression in expressions");

            ParameterExpression parameterExp = Expression.Parameter(typeof(T), "m");

            Expression andChainedExpression = null;
            Expression visitedExp;
            foreach (Expression<Func<T, bool>> exp in expressions)
            {
                // Replace paremeter in expression
                ExpressionParameterReplacer parameterReplacer =
                    new ExpressionParameterReplacer(exp.Parameters.First(), parameterExp);
                visitedExp = parameterReplacer.Visit(exp.Body);

                if (andChainedExpression == null)
                    andChainedExpression = visitedExp;
                else
                    andChainedExpression = Expression
                        .AndAlso(andChainedExpression, visitedExp);
            }

            return Expression.Lambda<Func<T, bool>>(
                andChainedExpression, parameterExp);
        }

        /// <summary>
        /// Chain all expressions in the list by And expression.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When expressions is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When there is no expression in expressions.
        /// </exception>
        /// <param name="expressions">List of expressions to chain.</param>
        /// <returns>Constructed expression.</returns>
        public static Expression ConstructAndChain(
            this IEnumerable<Expression> expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException("expressions");
            if (expressions.Count() == 0)
                throw new ArgumentOutOfRangeException("expressions", "There must be at least one expression in expressions");

            Expression andChainedExpression = null;
            foreach (Expression exp in expressions)
            {
                if (andChainedExpression == null)
                    andChainedExpression = exp;
                else
                    andChainedExpression = Expression.AndAlso(andChainedExpression, exp);
            }

            return andChainedExpression;
        }

        /// <summary>
        /// Chain all expressions in the list by Or expression.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When expressions is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When there is no expression in expressions.
        /// </exception>
        /// <param name="expressions">List of expressions to chain.</param>
        public static Expression ConstructOrChain(
            this IEnumerable<Expression> expressions)
        {
            if (expressions == null)
                throw new ArgumentNullException("expressions");
            if (expressions.Count() == 0)
                throw new ArgumentOutOfRangeException("expressions", "There must be at least one expression in expressions");

            Expression orChainedExpression = null;
            foreach (Expression exp in expressions)
            {
                if (orChainedExpression == null)
                    orChainedExpression = exp;
                else
                    orChainedExpression = Expression.OrElse(orChainedExpression, exp);
            }

            return orChainedExpression;
        }

        /// <summary>
        /// Get element type of IEnumerable.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When source is null.
        /// </exception>
        /// <param name="source">Source to get element from.</param>
        /// <returns>Element type of IEnumerable.</returns>
        public static Type GetElementType(this IEnumerable source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Type elementType = null;

            Type enumerableType = source.GetType();
            if (enumerableType.IsArray)
                elementType = enumerableType.GetElementType();

            if (enumerableType.IsGenericType)
                elementType = enumerableType.GenericTypeArguments.First();

            return elementType;
        }

        /// <summary>
        /// Get underlying type of property.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When typeToGetUnderlying is null.
        /// </exception>
        /// <param name="property">Property to get type of.</param>
        /// <returns>Underlying type of property.</returns>
        public static Type GetUnderlyingType(this Type typeToGetUnderlying)
        {
            if (typeToGetUnderlying == null)
                throw new ArgumentNullException("typeToGetUnderlying");

            Type propertyType = typeToGetUnderlying; //property.PropertyType;

            if (propertyType.IsGenericType)
                propertyType = propertyType.GenericTypeArguments.First().GetUnderlyingType();

            return propertyType;
        }

        /// <summary>
        /// Cast non-generic IEnumerable to IEnumerable<>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When source is null.
        /// </exception>
        /// <param name="source">Source to convert.</param>
        /// <returns>Generic IEnumerable.</returns>
        public static IEnumerable<object> CastToGeneric(this IEnumerable source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            MethodInfo castMethod = typeof(Enumerable).GetMethod("Cast");
            MethodInfo genericCastMethod = castMethod
                .MakeGenericMethod(source.GetElementType());

            return (IEnumerable<object>)genericCastMethod.Invoke(
                null, new object[] { source });
        }

        /// <summary>
        /// Construct string as {PropertyName} : {PropertyValue} for supplied
        /// property names from entity.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When entity or propertyNames is null.
        /// </exception>
        /// <typeparam name="T">Type of entity.</typeparam>
        /// <param name="entity">Entity to get property values from.</param>
        /// <param name="propertyNames">Names of properties.</param>
        /// <returns>Constructed info.</returns>
        public static string GetPropertyData<T>(
            this T entity,
            IEnumerable<string> propertyNames)
            where T : class
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (propertyNames == null)
                throw new ArgumentNullException("propertyNames");

            StringBuilder strBuilder = new StringBuilder();

            foreach (string propertyName in propertyNames)
            {
                if (strBuilder.Length > 0)
                    strBuilder.Append(" | ");

                strBuilder.AppendFormat("{0} : {1}",
                    propertyName,
                    entity.GetPropertyValue(propertyName));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Construct string as {PropertyName} : {PropertyValue} for supplied
        /// properties from entity.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// When entity or properties is null.
        /// </exception>
        /// <typeparam name="T">Type of entity.</typeparam>
        /// <param name="entity">Entity to get property values from.</param>
        /// <param name="properties">Properties.</param>
        /// <returns>Constructed info.</returns>
        public static string GetPropertyData<T>(
            this T entity,
            IEnumerable<PropertyInfo> properties)
            where T : class
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (properties == null)
                throw new ArgumentNullException("properties");

            StringBuilder strBuilder = new StringBuilder();

            foreach (PropertyInfo propertyName in properties)
            {
                strBuilder.AppendFormat("{0} : {1}\n",
                    propertyName,
                    entity.GetPropertyValue(propertyName));
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Generate member access expression according to type and property name.
        /// </summary>
        /// <remarks>
        /// Handles nested properties.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// When modelType or propertyName is null.
        /// </exception>
        /// <param name="modelType">Type of model.</param>
        /// <param name="propertyName">Name of property to generate expression according to.</param>
        /// <returns>Lambda expression to access to the member.</returns>
        public static LambdaExpression GenerateMemberAccessExpression(
           this Type modelType,
           string propertyName)
        {
            if (modelType == null)
                throw new ArgumentNullException(nameof(modelType));

            ParameterExpression parameterExp = Expression.Parameter(modelType, "m");
            Expression currentExpression = parameterExp;
            MemberExpression propertyAccessExpression = null;
            PropertyInfo property = null;

            /// It can be nested property access call so, try to split
            /// according to '.' sign and make member access expression
            /// step by step.
            foreach (string splitedPropertyName in propertyName.Split('.'))
            {
                property = modelType.GetProperty(splitedPropertyName);
                propertyAccessExpression = Expression
                    .MakeMemberAccess(currentExpression, property);

                // Store next model type and current expression to use in next
                // member access call, if it will be needed.
                modelType = property.PropertyType;
                currentExpression = propertyAccessExpression;
            }

            LambdaExpression lambdaExpression =
                Expression.Lambda(propertyAccessExpression, parameterExp);

            return lambdaExpression;
        }

        /// <summary>
        /// Get PropertyInfo according to property name.
        /// </summary>
        /// <remarks>
        /// Handles nested proeprties.
        /// </remarks>
        /// <param name="modelType">Type of model to get property.</param>
        /// <param name="propertyName">Name of proeprty to get.</param>
        /// <returns>PopertyInfo.</returns>
        public static PropertyInfo GetPropertyInfo(
            this Type modelType,
            string propertyName)
        {
            if (modelType == null)
                throw new ArgumentNullException(nameof(modelType));

            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            PropertyInfo propertyInfo = null;

            /// It can be nested property call so try to split according to
            /// '.' sign and get chained properties.
            foreach (string splitedPropertyName in propertyName.Split('.'))
            {
                propertyInfo = modelType.GetProperty(splitedPropertyName);
                modelType = propertyInfo.PropertyType;
            }

            return propertyInfo;
        }
    }
}
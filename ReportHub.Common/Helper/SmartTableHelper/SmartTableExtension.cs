using ReportHub.Common.Helper.SmartTableHelper.SmartTableInterface;
using System.Linq.Expressions;
using System.Reflection;

namespace ReportHub.Common.Helper.SmartTableHelper;

public class SmartTableExtension : ISmartTableExtension
{
    public SmartTableResult<TModel> ToSmartTableResult<TModel>(IQueryable<TModel> query, SmartTableParam param)
    {
        try
        {
            var totalRecord = query.Count();
            var items = AppendSortAndPagingation(query, param).ToList();

            return new SmartTableResult<TModel>
            {
                Items = items,
                TotalRecord = totalRecord,
                NumberOfPages = (int)Math.Ceiling((double)totalRecord / param.Pagination.Number)
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public SmartTableResult<TResult> ToSmartTableResult<TModel, TResult>(IQueryable<TModel> query, SmartTableParam param, Expression<Func<TModel, TResult>> selector)
    {
        try
        {
            var totalRecord = query.Count();
            query = AppendSortAndPagingation(query, param);
            var items = query.Select(selector).ToList();

            return new SmartTableResult<TResult>
            {
                Items = items,
                TotalRecord = totalRecord,
                NumberOfPages = (int)Math.Ceiling((double)totalRecord / param.Pagination.Number)
            };
        }
        catch (Exception ex)
        {

            throw ex;
        }

    }


    public SmartTableResult<TResult> ToSmartTableResultNoProjection<TModel, TResult>(IQueryable<TModel> query, SmartTableParam param, Expression<Func<TModel, TResult>> selector)
    {
        try
        {
            var totalRecord = query.Count();
            var items = AppendSortAndPagingation(query, param).ToList();

            return new SmartTableResult<TResult>
            {
                Items = items.AsQueryable().Select(selector),
                TotalRecord = totalRecord,
                NumberOfPages = (int)Math.Ceiling((double)totalRecord / param.Pagination.Number)
            };
        }
        catch (Exception ex)
        {

            throw ex;
        }

    }

    private IQueryable<TModel> AppendSortAndPagingation<TModel>(IQueryable<TModel> query, SmartTableParam param)
    {
        try
        {
            if (param.Pagination.Number <= 0)
            {
                param.Pagination.Number = 10;
            }

            if (!string.IsNullOrWhiteSpace(param.Sort.Predicate))
            {
                query = OrderByName(query, param.Sort.Predicate, param.Sort.Reverse);
            }
            //else
            //{
            //query = OrderByName(query, "Id", true);
            //}
            query = query
                .Skip((param.Pagination.Start - 1) * param.Pagination.Number)
                .Take(param.Pagination.Number);

            return query;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public IQueryable<T> OrderByName<T>(IQueryable<T> source, string propertyName, bool isDescending)
    {
        try
        {
            if (source == null)
            {
                throw new ArgumentException(nameof(source));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException(nameof(propertyName));
            }

            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            Expression expression = Expression.Property(arg, propertyInfo);
            type = propertyInfo.PropertyType;

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expression, arg);

            var methodName = isDescending ? "OrderByDescending" : "OrderBy";
            object result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, lambda });
            return (IQueryable<T>)result;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
}


using System.Linq.Expressions;

namespace ReportHub.Common.Helper.SmartTableHelper.SmartTableInterface;

public interface ISmartTableExtension
{
    SmartTableResult<TModel> ToSmartTableResult<TModel>(IQueryable<TModel> query, SmartTableParam param);
    SmartTableResult<TResult> ToSmartTableResult<TModel, TResult>(IQueryable<TModel> query, SmartTableParam param, Expression<Func<TModel, TResult>> selector);
    SmartTableResult<TResult> ToSmartTableResultNoProjection<TModel, TResult>(IQueryable<TModel> query, SmartTableParam param, Expression<Func<TModel, TResult>> selector);
    IQueryable<T> OrderByName<T>(IQueryable<T> source, string propertyName, bool isDescending);
}


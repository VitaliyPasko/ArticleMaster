namespace ArticleMaster.Application.Interfaces;

public interface IRepository<T>
{
    Task Create(T entity);
}
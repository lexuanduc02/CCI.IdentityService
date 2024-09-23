namespace CCI.Domain.Contractors
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}

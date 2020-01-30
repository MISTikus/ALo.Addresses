namespace ALo.Addresses.Data.Models
{
    public interface IHasId<TKey>
    {
        TKey Id { get; set; }
    }
}

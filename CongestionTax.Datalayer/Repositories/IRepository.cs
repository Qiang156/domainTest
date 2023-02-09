using CongestionTax.Domain;

namespace CongestionTax.DataLayer.Repositories
{
    public interface IRepository<T> where T : EntityBase
    {
        public T? Get(string reference);
        public void Upsert(T dailyFee);
    }
}
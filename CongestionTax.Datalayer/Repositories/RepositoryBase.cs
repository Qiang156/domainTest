using CongestionTax.Domain;

namespace CongestionTax.DataLayer.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : Domain.EntityBase
    {
        private readonly List<T> _records;

        protected RepositoryBase()
        {
            _records = new List<T>();
        }

        public void Upsert(T dailyFee)
        {
            var index = _records.FindIndex(r =>
                string.Equals(r.Reference, dailyFee.Reference, StringComparison.InvariantCultureIgnoreCase));

            if (index != -1)
            {
                _records[index] = dailyFee;
            }
            else
            {
                _records.Add(dailyFee);
            }
        }

        public T? Get(string reference)
        {
            return _records.Find(r => string.Equals(r.Reference, reference, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
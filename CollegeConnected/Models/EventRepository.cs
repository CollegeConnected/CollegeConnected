namespace CollegeConnected.Models
{
    using CollegeConnected.Models;

    public class EventRepository : IRepository
    {

        public static Event Get(object id)
        {
            using (CollegeConnectedDbContext db = new CollegeConnectedDbContext())
            {
                var e = db.Events.Find(id);
                return e;
            }
        }

        void Attach(T entity);
        IQueryable<T> GetAll();
        void Insert(T entity);
        void Delete(T entity);
        void SubmitChanges();
    }
}
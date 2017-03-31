using System;
using CollegeConnected.Models;

namespace CollegeConnected.DataLayer
{
    public class UnitOfWork : IDisposable
    {
        private readonly CollegeConnectedDbContext db = new CollegeConnectedDbContext();

        private bool disposed;
        private GenericRepository<EventAttendance> eventAttendanceRepository;
        private GenericRepository<Event> eventRepository;
        private GenericRepository<Settings> settingsRepository;
        private GenericRepository<Constituent> studentRepository;
        private GenericRepository<User> userRepository;

        public GenericRepository<Constituent> StudentRepository
        {
            get
            {
                if (studentRepository == null)
                    studentRepository = new GenericRepository<Constituent>(db);
                return studentRepository;
            }
        }

        public GenericRepository<Event> EventRepository
        {
            get
            {
                if (eventRepository == null)
                    eventRepository = new GenericRepository<Event>(db);
                return eventRepository;
            }
        }

        public GenericRepository<EventAttendance> EventAttendanceRepository
        {
            get
            {
                if (eventAttendanceRepository == null)
                    eventAttendanceRepository = new GenericRepository<EventAttendance>(db);
                return eventAttendanceRepository;
            }
        }

        public GenericRepository<User> UserRepository
        {
            get
            {
                if (userRepository == null)
                    userRepository = new GenericRepository<User>(db);
                return userRepository;
            }
        }

        public GenericRepository<Settings> SettingsRepository
        {
            get
            {
                if (settingsRepository == null)
                    settingsRepository = new GenericRepository<Settings>(db);
                return settingsRepository;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    db.Dispose();
                disposed = true;
            }
        }
    }
}
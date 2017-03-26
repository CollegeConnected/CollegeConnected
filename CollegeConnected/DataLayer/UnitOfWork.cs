using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CollegeConnected.Models;

namespace CollegeConnected.DataLayer
{
    public class UnitOfWork : IDisposable
    {
        private CollegeConnectedDbContext db = new CollegeConnectedDbContext();
        private GenericRepository<Constituent> studentRepository;
        private GenericRepository<Event> eventRepository;
        private GenericRepository<EventAttendance> eventAttendanceRepository;
        private GenericRepository<User> userRepository;
        private GenericRepository<Settings> settingsRepository;

        private bool disposed = false;

        public GenericRepository<Constituent> StudentRepository
        {
            get
            {
                if (this.studentRepository == null)
                {
                    this.studentRepository = new GenericRepository<Constituent>(db);
                }
                return studentRepository;
            }
        }

        public GenericRepository<Event> EventRepository
        {
            get
            {
                if (this.eventRepository == null)
                {
                    this.eventRepository = new GenericRepository<Event>(db);
                }
                return eventRepository;
            }
        }
        public GenericRepository<EventAttendance> EventAttendanceRepository
        {
            get
            {
                if (this.eventAttendanceRepository == null)
                {
                    this.eventAttendanceRepository = new GenericRepository<EventAttendance>(db);
                }
                return eventAttendanceRepository;
            }
        }

        public GenericRepository<User> UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new GenericRepository<User>(db);
                }
                return userRepository;
            }
        }
        public GenericRepository<Settings> SettingsRepository
        {
            get
            {
                if (this.settingsRepository == null)
                {
                    this.settingsRepository = new GenericRepository<Settings>(db);
                }
                return settingsRepository;
            }
        }

        public void Save()
        {
            db.SaveChanges();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize((this));
        }
    }
}
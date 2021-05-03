using System.Data.Entity;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
using RealtyModel.Model.RealtyObjects;

namespace RealtorObjects.Model
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext() : base("DBConnection")
        {
            //Database.SetInitializer<DataBaseContext>(null);
            UpdateTime.Load();
            Flats.Load();
            Houses.Load();
            Locations.Load();
            Cities.Load();
            Districts.Load();
            Streets.Load();
            Customers.Load();
            Albums.Load();
            Photos.Load();
        }

        public DbSet<UpdateTime> UpdateTime { get; set; }
        
        public DbSet<Flat> Flats { get; set; }
        public DbSet<House> Houses { get; set; }
        
        public DbSet<Location> Locations { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}

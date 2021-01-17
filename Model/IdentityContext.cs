using System.Data.Entity;
using RealtyModel.Model;


namespace RealtorObjects.Model
{
    public class ClientContext : DbContext
    {
        public ClientContext() : base("ClientDBConnection")
        {
            Credentials.Load();
            Credentials.Local.CollectionChanged += (sender, e) => { this.SaveChanges(); };
        }
        public DbSet<Credential> Credentials { get; set; }
    }
}

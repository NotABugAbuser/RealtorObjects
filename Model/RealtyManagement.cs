using RealtyModel.Event;
using RealtyModel.Event.RealtyEvents;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Model.RealtyObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RealtorObjects.Model
{
    public class RealtyManagement
    {
        private Dispatcher dispatcher;
        public event UpdateFinishedEventHandler UpdateFinished;

        public Boolean HasUpdate { get; set; }

        public RealtyManagement(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        internal void ReceiveDbUpdate(ReceivedDbUpdateEventArgs e)
        {
            try
            {
                String data = Encoding.UTF8.GetString((Byte[])e.UpdateData);
                if (!String.IsNullOrWhiteSpace(data))
                {
                    if (e.TargetType == TargetType.All)
                    {
                        String[] objects = data.Split(new String[] { "<FLATS>", "<HOUSES>" }, StringSplitOptions.None);

                        using (var context = new DataBaseContext())
                        {
                            if (!String.IsNullOrWhiteSpace(objects[0]))
                            {
                                Flat[] flats = JsonSerializer.Deserialize<Flat[]>(objects[0]);
                                Debug.WriteLine($"{DateTime.Now} UPDATE received {flats.Length} flats");
                                context.Flats.AddRange(flats);
                                context.SaveChanges();
                            }
                            else Debug.WriteLine($"{DateTime.Now} UPDATE there is no flats");
                            
                            if (!String.IsNullOrWhiteSpace(objects[1]))
                            {
                                House[] houses = JsonSerializer.Deserialize<House[]>(objects[1]);
                                Debug.WriteLine($"{DateTime.Now} UPDATE received {houses.Length} houses");
                                context.Houses.AddRange(houses);
                                context.SaveChanges();
                            }
                        }
                    }
                    else if (e.TargetType == TargetType.None)
                    {
                        using (var context = new DataBaseContext())
                        {
                            HasUpdate = true;
                            UpdateFinished?.Invoke(this, new UpdateFinishedEventArgs());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{DateTime.Now} ReceiveUpdate {ex.Message}");
                //Запросить обновление снова
            }
        }

        internal Boolean AddFlat(Flat flat)
        {
            try
            {
                using (var context = new DataBaseContext())
                {
                    context.Flats.Local.Add(flat);
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        internal Boolean UpdateFlat(Flat flat)
        {
            try
            {
                using (var context = new DataBaseContext())
                {
                    Flat dbFlat = context.Flats.Find(flat.Id);
                    dbFlat = flat;
                    context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        internal Boolean DeleteFlat(Flat flat)
        {
            try
            {
                using (var context = new DataBaseContext())
                {
                    Flat dbFlat = context.Flats.Find(flat.Id);
                    context.Flats.Remove(dbFlat);
                    context.SaveChanges();
                };
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal void AddHouse(House house)
        {
            using (var context = new DataBaseContext())
            {
                context.Houses.Add(house);
                context.SaveChanges();
            }
        }
        internal void UpdateHouse(House house)
        {
            using (var context = new DataBaseContext())
            {
                House dbHouse = context.Houses.Find(house.Id);
                dbHouse = house;
                context.SaveChanges();
            }
        }
        internal void DeleteHouse(House house)
        {
            using (var context = new DataBaseContext())
            {
                House dbHouse = context.Houses.Find(house.Id);
                context.Houses.Remove(dbHouse);
                context.SaveChanges();
            }
        }

        internal void PullPhoto(Photo photo)
        {

        }

        private LocationOptions GetLocationOptions()
        {
            LocationOptions locationOptions = new LocationOptions();
            using (var context = new DataBaseContext())
            {
                foreach (City city in context.Cities.AsNoTracking())
                    locationOptions.Cities.Add(city);
                foreach (District district in context.Districts.AsNoTracking())
                    locationOptions.Districts.Add(district);
                foreach (Street street in context.Streets.AsNoTracking())
                    locationOptions.Streets.Add(street);
            }
            return locationOptions;
        }
    }
}

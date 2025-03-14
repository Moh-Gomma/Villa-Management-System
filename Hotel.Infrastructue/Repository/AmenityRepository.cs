using Hotel.Application.Common.Interfaces;
using Hotel.Domain.Entities;
using Hotel.Infrastructue.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Infrastructue.Repository
{
    public class AmenityRepository : Repository<Amenity> , IAmenityRepository
    {
        private readonly ApplicationDbContext _db;

        public AmenityRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Amenity entity)
        {
            _db.Amenities.Update(entity);
        }

    }
}

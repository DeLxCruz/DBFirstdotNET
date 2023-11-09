using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Persistence.Data;

namespace APP.Repositories
{
    public class CountryRepository : GenericRepository<Country>, ICountry
    {
        private readonly DBFirstContext context;

        public CountryRepository(DBFirstContext context) : base(context)
        {
            this.context = context;
        }
    }
}
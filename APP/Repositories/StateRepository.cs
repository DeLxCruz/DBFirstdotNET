using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Persistence.Data;

namespace APP.Repositories
{
    public class StateRepository : GenericRepository<State>, IState
    {
        private readonly DBFirstContext _context;
        public StateRepository(DBFirstContext context) : base(context)
        {
            _context = context;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Persistence.Data;

namespace API.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly DBFirstContext _context;

        public CustomerController(IUnitOfWork unitOfWork, IMapper mapper, DBFirstContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> Get()
        {
            var Customers = await _unitOfWork.Customers.GetAllAsync();
            return _mapper.Map<List<CustomerDto>>(Customers);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> Get(int id)
        {
            var Customers = await _unitOfWork.Customers.GetByIdAsync(id);

            if (Customers == null)
            {
                return NotFound();
            }

            return _mapper.Map<CustomerDto>(Customers);
        }

        [HttpGet("GetCustomerWithCity/{city}")]
        public Task<IQueryable<CustomersWithCity>> GetCustomerWitCity(string city)
        {
            var Customers = from c in _context.Customers
                            join ci in _context.Cities on c.IdcityFk equals ci.Id
                            where ci.Name == city
                            select new CustomersWithCity
                            {
                                Name = c.Name,
                                City = ci.Name
                            };
            return Task.FromResult(Customers);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Customer>> Post(CustomerDto customerDto)
        {
            var Customers = _mapper.Map<Customer>(customerDto);
            this._unitOfWork.Customers.Add(Customers);
            await _unitOfWork.SaveAsync();

            if (Customers == null)
            {
                return BadRequest();
            }
            customerDto.Id = Customers.Id;
            return CreatedAtAction(nameof(Post), new { id = customerDto.Id }, customerDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> Put(int id, [FromBody] CustomerDto customerDto)
        {
            if (customerDto.Id == 0)
            {
                customerDto.Id = id;
            }

            if(customerDto.Id != id)
            {
                return BadRequest();
            }

            if(customerDto == null)
            {
                return NotFound();
            }

            var Customers = _mapper.Map<Customer>(customerDto);
            _unitOfWork.Customers.Update(Customers);
            await _unitOfWork.SaveAsync();
            return customerDto;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var Customers = await _unitOfWork.Customers.GetByIdAsync(id);

            if (Customers == null)
            {
                return NotFound();
            }

            _unitOfWork.Customers.Remove(Customers);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
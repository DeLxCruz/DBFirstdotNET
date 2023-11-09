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

namespace API.Controllers
{
    public class CountryController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CountryDto>>> Get()
        {
            var Countries = await _unitOfWork.Countries.GetAllAsync();
            return _mapper.Map<List<CountryDto>>(Countries);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CountryDto>> Get(int id)
        {
            var Countries = await _unitOfWork.Countries.GetByIdAsync(id);

            if (Countries == null)
            {
                return NotFound();
            }

            return _mapper.Map<CountryDto>(Countries);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Country>> Post(CountryDto countryDto)
        {
            var Countries = _mapper.Map<Country>(countryDto);
            this._unitOfWork.Countries.Add(Countries);
            await _unitOfWork.SaveAsync();

            if (Countries == null)
            {
                return BadRequest();
            }
            countryDto.Id = Countries.Id;
            return CreatedAtAction(nameof(Post), new { id = countryDto.Id }, countryDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CountryDto>> Put(int id, [FromBody] CountryDto countryDto)
        {
            if (countryDto.Id == 0)
            {
                countryDto.Id = id;
            }

            if(countryDto.Id != id)
            {
                return BadRequest();
            }

            if(countryDto == null)
            {
                return NotFound();
            }

            var Countries = _mapper.Map<Country>(countryDto);
            _unitOfWork.Countries.Update(Countries);
            await _unitOfWork.SaveAsync();
            return countryDto;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var Countries = await _unitOfWork.Countries.GetByIdAsync(id);

            if (Countries == null)
            {
                return NotFound();
            }

            _unitOfWork.Countries.Remove(Countries);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
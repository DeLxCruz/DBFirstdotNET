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
    public class CityController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CityController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CityDto>>> Get()
        {
            var Cities = await _unitOfWork.Cities.GetAllAsync();
            return _mapper.Map<List<CityDto>>(Cities);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CityDto>> Get(int id)
        {
            var Cities = await _unitOfWork.Cities.GetByIdAsync(id);

            if (Cities == null)
            {
                return NotFound();
            }

            return _mapper.Map<CityDto>(Cities);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<City>> Post(CityDto cityDto)
        {
            var Cities = _mapper.Map<City>(cityDto);
            this._unitOfWork.Cities.Add(Cities);
            await _unitOfWork.SaveAsync();

            if (Cities == null)
            {
                return BadRequest();
            }
            cityDto.Id = Cities.Id;
            return CreatedAtAction(nameof(Post), new { id = cityDto.Id }, cityDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CityDto>> Put(int id, [FromBody] CityDto cityDto)
        {
            if (cityDto.Id == 0)
            {
                cityDto.Id = id;
            }

            if(cityDto.Id != id)
            {
                return BadRequest();
            }

            if(cityDto == null)
            {
                return NotFound();
            }

            var Cities = _mapper.Map<City>(cityDto);
            _unitOfWork.Cities.Update(Cities);
            await _unitOfWork.SaveAsync();
            return cityDto;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var Cities = await _unitOfWork.Cities.GetByIdAsync(id);

            if (Cities == null)
            {
                return NotFound();
            }

            _unitOfWork.Cities.Remove(Cities);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
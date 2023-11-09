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
    public class PersonTypeController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PersonTypeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PersonTypeDto>>> Get()
        {
            var PersonTypes = await _unitOfWork.PersonTypes.GetAllAsync();
            return _mapper.Map<List<PersonTypeDto>>(PersonTypes);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PersonTypeDto>> Get(int id)
        {
            var PersonTypes = await _unitOfWork.PersonTypes.GetByIdAsync(id);

            if (PersonTypes == null)
            {
                return NotFound();
            }

            return _mapper.Map<PersonTypeDto>(PersonTypes);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PersonType>> Post(PersonTypeDto personTypeDto)
        {
            var PersonTypes = _mapper.Map<PersonType>(personTypeDto);
            this._unitOfWork.PersonTypes.Add(PersonTypes);
            await _unitOfWork.SaveAsync();

            if (PersonTypes == null)
            {
                return BadRequest();
            }
            personTypeDto.Id = PersonTypes.Id;
            return CreatedAtAction(nameof(Post), new { id = personTypeDto.Id }, personTypeDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PersonTypeDto>> Put(int id, [FromBody] PersonTypeDto personTypeDto)
        {
            if (personTypeDto.Id == 0)
            {
                personTypeDto.Id = id;
            }

            if(personTypeDto.Id != id)
            {
                return BadRequest();
            }

            if(personTypeDto == null)
            {
                return NotFound();
            }

            var PersonTypes = _mapper.Map<PersonType>(personTypeDto);
            _unitOfWork.PersonTypes.Update(PersonTypes);
            await _unitOfWork.SaveAsync();
            return personTypeDto;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var PersonTypes = await _unitOfWork.PersonTypes.GetByIdAsync(id);

            if (PersonTypes == null)
            {
                return NotFound();
            }

            _unitOfWork.PersonTypes.Remove(PersonTypes);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
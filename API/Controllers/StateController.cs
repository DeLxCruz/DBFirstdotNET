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
    public class StateController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StateController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<StateDto>>> Get()
        {
            var States = await _unitOfWork.States.GetAllAsync();
            return _mapper.Map<List<StateDto>>(States);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StateDto>> Get(int id)
        {
            var States = await _unitOfWork.States.GetByIdAsync(id);

            if (States == null)
            {
                return NotFound();
            }

            return _mapper.Map<StateDto>(States);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<State>> Post(StateDto stateDto)
        {
            var States = _mapper.Map<State>(stateDto);
            this._unitOfWork.States.Add(States);
            await _unitOfWork.SaveAsync();

            if (States == null)
            {
                return BadRequest();
            }
            stateDto.Id = States.Id;
            return CreatedAtAction(nameof(Post), new { id = stateDto.Id }, stateDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StateDto>> Put(int id, [FromBody] StateDto stateDto)
        {
            if (stateDto.Id == 0)
            {
                stateDto.Id = id;
            }

            if(stateDto.Id != id)
            {
                return BadRequest();
            }

            if(stateDto == null)
            {
                return NotFound();
            }

            var States = _mapper.Map<State>(stateDto);
            _unitOfWork.States.Update(States);
            await _unitOfWork.SaveAsync();
            return stateDto;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var States = await _unitOfWork.States.GetByIdAsync(id);

            if (States == null)
            {
                return NotFound();
            }

            _unitOfWork.States.Remove(States);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }
    }
}
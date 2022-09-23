using AutoMapper;
using BusinessTier.DTOS;
using BusinessTier.Repository;
using DataTier.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AddressBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IGenericRepository<Job> _repository;
        private readonly IMapper _mapper;

        public JobController(IGenericRepository<Job> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var Jobs = await _repository.GetAllAsync();
            var model = _mapper.Map<IEnumerable<JobDto>>(Jobs);
            return new JsonResult(model);
        }

        [HttpGet("{id:int}", Name = "JobDetialsRoute")]
        public async Task<ActionResult> GetById(int id)
        {
            var Jobs = await _repository.GetByIdAsync(id);
            if (Jobs == null)
                return new JsonResult("Not exist");
            var model = _mapper.Map<JobDto>(Jobs);
            return new JsonResult(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(JobDto job)
        {
            if (ModelState.IsValid)
            {
                var model = _mapper.Map<Job>(job);
                await _repository.InsertAsync(model);
                return new JsonResult("Added Successfully !");
            }
            return new JsonResult("Something Wrong");
        }

        [HttpPut]
        public IActionResult Edit(JobDto job)
        {

            try
            {
                var model = _mapper.Map<Job>(job);
                _repository.Update(model);
                return new JsonResult("Updated Successfully !");
            }
            catch (Exception e)
            {
                return new JsonResult(e.Message);
            }


        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _repository.GetByIdAsync(id);
            if (job == null)
            {
                return new JsonResult("Not Exist");
            }
            _repository.Delete(job);
            return new JsonResult("Deleted Successfully !  ");
        }
    }
}

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
    public class EntryController : ControllerBase
    {
        private readonly IGenericRepository<Entry> _repository;
        private readonly IGenericRepository<Department> _department;
        private readonly IGenericRepository<Job> _job;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public EntryController(
            IGenericRepository<Entry> repository,
            IGenericRepository<Department> department,
            IGenericRepository<Job> job,
            IMapper mapper,
            IWebHostEnvironment env)
        {
            _repository = repository;
            _department = department;
            _job = job;
            _mapper = mapper;
            _env = env;
        }

        [Route("GetAllDepartmentsNames")]
        [HttpGet]
        public async Task<IActionResult> GetAllDepartmentsNames()
        {
            var Departments = await _department.GetAllAsync();
            var model = _mapper.Map<IEnumerable<DepartmentsNamesDto>>(Departments);
            return new JsonResult(model);
        }

        [Route("GetAllJobsNames")]
        [HttpGet]
        public async Task<IActionResult> GetAllJobsNames()
        {
            var jobs = await _job.GetAllAsync();
            var model = _mapper.Map<IEnumerable<JobsNamesDto>>(jobs);
            return new JsonResult(model);
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string fileName = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + fileName;
                using(var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                return new JsonResult(fileName);
            }
            catch (Exception)
            {

                return new JsonResult("anonymouse.png");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var entries = await _repository.GetAllAsync();
            var model = _mapper.Map<IEnumerable<EntryDto>>(entries);
            return new JsonResult(model);
        }

        [HttpGet("{id:int}", Name = "EntryDetialsRoute")]
        public async Task<ActionResult> GetById(int id)
        {
            var entry = await _repository.GetByIdAsync(id);
            if (entry == null)
                return new JsonResult("Not Exist");
            var model = _mapper.Map<EntryDto>(entry);
            return new JsonResult(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EntryDto entry)
        {
            if (ModelState.IsValid)
            {
                var model = _mapper.Map<Entry>(entry);
                await _repository.InsertAsync(model);
                return new JsonResult("Added Sucessfully !");
            }
            return new JsonResult("Something Wrong");
        }

        [HttpPut]
        public IActionResult Edit(EntryDto entry)
        {

            try
            {
                var model = _mapper.Map<Entry>(entry);
                _repository.Update(model);
                return new JsonResult("Updated Successfully !");
            }
            catch (DbUpdateConcurrencyException e)
            {
                return new JsonResult(e.Message);
            }


        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entry = await _repository.GetByIdAsync(id);
            if (entry == null)
            {
                return new JsonResult("Not Exist");
            }
            _repository.Delete(entry);
            return new JsonResult("Deleted Successfully !");
        }
    }
}
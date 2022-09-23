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
    public class DepartmentController : ControllerBase
    {
        private readonly IGenericRepository<Department> _repository;
        private readonly IMapper _mapper;
        public DepartmentController(IGenericRepository<Department> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var departments = await _repository.GetAllAsync();
            var model = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
            return new JsonResult(model);
        }

        [HttpGet("{id:int}", Name = "DepartmentDetialsRoute")]
        public async Task<ActionResult> GetById(int id)
        {
            var Department = await _repository.GetByIdAsync(id);
            if (Department == null)
                return new JsonResult("NotFound");
            var model = _mapper.Map<DepartmentDto>(Department);
            return new JsonResult(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentDto department)
        {
            if (ModelState.IsValid)
            {
                var model = _mapper.Map<Department>(department);
                await _repository.InsertAsync(model);
                return new JsonResult("Added Successfully !");
            }
            return new JsonResult("Something wrong");
        }

        [HttpPut]
        public IActionResult Edit(DepartmentDto department)
        {
      
            try
            {
                var model = _mapper.Map<Department>(department);
                _repository.Update(model);
                return new JsonResult("Updated Successfully !");
            }
            catch(DbUpdateConcurrencyException e)
            {
                return new JsonResult(e.Message);
            }


        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _repository.GetByIdAsync(id);
            if (department == null)
            {
                return new JsonResult("Not Exist");
            }
            _repository.Delete(department);
            return new JsonResult("Deleted Successfully !");
        }
    }
}

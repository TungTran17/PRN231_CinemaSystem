using BussinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Repositories.impl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace CinemaSystemManagermentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository = new CategoryRepository();

        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<ActionResult<Category>> Get(int key)
        {
            var category = await _categoryRepository.getCategoryWithFilms(key);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category); 
        }
    }
}
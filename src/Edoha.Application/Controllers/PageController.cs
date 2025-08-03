using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.DTOs.Page;
using Microsoft.AspNetCore.Mvc;

namespace Edoha.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageService;

        public PageController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pages = await _pageService.SelectAllPages();

            if (pages == null || !pages.Any())
            {
                return NoContent();
            }

            return Ok(pages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var page = await _pageService.SelectPageById(id);

            if (page == null)
            {
                return NotFound();
            }

            return Ok(page);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePage request)
        {
            if (request == null)
            {
                return BadRequest("Dados incompletos ou não enviados");
            }

            await _pageService.InsertPage(request);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePage request)
        {
            if (request == null)
            {
                return BadRequest("Dados incompletos ou não enviados");
            }

            await _pageService.UpdatePageById(request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(Guid id)
        {
            await _pageService.DeletePageById(id);
            return Ok();
        }
    }
}

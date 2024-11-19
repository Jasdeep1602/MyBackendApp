using Microsoft.AspNetCore.Mvc;

namespace MyBackendApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListItemController : ControllerBase
    {
        private readonly ListItemService _listItemService;

        public ListItemController(ListItemService listItemService)
        {
            _listItemService = listItemService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ListItem>>> Get() => 
            await _listItemService.GetAllAsync();

        [HttpGet("{id:length(24)}", Name = "GetListItem")]
        public async Task<ActionResult<ListItem>> Get(string id)
        {
            var item = await _listItemService.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        [HttpPost]
        public async Task<ActionResult<ListItem>> Create(ListItem newItem)
        {
            await _listItemService.CreateAsync(newItem);
            return CreatedAtRoute("GetListItem", new { id = newItem.Id }, newItem);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, ListItem updatedItem)
        {
            var item = await _listItemService.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _listItemService.UpdateAsync(id, updatedItem);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _listItemService.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            await _listItemService.DeleteAsync(id);
            return NoContent();
        }
    }
}

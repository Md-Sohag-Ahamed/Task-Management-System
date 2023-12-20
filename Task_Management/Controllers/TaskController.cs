using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_Management.Models;

namespace Task_Management.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tasks
        [HttpGet]
        public IActionResult GetTasks()
        {
            var tasks = _context.Tasks.ToList();
            return Ok(tasks);
        }

        // GET: api/tasks/{id}
        [HttpGet("{id}")]
        public IActionResult GetTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        // POST: api/tasks
        [HttpPost]
        public IActionResult CreateTask([FromBody] tblTask task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] tblTask updatedTask)
        {
            var task = _context.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }

            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.DueDate = updatedTask.DueDate;
            task.Status = updatedTask.Status;

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

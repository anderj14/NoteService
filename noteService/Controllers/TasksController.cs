using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteService.Dtos;
using NoteService.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Task = NoteService.Models.Task;

namespace NoteService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly NoteDatabaseContext _context;

        public TasksController(NoteDatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("addTask")]
        public async Task<IActionResult> CreateUserTask([FromBody] TaskDto task)
        {
            try
            {
                int userID = Convert.ToInt32(HttpContext.User.FindFirstValue("userID"));

                if (task == null)
                {
                    return BadRequest("You can not create an invalid task");
                }

                if (userID == null || userID <= 0)
                {
                    return BadRequest("This user is not allowed to use this endpoint");
                }

                var newTask = new Task
                {
                    UserId = userID,
                    TaskTitle = task.TaskTitle,
                    TaskDescription = task.TaskDescription,
                    CreatedDate = DateTime.Now,
                    ScheduledDate = task.ScheduledDate,
                    CompletedDate = task.CompletedDate != null ? task.CompletedDate : null
                };
                _context.Tasks.Add(newTask);
                await _context.SaveChangesAsync();

                return Ok("You have successfully created a new task");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("updateTask")]

        public async Task<IActionResult> UpdateUSerTask([FromBody] TaskDto task)
        {
            try
            {
                //get current userID
                int userID = Convert.ToInt32(HttpContext.User.FindFirstValue("userID"));

                if (task == null && task.TaskId == null || task.TaskId <= 0)
                {
                    return BadRequest("You can not create an invalid task");
                }

                if (userID == null || userID <= 0)
                {
                    return BadRequest("This user is not allowed to use this endpoint");
                }

                var dbTask = await _context.Tasks.Where(t => t.TaskId == task.TaskId && t.UserId == userID).FirstOrDefaultAsync();

                if (dbTask == null)
                {
                    return BadRequest("You cant update an non-existing task");
                }
                dbTask.TaskTitle = task.TaskTitle;
                dbTask.TaskDescription = task.TaskDescription;
                dbTask.ScheduledDate = task.ScheduledDate;
                dbTask.CompletedDate = task.CompletedDate != null ? task.CompletedDate : null;

                _context.Entry(dbTask).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                //return Ok("You have successfully update a task");
                return Ok(dbTask);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserTasks()
        {
            try
            {
                //get current userID
                int userID = Convert.ToInt32(HttpContext.User.FindFirstValue("userID"));

                if (userID == null || userID <= 0)
                {
                    return BadRequest("This user is not allowed to use this endpoint");
                }


                var taskList = _context.Tasks.Select(t => new
                {
                    t.TaskId,
                    t.UserId,
                    t.TaskTitle,
                    t.TaskDescription,
                    t.CreatedDate,
                    t.ScheduledDate,
                    t.CompletedDate
                }).Where(t => t.UserId == userID).ToList();


                return Ok(taskList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getByTaskId/{id}")]
        public async Task<IActionResult> GetUserTaskById(int id)
        {
            try
            {
                //get current userID
                int userID = Convert.ToInt32(HttpContext.User.FindFirstValue("userID"));

                if (userID == null || userID <= 0)
                {
                    return BadRequest("This user is not allowed to use this endpoint");
                }

                if(id == null || id <= 0)
                {
                    return BadRequest("This task does no exist");
                }


                var task= _context.Tasks.Select(t => new
                {
                    t.TaskId,
                    t.UserId,
                    t.TaskTitle,
                    t.TaskDescription,
                    t.CreatedDate,
                    t.ScheduledDate,
                    t.CompletedDate
                }).Where(t => t.UserId == userID && t.TaskId == id).FirstOrDefault();


                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        [Route("DeleteTaskId/{id}")]
        public async Task<IActionResult> DeleteUserTaskById(int id)
        {
            try
            {
                //get current userID
                int userID = Convert.ToInt32(HttpContext.User.FindFirstValue("userID"));

                if (userID == null || userID <= 0)
                {
                    return BadRequest("This user is not allowed to use this endpoint");
                }

                if (id == null || id <= 0)
                {
                    return BadRequest("This task does no exist");
                }


                var task = _context.Tasks.Where(t => t.UserId == userID && t.TaskId == id).FirstOrDefault();
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

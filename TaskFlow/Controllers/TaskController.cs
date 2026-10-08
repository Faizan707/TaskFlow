using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs.Tasks;
using TaskFlow.Interfaces;

namespace TaskFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITasks _tasksService;

        public TaskController(ITasks tasksService)
        {
            _tasksService = tasksService;
        }

        // CREATE — AssigneeId comes from body (Users.Id)
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpPost]
        public IActionResult CreateTask(TaskCreateDto taskDto)
        {
            if (taskDto.AssigneeId <= 0)
            {
                return BadRequest(new { message = "AssigneeId is required" });
            }

            try
            {
                var createdTask = _tasksService.CreateTask(taskDto, taskDto.AssigneeId);

                return Ok(new
                {
                    message = "Task created successfully",
                    task = createdTask
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET — Admin/Manager: all, User: only assigned to them
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpGet]
        public IActionResult GetTasks()
        {
            var userId = int.Parse(
                User.FindFirst("userId")!.Value
            );

            var role = User.FindFirst("role")?.Value;

            if (role == null)
                return Forbid();

            if (role == "Admin" || role == "Manager")
            {
                return Ok(_tasksService.GetAllTasks());
            }

            return Ok(_tasksService.GetUserTasks(userId));
        }

        // UPDATE
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpPut("{taskId}")]
        public IActionResult UpdateTask(int taskId, TaskUpdateDto taskDto)
        {
            var userId = int.Parse(
                User.FindFirst("userId")!.Value
            );

            var role = User.FindFirst("role")?.Value;

            if (role == null)
                return Forbid();

            var task = _tasksService.UpdateTask(
                taskId,
                taskDto,
                userId,
                role
            );

            if (task == null)
                return Forbid();

            return Ok(new
            {
                message = "Task updated successfully",
                task
            });
        }

        // MOVE — change stage (and sync status for default stages)
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpPut("{taskId}/move")]
        public IActionResult MoveTask(int taskId, TaskMoveDto moveDto)
        {
            if (moveDto.StageId <= 0)
                return BadRequest(new { message = "StageId is required" });

            var userId = int.Parse(
                User.FindFirst("userId")!.Value
            );

            var role = User.FindFirst("role")?.Value;

            if (role == null)
                return Forbid();

            var task = _tasksService.MoveTask(
                taskId,
                moveDto.StageId,
                userId,
                role
            );

            if (task == null)
                return Forbid();

            return Ok(new
            {
                message = "Task moved successfully",
                task
            });
        }

        // DELETE
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpDelete("{taskId}")]
        public IActionResult DeleteTask(int taskId)
        {
            var userId = int.Parse(
                User.FindFirst("userId")!.Value
            );

            var role = User.FindFirst("role")?.Value;

            if (role == null)
                return Forbid();

            var deleted = _tasksService.DeleteTask(
                taskId,
                userId,
                role
            );

            if (!deleted)
                return Forbid();

            return Ok(new
            {
                message = "Task deleted successfully"
            });
        }
    }
}

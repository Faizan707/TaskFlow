using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs.Projects;
using TaskFlow.Interfaces;

namespace TaskFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // CREATE PROJECT
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpPost]
        public IActionResult CreateProject(ProjectCreateDto projectDto)
        {
            var userId = int.Parse(
                User.FindFirst("userId")!.Value
            );

            var project = new Models.Project
            {
                name = projectDto.name,
                description = projectDto.description
            };

            var createdProject = _projectService.CreateProject(
                project,
                userId
            );

            return Ok(new
            {
                message = "Project created successfully",
                project = createdProject
            });
        }

        // GET USER'S PROJECTS
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpGet]
        public IActionResult GetProjects()
        {
            var userId = int.Parse(
                User.FindFirst("userId")!.Value
            );

            var role = User.FindFirst("role")?.Value;

            if (role == null)
                return Forbid();

            if (role == "Admin" || role == "Manager")
            {
                return Ok(_projectService.GetAllProjects());
            }

            return Ok(_projectService.GetUserProjects(userId));
        }

        // UPDATE
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpPut("{projectId}")]
        public IActionResult UpdateProject(
            int projectId,
            ProjectUpdateDto projectDto)
        {
            var userId = int.Parse(
                User.FindFirst("userId")!.Value
            );

            var role = User.FindFirst("role")?.Value;

            if (role == null)
                return Forbid();

            var project = _projectService.UpdateProject(
                projectId,
                projectDto,
                userId,
                role
            );

            if (project == null)
                return Forbid();

            return Ok(new
            {
                message = "Project updated successfully",
                project
            });
        }

        // DELETE
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpDelete("{projectId}")]
        public IActionResult DeleteProject(int projectId)
        {
            var userId = int.Parse(
                User.FindFirst("userId")!.Value
            );

            var role = User.FindFirst("role")?.Value;

            if (role == null)
                return Forbid();

            var deleted = _projectService.DeleteProject(
                projectId,
                userId,
                role
            );

            if (!deleted)
                return Forbid();

            return Ok(new
            {
                message = "Project deleted successfully"
            });
        }
    }
}
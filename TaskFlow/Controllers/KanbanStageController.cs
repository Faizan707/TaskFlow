using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.DTOs.Kanban;
using TaskFlow.Interfaces;

namespace TaskFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KanbanStageController : ControllerBase
    {
        private readonly IKanbanStageService _kanbanStageService;

        public KanbanStageController(IKanbanStageService kanbanStageService)
        {
            _kanbanStageService = kanbanStageService;
        }

        // GET stages for a project (creates default 3 if missing)
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpGet("project/{projectId}")]
        public IActionResult GetProjectStages(int projectId)
        {
            var stages = _kanbanStageService.GetProjectStages(projectId);
            return Ok(stages);
        }

        // CREATE custom stage
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpPost]
        public IActionResult CreateStage(KanbanStageCreateDto dto)
        {
            var stage = _kanbanStageService.CreateStage(dto);

            return Ok(new
            {
                message = "Stage created successfully",
                stage
            });
        }

        // UPDATE stage name / order
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpPut("{stageId}")]
        public IActionResult UpdateStage(int stageId, KanbanStageUpdateDto dto)
        {
            var stage = _kanbanStageService.UpdateStage(stageId, dto);

            if (stage == null)
                return NotFound(new { message = "Stage not found" });

            return Ok(new
            {
                message = "Stage updated successfully",
                stage
            });
        }

        // DELETE custom stage only
        [Authorize(Roles = "User,Manager,Admin")]
        [HttpDelete("{stageId}")]
        public IActionResult DeleteStage(int stageId)
        {
            var deleted = _kanbanStageService.DeleteStage(stageId);

            if (!deleted)
            {
                return BadRequest(new
                {
                    message = "Cannot delete stage. Default stages or stages with tasks cannot be deleted."
                });
            }

            return Ok(new { message = "Stage deleted successfully" });
        }
    }
}

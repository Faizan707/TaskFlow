using TaskFlow.DTOs.Kanban;

namespace TaskFlow.Interfaces
{
    public interface IKanbanStageService
    {
        List<KanbanStageListDto> GetProjectStages(int projectId);
        KanbanStageListDto CreateStage(KanbanStageCreateDto dto);
        KanbanStageListDto? UpdateStage(int stageId, KanbanStageUpdateDto dto);
        bool DeleteStage(int stageId);
        void EnsureDefaultStages(int projectId);
    }
}

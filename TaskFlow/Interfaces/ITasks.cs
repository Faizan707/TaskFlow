using TaskFlow.DTOs.Tasks;

namespace TaskFlow.Interfaces
{
    public interface ITasks
    {
        TaskListDto CreateTask(TaskCreateDto task, int assigneeId);
        List<TaskListDto> GetAllTasks();
        List<TaskListDto> GetUserTasks(int userId);
        TaskListDto? UpdateTask(
            int taskId,
            TaskUpdateDto taskDto,
            int userId,
            string role
        );
        TaskListDto? MoveTask(
            int taskId,
            int stageId,
            int userId,
            string role
        );
        bool DeleteTask(int taskId, int userId, string role);
    }
}

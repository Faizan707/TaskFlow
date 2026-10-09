using TaskFlow.Interfaces;
using TaskFlow.Services;

namespace TaskFlow.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ITasks, TasksService>();
            services.AddScoped<IKanbanStageService, KanbanStageService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}

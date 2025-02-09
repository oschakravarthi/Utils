namespace SubhadraSolutions.Utils.Pivoting.Blazor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLeaderPivot(this IServiceCollection services)
    {
        return Plk.Blazor.DragDrop.ServiceCollectionExtensions.AddBlazorDragDrop(services);
    }
}

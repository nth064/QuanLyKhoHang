using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using QuanLyKhoHang.Models;

public class RoleInitializerHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public RoleInitializerHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { SD.Role_Admin, SD.Role_StaffStockIn, SD.Role_StaffStockOut, SD.Role_User };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

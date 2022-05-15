#nullable enable
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Maintenance;
using LBPUnion.ProjectLighthouse.Servers.Website.Pages.Layouts;
using LBPUnion.ProjectLighthouse.Types;
using Microsoft.AspNetCore.Mvc;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Pages.Admin;

public class AdminPanelPage : BaseLayout
{
    public List<ICommand> Commands = MaintenanceHelper.Commands;
    public AdminPanelPage(Database database) : base(database)
    {}

    public List<AdminPanelStatistic> Statistics = new();

    public async Task<IActionResult> OnGet([FromQuery] string? args, [FromQuery] string? command, [FromQuery] string? maintenanceJob)
    {
        User? user = this.Database.UserFromWebRequest(this.Request);
        if (user == null) return this.Redirect("~/login");
        if (!user.IsAdmin) return this.NotFound();

        this.Statistics.Add(new AdminPanelStatistic("Users", await StatisticsHelper.UserCount(), "users"));
        this.Statistics.Add(new AdminPanelStatistic("Slots", await StatisticsHelper.SlotCount()));
        this.Statistics.Add(new AdminPanelStatistic("Photos", await StatisticsHelper.PhotoCount()));
        this.Statistics.Add(new AdminPanelStatistic("Reports", await StatisticsHelper.ReportCount(), "reports/0"));

        if (!string.IsNullOrEmpty(command))
        {
            args ??= "";
            args = command + " " + args;
            string[] split = args.Split(" ");
            await MaintenanceHelper.RunCommand(split);
            return this.Redirect("~/admin");
        }

        if (!string.IsNullOrEmpty(maintenanceJob))
        {
            await MaintenanceHelper.RunMaintenanceJob(maintenanceJob);
            return this.Redirect("~/admin");
        }

        return this.Page();
    }
}
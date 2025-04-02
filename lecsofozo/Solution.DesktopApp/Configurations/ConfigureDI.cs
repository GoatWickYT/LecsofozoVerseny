using Solution.Services.Services;

namespace Solution.DesktopApp.Configurations;

public static class ConfigureDI
{
	public static MauiAppBuilder UseDIConfiguration(this MauiAppBuilder builder)
	{
		builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<MainView>();

		builder.Services.AddTransient<CreateOrEditTeamViewModel>();
		builder.Services.AddTransient<CreateOrEditTeamView>();

        builder.Services.AddScoped<IGoogleDriveService, GoogleDriveService> ();
        builder.Services.AddScoped<ITeamService, TeamService> ();
        builder.Services.AddScoped<IParticipantService, ParticipantService> ();

        return builder;
	}
}

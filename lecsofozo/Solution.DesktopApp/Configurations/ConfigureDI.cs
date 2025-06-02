namespace Solution.DesktopApp.Configurations;

public static class ConfigureDI
{
	public static MauiAppBuilder UseDIConfiguration(this MauiAppBuilder builder)
	{
        builder.Services.AddTransient<MainView>();
		builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<TeamListView>();
        builder.Services.AddTransient<RaceListViewModel>();

		builder.Services.AddTransient<CreateOrEditTeamView>();
		builder.Services.AddTransient<CreateOrEditTeamViewModel>();


        builder.Services.AddTransient<JudgeListView>();
        builder.Services.AddTransient<JudgeListViewModel>();

        builder.Services.AddTransient<CreateOrEditJudgeView>();
        builder.Services.AddTransient<CreateOrEditJudgeViewModel>();

         //builder.Services.AddTransient<RaceListView>();
         //builder.Services.AddTransient<RaceListViewModel>();

        builder.Services.AddTransient<CreateOrEditRaceViewModel>();
        builder.Services.AddTransient<CreateOrEditRaceView>();

        builder.Services.AddScoped<IGoogleDriveService, GoogleDriveService> ();
        builder.Services.AddScoped<ITeamService, TeamService> ();
        builder.Services.AddScoped<IParticipantService, ParticipantService> ();
        builder.Services.AddScoped<IRaceService, RaceService> ();
        builder.Services.AddScoped<IJudgeService, JudgeService> ();

        return builder;
	}
}

namespace ChatApp.Configurations;

public static class BuilderConfigurations
{
	public static void AddConfigurations(this IServiceCollection services)
		=> AddCors(services);

	private static IServiceCollection AddCors(IServiceCollection services)
		=> services.AddCors(options =>
			options.AddDefaultPolicy(builder =>
			{
				builder.WithOrigins("http://localhost:3000")
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowCredentials();
			}));
}

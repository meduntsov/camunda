using Camunda.Worker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WorkflowMvp.Api.Auth;
using WorkflowMvp.Api.Camunda;
using WorkflowMvp.Api.Data;
using WorkflowMvp.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CamundaOptions>(builder.Configuration.GetSection(CamundaOptions.SectionName));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddSingleton(provider =>
{
    var address = builder.Configuration["Camunda:GatewayAddress"] ?? "zeebe:26500";
    return ZeebeClient.Builder()
        .UseGatewayAddress(address)
        .UsePlainText()
        .Build();
});

builder.Services.AddHttpClient("OpenClaw", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["OpenClaw:BaseUrl"] ?? "http://openclaw:8081");
});

builder.Services.AddScoped<IOpenClawAssistantService>(provider =>
{
    var factory = provider.GetRequiredService<IHttpClientFactory>();
    var logger = provider.GetRequiredService<ILogger<OpenClaw.OpenClawAssistantService>>();
    return new OpenClaw.OpenClawAssistantService(factory.CreateClient("OpenClaw"), logger);
});

builder.Services.AddScoped<ICamundaWorkflowClient, CamundaWorkflowClient>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = "roles",
            NameClaimType = "preferred_username"
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

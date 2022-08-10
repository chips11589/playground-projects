using Asos.AspNetCore.Api.Authentication.Middlewares;
using Asos.AspNetCore.Api.Authentication.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    )
    .AddJwtBearer(
        options =>
        {
            options.RequireSsl = false;
            options.AuthorizationLockdownEnabled = true;
            options.SendDetailedErrorMessages = true;
            options.Issuers.Add("https://sts.windows.net/73695d2a-8a88-4cb4-9e96-332954866ca0/"); // tenant id
            options.Audiences.Add("api://a9e89fbc-3875-4c0c-add0-058a230f6dd9"); // client id
            options.MetadataAddresses.Add(new Uri("https://login.microsoftonline.com/common/.well-known/openid-configuration"));
            options.WhitelistedServiceIds.AddRange(new string[] { "6543e42e-e44c-4e00-b1fd-de5af396ce31" }); // app object id
        }
    );

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

builder.Services.ConfigureAsosSecurityPolicies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

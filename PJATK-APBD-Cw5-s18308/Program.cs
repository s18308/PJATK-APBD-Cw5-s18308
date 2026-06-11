using Microsoft.EntityFrameworkCore;
using PJATK_APBD_Cw5_s18308.Infrastructure;
using PJATK_APBD_Cw5_s18308.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opt => opt.SuppressMapClientErrors = true);

builder.Services.AddOpenApi();

builder.Services.AddScoped<IDbService, DbService>();

builder.Services.AddDbContext<DatabaseContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
using Microsoft.EntityFrameworkCore;
using XFundingHub.Application.Repositories;
using XFundingHub.Application.Services;
using XFundingHub.Infrastructure.Data;
using XFundingHub.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<XFundingHubDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<
    ILoanApplicationRepository,
    LoanApplicationRepository>();

builder.Services.AddSingleton<ApplicationIdGenerator>();

builder.Services.AddScoped<LoanApplicationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
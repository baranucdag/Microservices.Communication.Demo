using System;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Order.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderAPIDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("Npgsql"));
    }
    );

builder.Services.AddMassTransit((config) =>
{
    config.UsingRabbitMq((context, _configurator)=>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


using DynamicTranslator.IoC;
using Translator.Localization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDynamicTranslator(options =>
{
    options.ResourceType = typeof(Resources);
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GenericTranslationFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//app.UseDynamicTranslator<Resources>();

app.MapControllers();

app.Run();

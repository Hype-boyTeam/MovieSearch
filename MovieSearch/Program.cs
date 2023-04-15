using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.EntityFrameworkCore;
using MovieSearch.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Elasticsearch 설정
var elasticSettings =
    new ElasticsearchClientSettings(new Uri(builder.Configuration.GetConnectionString("ElasticHost")!));
if (builder.Environment.IsProduction())
{
    // Release mode일 때는 추가로 계정 비밀번호를 사용
    elasticSettings = elasticSettings
        .Authentication(
            new BasicAuthentication(
                builder.Configuration.GetConnectionString("ElasticUser")!,
                builder.Configuration.GetConnectionString("ElasticPassword")!
            )
        );
}
var elastic = new ElasticsearchClient(elasticSettings);
builder.Services.AddSingleton(elastic);

// DB 설정
builder.Services.AddDbContext<MovieDb>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.UseDeveloperExceptionPage();    
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MovieDb>();
    
    if (app.Environment.IsDevelopment())
    {
        dbContext.Database.EnsureCreated();
    }

    if (app.Environment.IsProduction())
    {
        dbContext.Database.Migrate();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

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
    
    // Controller 실행 도중 잡지 못한 예외가 있다면 웹 페이지에 바로 오류 메시지가 노출되도록 설정 
    app.UseDeveloperExceptionPage();
    
    // DB 초기화
    app.UseMigrationsEndPoint();
    
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MovieDb>();
    dbContext.Database.EnsureCreated();
    
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

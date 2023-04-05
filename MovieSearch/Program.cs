using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

var builder = WebApplication.CreateBuilder(args);

// Elasticsearch 설정
var elasticSettings =
    new ElasticsearchClientSettings(new Uri(builder.Configuration.GetConnectionString("ElasticHost")!))
        .ServerCertificateValidationCallback((_, _, _, _) => true)
        .Authentication(
            new BasicAuthentication(
                builder.Configuration.GetConnectionString("ElasticUser")!,
                builder.Configuration.GetConnectionString("ElasticPassword")!
            )
        );
var elastic = new ElasticsearchClient(elasticSettings);

// Add services to the container.
builder.Services.AddSingleton(elastic);
    
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

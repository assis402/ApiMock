using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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


// Função que lê e retorna o conteúdo do response.json
static async Task HandleRequest(HttpContext context)
{
    var filePath = Path.Combine(AppContext.BaseDirectory, "response.json");

    if (!File.Exists(filePath))
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync("Arquivo response.json não encontrado.");
        return;
    }

    var json = await File.ReadAllTextAsync(filePath);
    context.Response.ContentType = "application/json; charset=utf-8";
    await context.Response.WriteAsync(json);
}

// Rota raiz "/"
app.MapMethods("/", ["GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS", "HEAD"], HandleRequest);

// Qualquer outra rota
app.MapMethods("/{**catchAll}", ["GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS", "HEAD"], HandleRequest);

await app.RunAsync();
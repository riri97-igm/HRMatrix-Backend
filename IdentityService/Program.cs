var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connect to SQL Server 
builder.Services.AddDbContext<AppDbContext>(option =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Setup JWT authentication 
var JwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = trues,
            ValidIssuer = builder.Configuration["Jwt:Issuer"]
            ValidAudience = builder.Configuration["Jwt:Audience"]
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(JwtKey))
        };
    });

builder.Services.AddAuthorization();

//Register JwtService so controller can use it 
builder.Services.AddScoped<IJwtService, JwtService>();

//Allow React frontend to call this API
builder.Services.AddCors(option =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

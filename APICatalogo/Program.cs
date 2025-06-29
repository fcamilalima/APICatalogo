using APICatalogo.Context;
using APICatalogo.Extensions;
using APICatalogo.Filters;
using APICatalogo.Logging;
using APICatalogo.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using APICatalogo.RateLimitOptions;
using Asp.Versioning;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = new UrlSegmentApiVersionReader();
    //o.ApiVersionReader = ApiVersionReader.Combine(
    //    new QueryStringApiVersionReader(),
    //    new UrlSegmentApiVersionReader()
    //);
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add(typeof(ApiExceptionFilter));
    })
    .AddJsonOptions(
        options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
    )
    .AddNewtonsoftJson();
builder.Services.AddCors(options =>
{
    options.AddPolicy("OrigensComAcessoPermitido",
        policy =>
        {
            policy.WithOrigins("https://localhost:7185")
            .WithMethods("GET", "POST")
            .AllowAnyHeader();
        });
});
builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<ApiExceptionFilter>();
builder.Services.AddScoped<ICategoriasRepository, CategoriasRepository>();
builder.Services.AddScoped<IProdutosRepository, ProdutosRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "APICatalogo",
        Version = "v1",
        Description = "Catálogo de Produtps e Categorias",
        TermsOfService = new Uri("https://examplo.com/termos"),
        Contact = new OpenApiContact
        {
            Name = "Camila Lima",
            Email = "fcamilalima@gmail.com",
            Url = new Uri("https://examplo.com/meu_site")
        }, License = new OpenApiLicense
        {
            Name = "Licença de Uso",
            Url = new Uri("https://examplo.com/licenca")
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT "
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));

});



builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

string? mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnection,
    ServerVersion.AutoDetect(mySqlConnection)));

var secretKey = builder.Configuration["JWT:SecretKey"]
    ?? throw new ArgumentException("Invalid secret key!");
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));
    //options.AddPolicy("SuperAdminOnly", policy =>
    //    policy.RequireRole("SuperAdminOnly").RequireClaim("id", "superusuario"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("Users"));
    options.AddPolicy("ExclusiveOnly", policy =>
    {
        policy.RequireAssertion(context => context.User.HasClaim(claim =>
            claim.Type == "id" && (claim.Value == "superusuario") ||
            context.User.IsInRole("SuperAdmin")));
    });
});

var myOptions = new MyRateLimitOptions();

builder.Configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter(policyName: "fixedwindow", options =>
    {
        options.PermitLimit = myOptions.PermitLimit;
        options.Window = TimeSpan.FromSeconds(myOptions.Window);
        options.QueueLimit = myOptions.QueueLimit;
        options.QueueProcessingOrder = QueueProcessingOrder.NewestFirst;
    });
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        httpcontext => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpcontext.User.Identity?.Name ??
            httpcontext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 2,
                Window = TimeSpan.FromSeconds(10),
                QueueLimit = 0
            }
        ));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
    };
});

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.ConfigureExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "APICatalogo");
    });
    app.ConfigureExceptionHandler();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "APICatalogo");
        options.InjectStylesheet("/swaggercustom.css");
        options.RoutePrefix = string.Empty;
    });
    app.UseHsts();
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

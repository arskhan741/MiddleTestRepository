using ArsalanAssesment.Web.Configurations;
using ArsalanAssesment.Web.Data;
using ArsalanAssesment.Web.Repository;
using ArsalanAssesment.Web.Repository.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add SQL Connection
builder.Services.AddDbContext<ApplicationDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnectionOfc"));
});

//Register for swagger Controller
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    //Add Security Defination
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' followed by a space of your token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    //Add Security Requirment
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                Array.Empty<String>()
            }
        });
});



// For Identity  
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();


// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddTransient<ISaleRepository, SaleRepository>();
builder.Services.AddTransient<IDashBoardMetricsRepository, DashBoardMetricsRepository>();
builder.Services.AddTransient<IUserAuthRepository, UserAuthRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//For Authentication and JWT Token
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Key").Value!))
    };

});
//End settings for auth

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy =>
        {
            policy.WithOrigins("http://127.0.0.1:5500") // Add your front-end URL here
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Seed the admin user and role
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedAdminUserAsync(userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


// Method to seed the admin user and role
async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
{
    const string adminEmail = "admin@domain.com";
    const string adminUsername = "HeadAdmin";
    const string adminPassword = "Nesl@admin123";
    const string adminRole = "admin";

    // Ensure the admin role exists
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // Check if the admin user exists
    var adminUser = await userManager.FindByNameAsync(adminUsername);

    if (adminUser == null)
    {
        // Create the admin user
        adminUser = new IdentityUser
        {
            UserName = adminUsername,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);

        if (createUserResult.Succeeded)
        {
            // Assign the admin role to the user
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
        else
        {
            // Handle any errors during user creation
            throw new Exception("Failed to create the admin user");
        }
    }
}




// I DID SOME WORK ON MAIN and will push it to middle/Orp_Branch

// testing 123
// testing 1234
// testing 1235


// -     - - - - - - - - - -- - - - - -  - - -

// testing commit 2    '''''
// testing commit 2 '''''
// testing commit 2     ''''''
// testing commit 2         '''''''''''


// -     - - - - - - - - - -- - - - - -  - - -

// testing commit 3    '''''
// testing commit 3 '''''
// testing commit 3     ''''''
// testing commit 3         '''''''''''
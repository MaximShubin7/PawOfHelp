// Program.cs
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PawOfHelp.Data;
using PawOfHelp.DTOs.Auth;
using PawOfHelp.DTOs.Comment;
using PawOfHelp.DTOs.User;
using PawOfHelp.Services;
using PawOfHelp.Services.Interfaces;
using PawOfHelp.Validators.Auth;
using PawOfHelp.Validators.Comment;
using PawOfHelp.Validators.User;
using Resend;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<ConfirmEmailRequestDto>, ConfirmEmailRequestValidator>();
builder.Services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
builder.Services.AddScoped<IValidator<ResetPasswordWithCodeRequestDto>, ResetPasswordValidator>();
builder.Services.AddScoped<IValidator<ChangePasswordDto>, ChangePasswordValidator>();
builder.Services.AddScoped<IValidator<CreateCommentDto>, CreateCommentValidator>();
builder.Services.AddScoped<IValidator<UpdateCommentDto>, UpdateCommentValidator>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IImageKitService, ImageKitService>();
builder.Services.AddHttpClient<IImageKitService, ImageKitService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IPublicUserService, PublicUserService>();

builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(o => o.ApiToken = builder.Configuration["Resend:ApiToken"]);
builder.Services.AddTransient<IResend, ResendClient>();

var jwtKey = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
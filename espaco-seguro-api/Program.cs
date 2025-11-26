using System.Text;
using espaco_seguro_api._2___Application.Interfaces.Auth;
using espaco_seguro_api._2___Application.Interfaces.Postagem;
using espaco_seguro_api._2___Application.JwtSettings;
using espaco_seguro_api._2___Application.ServiceApp;
using espaco_seguro_api._2___Application.ServiceApp.IServiceApp;
using espaco_seguro_api._2___Application.ServiceApp.IServiceApp.ComentarioPostagem;
using espaco_seguro_api._2___Application.ServiceApp.IServiceApp.Chat;
using espaco_seguro_api._3___Domain.Auth;
using espaco_seguro_api._3___Domain.Entities;
using espaco_seguro_api._3___Domain.Exceptions;
using espaco_seguro_api._3___Domain.Interfaces.Repositories;
using espaco_seguro_api._3___Domain.Interfaces.Services;
using espaco_seguro_api._3___Domain.Interfaces.Services.Chat;
using espaco_seguro_api._3___Domain.Security;
using espaco_seguro_api._3___Domain.Services;
using espaco_seguro_api._3___Domain.Services.Chat;
using espaco_seguro_api._3___Domain.Services.Postagem.ComentarioPostagem;
using espaco_seguro_api._3___Domain.Services.Security;
using espaco_seguro_api._4___Data;
using espaco_seguro_api._4___Data.Helpers;
using espaco_seguro_api._4___Data.Repositories;
using espaco_seguro_api._5___Infra.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// =====================
// 🔥 Swagger
// =====================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =====================
// 🔥 Banco
// =====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConexaoPadrao")));

// =====================
// 🔥 JWT
// =====================
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSettings = new JwtSettings();
jwtSection.Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false; // IMPORTANTE NO DOCKER
    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = key
    };
});

// =====================
// 🔥 CORS — para Flutter
// =====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p
        .AllowAnyOrigin()    // app mobile
        .AllowAnyMethod()
        .AllowAnyHeader()
    );
});

// =====================
// 🔥 Injeta serviços
// =====================
// (deixo aqui igual ao seu, sem alterar)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioServiceApp, UsuarioServiceApp>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ICardServiceApp, CardServiceApp>();
builder.Services.AddScoped<IPostagemRepository, PostagemRepository>();
builder.Services.AddScoped<IPostagemService, PostagemService>();
builder.Services.AddScoped<IPostagemServiceApp, PostagemServiceApp>();
builder.Services.AddScoped<ICurtidaPostagemRepository, CurtidaPostagemRepository>();
builder.Services.AddScoped<ICurtidaPostagemService, CurtidaPostagemService>();
builder.Services.AddScoped<ICurtidaPostagemServiceApp, CurtidaPostagemServiceApp>();
builder.Services.AddScoped<IComentarioPostagemRepository, ComentarioPostagemRepository>();
builder.Services.AddScoped<IComentarioPostagemService, ComentarioPostagemService>();
builder.Services.AddScoped<IComentarioPostagemServiceApp, ComentarioPostagemServiceApp>();
builder.Services.AddScoped<ISessaoChatRepository, SessaoChatRepository>();
builder.Services.AddScoped<IMensagemChatRepository, MensagemChatRepository>();
builder.Services.AddScoped<ISessaoChatService, SessaoChatService>();
builder.Services.AddScoped<IMensagemChatService, MensagemChatService>();
builder.Services.AddScoped<ISessaoChatServiceApp, SessaoChatServiceApp>();
builder.Services.AddScoped<IMensagemChatServiceApp, MensagemChatServiceApp>();
builder.Services.AddScoped<IFabricadordeToken, FabricadorDeToken>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ILoginServiceApp, LoginServiceApp>();
builder.Services.AddScoped<IPasswordHasher, BcryptPaswordHasher>();
builder.Services.AddScoped<Helpers>();

// =====================
// 🔥 Porta Render
// =====================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

var app = builder.Build();

// =====================
// 🔥 Migrations automáticas
// =====================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// =====================
// 🔥 Middlewares
// =====================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ⚠️ REMOVIDO — causa crash no Render
// app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// =====================
// 🔥 START
// =====================
app.Run();

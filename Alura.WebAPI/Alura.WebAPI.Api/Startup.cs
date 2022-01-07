using Alura.ListaLeitura.Api.Formatters;
using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WebAPI.Api.Filtros;
using Alura.WebAPI.Api.HttpClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Alura.WebAPI.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LeituraContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ListaLeitura"));
            });

            services.AddTransient<IRepository<Livro>, RepositorioBaseEF<Livro>>();

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            }).AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("alura-webapi-authentication-valid")),
                    ClockSkew = TimeSpan.FromMinutes(30),
                    ValidIssuer = "Alura.WebApp",
                    ValidAudience = "Postman",
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AluraWebAPI", Version = "1.0" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "AluraWebAPI", Version = "2.0" });
                //Esse codigo foi adicionado para resolver ações conflitantes que é quando ações estão
                //usando a mesma rota
                //c.ResolveConflictingActions(x => x.First());
                c.EnableAnnotations();
                //documentando a autorização
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    //Onde o token sera enviado
                    In = ParameterLocation.Header,
                    //Tipo de mecanismo
                    Description = "Autenticação Bearer via JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            services.AddHttpClient<AuthApiClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5000/api/");
            });

            services.AddMvc(options =>
            {
                //Adicionando formatos personalizados ao pipeline
                options.OutputFormatters.Add(new LivroCsvFormatter());
                //Informar que estou usando esse filtro de exceções
                options.Filters.Add(typeof(ErrorResponseFilter));
            }).AddXmlSerializerFormatters()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            //Mudando a configuração padrão da api do .NET a fim dela não fazer a validação automática do ModelState
            //Dessa forma conseguimos utilizar a nossa própria validação do ModelState
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //Preciso adicionar o suporte de versão da api no midleware da aplicação
            //services.AddApiVersioning(options =>
            //{
            //    Informar qual o leitor da versão
            //    Aqui informo que o leitor vai ser o cabeçalho que vai ler a chave api - version
            //    options.ApiVersionReader = ApiVersionReader.Combine
            //        (
            //            //Classes que vao ler a versao
            //            new QueryStringApiVersionReader("api-version"), //query string
            //            new HeaderApiVersionReader("api-version") //header
            //        );
            //});

            services.AddApiVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            //Ativa o Swagger
            app.UseSwagger();

            // Ativa o Swagger UI
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Versão 1.0");
                opt.SwaggerEndpoint("/swagger/v2/swagger.json", "Versão 2.0");

            });

            app.UseStaticFiles();

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                     name: "default",
                     pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

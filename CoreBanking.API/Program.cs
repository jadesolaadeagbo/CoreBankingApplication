using CoreBanking.API.gRPC.Services;
using CoreBanking.API.Middleware;
using CoreBanking.App.Common.Mappings;
using CoreBanking.Application.Accounts.Commands.CreateAccount;
using CoreBanking.Application.Accounts.EventHandlers;
using CoreBanking.Application.Common.Behaviours;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Mappings;
using CoreBanking.Application.Customers.Commands.CreateCustomer;
using CoreBanking.Application.Customers.Queries.GetCustomerDetails;
using CoreBanking.Core.Events;
using CoreBanking.Core.Interfaces;
using CoreBanking.DataAccessLayer.Data;
using CoreBanking.DataAccessLayer.Repositories;
using CoreBanking.DataAccessLayer.Services;
using CoreBanking.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<BankingDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Add Application and Infrastructure Services
        //builder.Services.AddApplicationServices();
        //builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.WebHost.ConfigureKestrel(options =>
        {
            // HTTP (for Swagger, REST, etc.)
            options.ListenLocalhost(5037, o =>
            {
                o.Protocols = HttpProtocols.Http1;
            });

            // HTTPS (for gRPC, requires HTTP/2)
            options.ListenLocalhost(7288, o =>
            {
                o.UseHttps(); // uses developer cert
                o.Protocols = HttpProtocols.Http2;
            });
        });


        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddControllers();

        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        builder.Services.AddTransient<INotificationHandler<AccountCreatedEvent>, AccountCreatedEventHandler>();
        builder.Services.AddTransient<INotificationHandler<MoneyTransferredEvent>, MoneyTransferredEventHandler>();
        builder.Services.AddTransient<INotificationHandler<InsufficientFundsEvent>, InsufficientFundsEventHandler>();

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainEventsBehaviour<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

        // Add gRPC services to the container.
        builder.Services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
            //options.Interceptors.Add<ExceptionInterceptor>();
        });
        builder.Services.AddGrpcReflection();

        // Add MediatR with behaviours
        builder.Services.AddMediatR(cfg =>
        {
            // Note: Registering one command is enough per Layer—MediatR scans the entire Application assembly (all Commands & Queries).
            cfg.RegisterServicesFromAssembly(typeof(CreateAccountCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(CreateCustomerCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(GetCustomerDetailsQuery).Assembly);


            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            cfg.AddOpenBehavior(typeof(DomainEventsBehaviour<,>));

            cfg.Lifetime = ServiceLifetime.Scoped;
        });

        builder.Services.AddValidatorsFromAssembly(typeof(CreateAccountCommandValidator).Assembly);

        builder.Services.AddAutoMapper(cfg => { },
typeof(AccountProfile).Assembly);

        builder.Services.AddAutoMapper(cfg => { }, typeof(AccountGrpcProfile).Assembly);


        // Register outbox and Background services
        builder.Services.AddScoped<IOutboxMessageProcessor, OutboxMessageProcessor>();
        builder.Services.AddHostedService<OutboxBackgroundService>();

        // Add controllers and swagger
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Enriched swaggerGen with XML comments and authentication
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CoreBanking API",
                Version = "v1",
                Description = "A modern banking API built with Clean Architecture, DDD and CQRS",
                Contact = new OpenApiContact
                {
                    Name = "CoreBanking Team",
                    Email = "support@corebanking.com"
                }
            });

            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            // Add authentication support in Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(options => options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0);

            // Enriched Swagger UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoreBanking API v1");
                c.RoutePrefix = "swagger"; // Access at /swagger
                c.DocumentTitle = "CoreBanking API Documentation";
                c.EnableDeepLinking();
                c.DisplayOperationId();
            });
        }



        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();


        app.MapControllers();

        //Use grpc Endpoints
        app.MapGrpcService<AccountGrpcService>();
        app.MapGet("/", () => "CoreBanking API is running. Use /swagger for REST or a gRPC client for gRPC calls.");
        if (app.Environment.IsDevelopment())
        {
            app.MapGrpcReflectionService();
        }


        app.Run();
    }
}
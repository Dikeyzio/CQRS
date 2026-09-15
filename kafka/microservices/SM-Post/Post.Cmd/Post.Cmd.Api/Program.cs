using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infostructure;
using CQRS.Core.Prodicers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Post.Cmd.Api.Command;
using Post.Cmd.Api.Produsers;
using Post.Cmd.Api.Stores;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infostructure.Config;
using Post.Cmd.Infostructure.Dispatchers;
using Post.Cmd.Infostructure.Handlers;
using Post.Cmd.Infostructure.Repositories;
using Post.Common.Events;

var builder = WebApplication.CreateBuilder(args);
BsonClassMap.RegisterClassMap<BaseEvent>();
BsonClassMap.RegisterClassMap<PostCreatedEvent>();
BsonClassMap.RegisterClassMap<MessageUpdateEvent>();
BsonClassMap.RegisterClassMap<PostLikeEvent>();
BsonClassMap.RegisterClassMap<CommentAddedEvent>();
BsonClassMap.RegisterClassMap<CommentRemoveEvent>();
BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
var path = builder.Configuration.GetSection(nameof(MongoDbConfig));
builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
// Add services to the container.
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<ICommandHandler, CommandHandler>();
builder.Services.AddScoped<IEventProducer, EventProducer>();

builder.Services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//register command handler methods
var commandHandler = builder.Services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
var dispatcher = new CommandDispatcher();
dispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<EditMessageCommand>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
dispatcher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);
builder.Services.AddSingleton<ICommandDispatcher, CommandDispatcher>(_ => dispatcher);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();


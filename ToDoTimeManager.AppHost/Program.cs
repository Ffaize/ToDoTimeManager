var builder = DistributedApplication.CreateBuilder(args);

var dbPublish = builder.AddProject<Projects.ToDoTimeManager_DbPublisher>("db-publish");

var api = builder.AddProject<Projects.ToDoTimeManager_WebApi>("webapi")
    .WaitForCompletion(dbPublish);

builder.AddProject<Projects.ToDoTimeManager_WebUI>("webui")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();

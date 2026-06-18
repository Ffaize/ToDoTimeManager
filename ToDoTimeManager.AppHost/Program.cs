var builder = DistributedApplication.CreateBuilder(args);

var dbPublish = builder.AddProject<Projects.ToDoTimeManager_DbPublisher>("db-publish")
    .WithExplicitStart();

var api = builder.AddProject<Projects.ToDoTimeManager_WebApi>("webapi")
    .WithUrlForEndpoint("https", url => url.DisplayText = "API (Swagger)");

builder.AddProject<Projects.ToDoTimeManager_WebUI>("webui")
    .WithReference(api)
    .WaitFor(api)
    .WithUrlForEndpoint("https", url => url.DisplayText = "Web UI");

builder.Build().Run();

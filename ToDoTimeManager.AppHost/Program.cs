var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var blobs = storage.AddBlobs("blobs");

var dbPublish = builder.AddProject<Projects.ToDoTimeManager_DbPublisher>("db-publish")
    .WithExplicitStart();

var api = builder.AddProject<Projects.ToDoTimeManager_WebApi>("webapi")
    .WithReference(blobs)
    .WaitFor(blobs)
    .WithUrlForEndpoint("https", url => url.DisplayText = "API (Swagger)");

builder.AddProject<Projects.ToDoTimeManager_WebUI>("webui")
    .WithReference(api)
    .WaitFor(api)
    .WithUrlForEndpoint("https", url => url.DisplayText = "Web UI");

builder.Build().Run();

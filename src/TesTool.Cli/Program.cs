using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Linq;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            using var serviceProvider = new ServiceCollection()
                .AddServices()
                .BuildServiceProvider();

            var argumentsCoreService = serviceProvider.GetService<ICommandFactoryService>();

            var console = serviceProvider.GetService<ILoggerInfraService>();
            var jsonOptions = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var project = serviceProvider.GetService<IWebApiScanInfraService>();

            var model = project.GetModelAsync("PersonResponse").Result;
            console.LogInformation(JsonConvert.SerializeObject(model, Formatting.Indented, jsonOptions));

            /*
            project.GetControllersAsync().Result.ToList().ForEach(c =>
            {
                console.LogInformation("\n" + (c.Authorize ? "[Authorize] " : "[AllowAnonymous] ") + c.Route);
                c.Endpoints.ToList().ForEach(e =>
                {
                    console.LogInformation($" {(e.Authorize ? "[Authorize] " : "[AllowAnonymous] ")}[{e.Method.Name}] {e.Route}");
                    //console.LogInformation($" [{e.Method.Name}] {e.Route} ===========================================");
                    e.Inputs.ToList().ForEach(i =>
                    {
                        console.LogInformation($"   [{i.Source.Name}] {i.Name}");
                        //console.LogInformation($"   [{i.Source.Name}] {i.Name} PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP");
                        //console.LogInformation(JsonConvert.SerializeObject(i, Formatting.Indented, jsonOptions));
                    });
                    //console.LogInformation($" [{e.Method.Name}] {e.Route} RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR");
                    console.LogInformation(JsonConvert.SerializeObject(e.Output, Formatting.Indented, jsonOptions));
                });
            });
            */

            argumentsCoreService.CreateCommand(args)?.ExecuteAsync().Wait();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync(args).Wait();
        }

        static async Task RunAsync(string[] args)
        {
            try
            {
                using var serviceProvider = new ServiceCollection()
                    .AddServices()
                    .BuildServiceProvider();

                var argumentsCoreService = serviceProvider.GetService<ICommandFactoryService>();
                await argumentsCoreService.CreateCommand(args)?.ExecuteAsync();

                var console = serviceProvider.GetService<ILoggerInfraService>();
                var jsonOptions = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

                /*
                var project = serviceProvider.GetService<IWebApiScanInfraService>();
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
                        //console.LogInformation(JsonConvert.SerializeObject(e.Output, Formatting.Indented, jsonOptions));
                    });
                });
                */

                //var templateService = serviceProvider.GetService<ITemplateCodeInfraService>();
            }
            catch (TesToolExceptionBase exception)
            {
                Console.WriteLine(exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Interfaces.Services;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Metadata.Types;
using TesTool.Infra.Extensions;

namespace TesTool.Infra.Services
{
    public class WebApiDbContextInfraService : WebApiScanInfraService, IWebApiDbContextInfraService
    {
        public WebApiDbContextInfraService(
            ILoggerInfraService loggerInfraService, 
            ISettingInfraService settingInfraService
        ) : base(loggerInfraService, settingInfraService)
        { }

        public async Task<bool> IsDbContextClassAsync(string className)
        {
            var project = await GetProjectAsync();
            if (project is null) return default;

            var projects = new[] { project }.Concat(GetProjectReferences(project)).ToArray();
            var classes = await GetClassesAsync(projects);
            var @class = classes.FirstOrDefault(c => c.Declaration.Identifier.Text == className);
            if (@class is null) return false;

            return @class.TypeSymbol.ImplementsClass("DbContext", "Microsoft.EntityFrameworkCore");
        }

        public async Task<bool> IsDbSetClassAsync(string dbContext, string className)
        {
            var dbContextClass = await GetModelAsync(dbContext) as Class;
            return dbContextClass.Properties.Any(p => p.Name == className && 
                p.Type is Array array && array.Name == "DbSet" &&
                array.Namespace == "Microsoft.EntityFrameworkCore"
            );
        }

        public async Task<IEnumerable<DbSet>> GetDbSetsAsync(string dbContext)
        {
            var dbContextClass = await GetModelAsync(dbContext) as Class;
            return dbContextClass.Properties
                .Where(p => p.Type is Array)
                .Select(p => new { p.Name, Type = p.Type as Array })
                .Where(p => p.Type.Name == "DbSet" && p.Type.Namespace == "Microsoft.EntityFrameworkCore")
                .Where(p => p.Type.Type is Class)
                .Select(p => new DbSet(p.Type.Type as Class, p.Name))
                .ToArray();
        }

        public async Task<IEnumerable<Class>> GetDbContextClassesAsync()
        {
            var project = await GetProjectAsync();
            if (project is null) return default;

            var projects = new[] { project }.Concat(GetProjectReferences(project)).ToArray();
            var classes = await GetClassesAsync(projects);
            return classes.Where(c => c.TypeSymbol.ImplementsClass("DbContext", "Microsoft.EntityFrameworkCore"))
                .Select(c => GetModelType(c.TypeSymbol) as Class);
        }
    }
}

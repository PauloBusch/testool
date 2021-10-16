using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TesTool.Core.Attributes;
using TesTool.Core.Enumerations;
using TesTool.Core.Exceptions;
using TesTool.Core.Interfaces.Services;

namespace TesTool.Core.Commands.Generate
{
    [Command("controller", Order = 2, HelpText = "Gerar código de teste a partir de controlador.")]
    public class GenerateControllerCommand : GenerateCommandBase
    {
        [Parameter(HelpText = "Nome da classe controlador.")]
        public string Controller { get; set; }

        [Flag(HelpText = "Habilita modo estático de geração de código.")]
        public bool Static { get; set; }

        private readonly ITestScanInfraService _testScanInfraService;
        private readonly ITemplateCodeInfraService _templateCodeInfraService;
        private readonly IWebApiScanInfraService _webApiScanInfraService;
        private readonly IFileSystemInfraService _fileSystemInfraService;

        public GenerateControllerCommand(
            ITestScanInfraService testScanInfraService,
            ITemplateCodeInfraService templateCodeInfraService,
            IWebApiScanInfraService webApiScanInfraService,
            IEnvironmentInfraService environmentInfraService,
            IFileSystemInfraService fileSystemInfraService
        ) : base(environmentInfraService) 
        {
            _testScanInfraService = testScanInfraService;
            _templateCodeInfraService = templateCodeInfraService;
            _webApiScanInfraService = webApiScanInfraService;
            _fileSystemInfraService = fileSystemInfraService;
        }

        public override async Task ExecuteAsync()
        {
            var controllerClass = await _webApiScanInfraService.GetControllerAsync(GetControllerName());
            if (controllerClass is null) throw new ClassNotFoundException(GetControllerName());
            if (!controllerClass.Endpoints.Any()) throw new ValidationException("The controller has no endpoints.");

            var helpClassesRegex = HelpClassEnumerator.GetAll().Select(h => h.Regex).ToArray();
            var regexClassesNotFounded = await _testScanInfraService.GetNotFoundClassesAsync(helpClassesRegex);
            if (regexClassesNotFounded.Any())
            {
                var classNames = HelpClassEnumerator.GetAll().Where(h => regexClassesNotFounded.Any(r => r == h.Regex));
                throw new UnsupportedCommandException($"O projeto não possui as classes necessárias: {string.Join(", ", classNames)}.");
            }
        }

        private string GetControllerName() => Controller.Contains("Controller") ? Controller : $"{Controller}Controller";
    }
}

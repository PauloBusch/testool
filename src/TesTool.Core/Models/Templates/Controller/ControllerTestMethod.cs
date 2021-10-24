using System.Collections.Generic;
using TesTool.Core.Models.Metadata;
using TesTool.Core.Models.Templates.Faker;

namespace TesTool.Core.Models.Templates.Controller
{
    public class ControllerTestMethod
    {
        private readonly List<FakerDeclaration> _entities;
        private readonly List<FakerDeclaration> _models;

        public ControllerTestMethod(string name, RequestCall request)
        {
            Name = name;
            Request = request;
            _entities = new List<FakerDeclaration>();
            _models = new List<FakerDeclaration>();
        }

        public bool Unsafe { get; private set; }
        public string Name { get; private set; }
        public RequestCall Request { get; private set; }
        public IReadOnlyCollection<FakerDeclaration> Entities => _entities.AsReadOnly();
        public IReadOnlyCollection<FakerDeclaration> Models => _models.AsReadOnly();

        public void AddEntity(FakerDeclaration entity) => _entities.Add(entity);
        public void AddModel(FakerDeclaration model) => _models.Add(model);
        public void AddModels(IEnumerable<FakerDeclaration> models) => _models.AddRange(models);
        public void MarkAsUnsafe() => Unsafe = true;
    }
}

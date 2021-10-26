namespace TesTool.Core.Models.Templates.Controller
{
    public class ControllerTestMethodSectionAct
    {
        public ControllerTestMethodSectionAct(
            string route, 
            string method, 
            string returnType, 
            string bodyModel, 
            string queryModel
        )
        {
            Route = route;
            Method = method;
            ReturnType = returnType;
            BodyModel = bodyModel;
            QueryModel = queryModel;
        }

        public bool Unsafe { get; private set; }
        public string Route { get; private set; }
        public string Method { get; private set; }
        public string ReturnType { get; private set; }
        public string BodyModel { get; private set; }
        public string QueryModel { get; private set; }
        public void MarkAsUnsafe() => Unsafe = true;
    }
}

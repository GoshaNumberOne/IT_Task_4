using System.Reflection;

namespace ChessApp.ViewModels
{
    public class MethodDescription
    {
        public string Name { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public string DisplayName { get; private set; }

        public MethodDescription(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            Name = methodInfo.Name;
            var parameters = string.Join(", ", methodInfo.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
            DisplayName = $"{methodInfo.ReturnType.Name} {Name}({parameters})";
        }

        public override string ToString() => DisplayName;
    }
}

namespace ChessApp.ViewModels
{
    public class TypeDescription
    {
        public string Name { get; set; }
        public Type Type { get; set; }

        public override string ToString() => Name;
    }
}
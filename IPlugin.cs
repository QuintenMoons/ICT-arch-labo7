namespace PluginContracts
{
    public interface IPlugin
    {
        string Name { get; }
        BerekendeCel Execute(List<string> expressiesKinderen, List<BerekendeCel> resultatenKinderen);
    }
}
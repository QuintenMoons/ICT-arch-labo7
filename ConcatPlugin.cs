using PluginContracts;

public class ConcatPlugin : IPlugin
{
    public string Name => "concat";

    public BerekendeCel Execute(List<string> expressiesKinderen, List<BerekendeCel> resultatenKinderen)
    {
        string resultaat = "";
        for (int i = 0; i < resultatenKinderen.Count; i++)
        {
            if (resultatenKinderen[i].CelType != CelType.STRING)
            {
                return new BerekendeCel($"Fout: resultaat voor {expressiesKinderen[i]} is geen string.", CelType.ERROR);
            }
            resultaat += resultatenKinderen[i].VoorstellingWaarde;
        }
        return new BerekendeCel(resultaat, CelType.STRING);
    }
}
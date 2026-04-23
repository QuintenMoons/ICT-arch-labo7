using PluginContracts;

public class UpperCasePlugin : IPlugin
{
    public string Name => "upperCase";

    public BerekendeCel Execute(List<string> expressiesKinderen, List<BerekendeCel> resultatenKinderen)
    {
        if (resultatenKinderen.Count != 1)
        {
            return new BerekendeCel($"Fout: upperCase verwacht exact 1 argument, gekregen: {resultatenKinderen.Count}", CelType.ERROR);
        }

        if (resultatenKinderen[0].CelType != CelType.STRING)
        {
            return new BerekendeCel($"Fout: resultaat voor {expressiesKinderen[0]} is geen string.", CelType.ERROR);
        }

        return new BerekendeCel(resultatenKinderen[0].VoorstellingWaarde.ToUpper(), CelType.STRING);
    }
}
using PluginContracts;

public class MinPlugin : IPlugin
{
    public string Name => "min";

    public BerekendeCel Execute(List<string> expressiesKinderen, List<BerekendeCel> resultatenKinderen)
    {
        if (resultatenKinderen.Count < 1)
        {
            return new BerekendeCel("Fout: min-operatie vereist minstens 1 operand.", CelType.ERROR);
        }

        // Controleer of het eerste argument een getal is
        if (resultatenKinderen[0].CelType != CelType.INT)
        {
            return new BerekendeCel($"Fout: resultaat voor {expressiesKinderen[0]} is geen getal.", CelType.ERROR);
        }

        int totaal = Convert.ToInt32(resultatenKinderen[0].VoorstellingWaarde);

        for (int i = 1; i < resultatenKinderen.Count; i++)
        {
            if (resultatenKinderen[i].CelType != CelType.INT)
            {
                return new BerekendeCel($"Fout: resultaat voor {expressiesKinderen[i]} is geen getal.", CelType.ERROR);
            }
            totaal -= Convert.ToInt32(resultatenKinderen[i].VoorstellingWaarde);
        }
        return new BerekendeCel(totaal.ToString(), CelType.INT);
    }
}
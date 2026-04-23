using PluginContracts;

public class PlusPlugin : IPlugin
{
    public string Name => "plus";

    public BerekendeCel Execute(List<string> expressiesKinderen, List<BerekendeCel> resultatenKinderen)
    {
        int totaal = 0;
        for (int i = 0; i < resultatenKinderen.Count; i++)
        {
            var resultaatKind = resultatenKinderen[i];
            var expressieKind = expressiesKinderen[i];

            if (resultaatKind.CelType != CelType.INT)
            {
                return new BerekendeCel($"Fout: resultaat voor {expressieKind} is geen getal.", CelType.ERROR);
            }
            
            totaal += Convert.ToInt32(resultaatKind.VoorstellingWaarde);
        }
        return new BerekendeCel(totaal.ToString(), CelType.INT);
    }
}
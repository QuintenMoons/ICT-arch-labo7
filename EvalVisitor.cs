using System.Text.RegularExpressions;
using Antlr4.Runtime.Misc;
using PluginContracts;

public class EvalVisitor : ExprParserBaseVisitor<BerekendeCel>
{

    private BerekendeCel[,] berekendRooster;
    private List<IPlugin> plugins;
    // TODO hier een lijst of andere datastructuur met ingeladen extensies bijhouden

    public EvalVisitor(BerekendeCel[,] berekendRooster, List<IPlugin> plugins)
    {
        this.berekendRooster = berekendRooster;
        this.plugins = plugins;
        // TODO: hier lijst van plugins voorzien als extra parameter en bijhouden
    }

    public override BerekendeCel VisitFunc([NotNull] ExprParser.FuncContext context)
    {
        var expressiesKinderen = new List<string>();
        var resultatenKinderen = new List<BerekendeCel>();

        // 1. Evalueer alle argumenten van de functie
        foreach (var arg in context.arglist().expr()) // Gebruik .expr() voor directe toegang
        {
            BerekendeCel resultaatKind = this.Visit(arg);
            if (resultaatKind is null)
            {
                return new BerekendeCel($"null als resultaat voor argument {arg.GetText()}", CelType.ERROR);
            }
            expressiesKinderen.Add(arg.GetText());
            resultatenKinderen.Add(resultaatKind);
        }

        // 2. Haal de functienaam op
        var functieNaam = context.ID().GetText();

        // 3. Zoek een plugin die deze naam ondersteunt
        var plugin = plugins.FirstOrDefault(p => p.Name.Equals(functieNaam, StringComparison.OrdinalIgnoreCase));
    
        if (plugin != null)
        {
            // Delegeer de berekening naar de plugin
            return plugin.Execute(expressiesKinderen, resultatenKinderen);
        }

        return new BerekendeCel($"Functie '{functieNaam}' niet gevonden!", CelType.ERROR);
    }

    public override BerekendeCel VisitPlainint([NotNull] ExprParser.PlainintContext context)
    {
        return new BerekendeCel(context.GetText(), CelType.INT);
    }

    public override BerekendeCel VisitPlainstring([NotNull] ExprParser.PlainstringContext context)
    {
        var zonderQuotes = context.GetText().Substring(1);
        zonderQuotes = zonderQuotes.Substring(0, zonderQuotes.Length - 1);
        return new BerekendeCel(zonderQuotes, CelType.STRING);
    }


    public override BerekendeCel VisitReference([NotNull] ExprParser.ReferenceContext context)
    {
        string patroon = @"([A-Z]+)([1-9][0-9]*)";
        var regex = new Regex(patroon);
        var match = regex.Match(context.GetText());
        if (match is not null)
        {
            var kolom = Conversie.LetterVoorstellingNaarGetal(match.Groups[1].Value);
            var rij = Convert.ToInt32(match.Groups[2].Value);
            if (rij > 0 && rij <= this.berekendRooster.GetLength(0) && kolom > 0 && kolom <= this.berekendRooster.GetLength(1))
            {
                return berekendRooster[rij - 1, kolom - 1];
            }
            else
            {
                return new BerekendeCel($"Verwijzing buiten grenzen van het rooster: {context.GetText()}", CelType.ERROR);
            }
        }
        else
        {
            return new BerekendeCel($"Ongeldige verwijzing: {context.GetText()}", CelType.ERROR);
        }
    }

}
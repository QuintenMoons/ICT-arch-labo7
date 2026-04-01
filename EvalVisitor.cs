using System.Text.RegularExpressions;
using Antlr4.Runtime.Misc;

public class EvalVisitor : ExprParserBaseVisitor<BerekendeCel>
{

    private BerekendeCel[,] berekendRooster;
    // TODO hier een lijst of andere datastructuur met ingeladen extensies bijhouden

    public EvalVisitor(BerekendeCel[,] berekendRooster)
    {
        this.berekendRooster = berekendRooster;
        // TODO: hier lijst van plugins voorzien als extra parameter en bijhouden
    }

    public override BerekendeCel VisitFunc([NotNull] ExprParser.FuncContext context)
    {
        var expressiesKinderen = new List<string>();
        var resultatenKinderen = new List<BerekendeCel>();
        foreach (var arg in context.arglist().children)
        {
            if (arg.GetText() != ",")
            {

                BerekendeCel resultaatKind = this.Visit(arg);
                if (resultaatKind is null)
                {
                    return new BerekendeCel($"null als resultaat voor argument {arg.GetText()}", CelType.ERROR);
                }
                expressiesKinderen.Add(arg.GetText());
                resultatenKinderen.Add(resultaatKind);
            }
        }
        var functieNaam = context.ID();
        // TODO: hier moet je met allerlei functies kunnen omgaan
        // begin door plus hier te verwijderen en in een plugin te plaatsen
        if (functieNaam.GetText() == "plus")
        {
            int totaal = 0;
            for (int i = 0; i < resultatenKinderen.Count; i++)
            {
                var expressieKind = expressiesKinderen[i];
                var resultaatKind = resultatenKinderen[i];
                if (resultaatKind.CelType != CelType.INT)
                {
                    return new BerekendeCel($"Fout: resultaat voor {expressieKind} in {context.GetText()} is geen getal.", CelType.ERROR);
                }
                else
                {
                    totaal += Convert.ToInt32(resultaatKind.VoorstellingWaarde);
                }

            }
            return new BerekendeCel(totaal.ToString(), CelType.INT);
        }

        if (functieNaam.GetText() == "min")
        {
            if (expressiesKinderen.Count < 1 || resultatenKinderen.Count < 1)
            {
                return new BerekendeCel($"Fout: min-operatie vereist minstens 1 operand.", CelType.ERROR);
            }
            var expressieKind1 = expressiesKinderen[0];
            var resultaatKind1 = resultatenKinderen[0];
            if (resultaatKind1.CelType != CelType.INT)
            {
                return new BerekendeCel($"Fout: resultaat voor {expressieKind1} in {context.GetText()} is geen getal.", CelType.ERROR);
            }
            int totaal = Convert.ToInt32(resultaatKind1.VoorstellingWaarde);
            for (int i = 1; i < resultatenKinderen.Count; i++)
            {
                var expressieKind = expressiesKinderen[i];
                var resultaatKind = resultatenKinderen[i];
                if (resultaatKind.CelType != CelType.INT)
                {
                    return new BerekendeCel($"Fout: resultaat voor {expressieKind} in {context.GetText()} is geen getal.", CelType.ERROR);
                }
                else
                {
                    totaal -= Convert.ToInt32(resultaatKind.VoorstellingWaarde);
                }

            }
            return new BerekendeCel(totaal.ToString(), CelType.INT);
        }

        if (functieNaam.GetText() == "concat")
        {
            string totaal = "";
            for (int i = 0; i < resultatenKinderen.Count; i++)
            {
                var expressieKind = expressiesKinderen[i];
                var resultaatKind = resultatenKinderen[i];
                if (resultaatKind.CelType != CelType.STRING)
                {
                    return new BerekendeCel($"Fout: resultaat voor {expressieKind} in {context.GetText()} is geen string.", CelType.ERROR);
                }
                else
                {
                    totaal += resultaatKind.VoorstellingWaarde;
                }

            }
            return new BerekendeCel(totaal.ToString(), CelType.STRING);
        }

        if (functieNaam.GetText() == "upperCase")
        {
            if (resultatenKinderen.Count != 1)
            {
                return new BerekendeCel($"Fout: upperCase verwacht exact 1 argument, gekregen: {resultatenKinderen.Count}", CelType.ERROR);
            }

            var resultaatKind = resultatenKinderen[0];
            if (resultaatKind.CelType != CelType.STRING)
            {
                return new BerekendeCel($"Fout: resultaat voor {expressiesKinderen[0]} is geen string.", CelType.ERROR);
            }

            string hoofdletters = resultaatKind.VoorstellingWaarde.ToUpper();
    
            return new BerekendeCel(hoofdletters, CelType.STRING);
        }
        return new BerekendeCel($"Kan {context.GetText()} niet uitrekenen!", CelType.ERROR);
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
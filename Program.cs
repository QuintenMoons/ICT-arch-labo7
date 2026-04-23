using Antlr4.Runtime;
using PluginContracts;

namespace LaboOefeningen
{
    public class TextCellUitgebreid
    {

        private static List<IPlugin>? _geladenPlugins;
        public static BerekendeCel BerekenOntbrekendeCelRechtstreeks(string formule, BerekendeCel[,] berekendRooster)
        {
            var expr = stringToExprParser(formule).expr();
    
            // Gebruik de lijst die door de loader is gevuld
            var evalVisitor = new EvalVisitor(berekendRooster, _geladenPlugins ?? new List<IPlugin>()); 
            return evalVisitor.VisitExpr(expr);
        }

        public static BerekendeCel[,] RekenRoosterUit(string[,] rooster)
        {
            BerekendeCel[,] berekendRooster = new BerekendeCel[rooster.GetLength(0), rooster.GetLength(1)];
            KopieerCellenZonderFormule(rooster, berekendRooster);
            while (BevatNullWaarden(berekendRooster))
            {
                BerekenOntbrekendeWaardenEenKeer(rooster, berekendRooster);
            }
            return berekendRooster;
        }

        /* Weergave van een uitgerekend rooster. Niet aanpassen. */
        public static void ToonRooster(BerekendeCel[,] rooster)
        {
            int maximumRijNummer = rooster.GetLength(0);
            int breedteRijIndex = maximumRijNummer.ToString().Length;
            int[] kolombreedtes = new int[rooster.GetLength(1)];
            for (int i = 0; i < kolombreedtes.Length; i++)
            {
                kolombreedtes[i] = 1;
            }
            for (int rij = 0; rij < rooster.GetLength(0); rij++)
            {
                for (int kolom = 0; kolom < rooster.GetLength(1); kolom++)
                {
                    int width = rooster[rij, kolom].VoorstellingWaarde.Length;
                    if (rooster[rij, kolom].VoorstellingWaarde != "" && rooster[rij, kolom].CelType == CelType.STRING)
                    {
                        width += 2;
                    }
                    kolombreedtes[kolom] = Math.Max(kolombreedtes[kolom], width);
                }
            }
            Console.Write("".PadRight(breedteRijIndex));
            for (int i = 0; i < rooster.GetLength(1); i++)
            {
                Console.Write("|");
                string tekstVoorstelling = Conversie.GetalVoorstellingNaarLetters(i + 1).PadRight(kolombreedtes[i]);
                Console.Write(tekstVoorstelling);
            }
            Console.WriteLine("|");
            for (int rij = 0; rij < rooster.GetLength(0); rij++)
            {
                Console.Write((rij + 1).ToString().PadRight(breedteRijIndex));
                for (int kolom = 0; kolom < rooster.GetLength(1); kolom++)
                {
                    Console.Write("|");
                    var cell = rooster[rij, kolom];
                    var toPrint = cell.VoorstellingWaarde;

                    if (cell.VoorstellingWaarde != "" && cell.CelType == CelType.STRING)
                    {
                        toPrint = "\"" + toPrint + "\"";
                    }
                    else if (cell.CelType == CelType.ERROR) {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.Write(toPrint.PadRight(kolombreedtes[kolom]));
                    Console.ResetColor();
                }
                Console.WriteLine("|");
            }
        }

        /* Als een cel geen formule bevat, hoeven we niets uit te rekenen, dus komt ze meteen in het uitgerekende rooster.
         */
        public static void KopieerCellenZonderFormule(string[,] roosterIn, BerekendeCel[,] roosterUit)
        {
            for (int rij = 0; rij < roosterIn.GetLength(0); rij++)
            {
                for (int kolom = 0; kolom < roosterIn.GetLength(1); kolom++)
                {
                    var ruweWaarde = roosterIn[rij, kolom];
                    // check expliciet omdat ANTLR anders errors print, zelfs al werkt rest
                    if (ruweWaarde != "")
                    {
                        var expr = stringToExprParser(roosterIn[rij, kolom]).expr();
                        if (expr.plainstring() is not null)
                        {
                            var quoteless = roosterIn[rij, kolom].Substring(1);
                            quoteless = quoteless.Substring(0, quoteless.Length - 1);
                            roosterUit[rij, kolom] = new BerekendeCel(quoteless, CelType.STRING);
                        }
                        else if (expr.plainint() is not null)
                        {
                            roosterUit[rij, kolom] = new BerekendeCel(roosterIn[rij, kolom], CelType.INT);
                        }
                        else
                        {
                            // pass, references and function calls need to be computed
                        }
                    }
                    else {
                        roosterUit[rij, kolom] = new BerekendeCel("", CelType.STRING);
                    }
                }
            }
        }

        /* null betekent dat een bepaalde cel nog niet volledig is uitgerekend, want een lege cel bevat "".
         * Dit staat dus toe om te checken of een rooster nog verder moet wordne uitgerekend.
         */
        public static bool BevatNullWaarden(BerekendeCel[,] rooster)
        {
            for (int rij = 0; rij < rooster.GetLength(0); rij++)
            {
                for (int kolom = 0; kolom < rooster.GetLength(1); kolom++)
                {
                    if (rooster[rij, kolom] is null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void BerekenOntbrekendeWaardenEenKeer(string[,] rooster, BerekendeCel[,] berekendRooster)
        {
            // Voeg ?? new List<IPlugin>() toe aan het einde van de argumenten
            var evalVisitor = new EvalVisitor(berekendRooster, _geladenPlugins ?? new List<IPlugin>());

            for (int rij = 0; rij < rooster.GetLength(0); rij++)
            {
                for (int kolom = 0; kolom < rooster.GetLength(1); kolom++)
                {
                    if (berekendRooster[rij, kolom] is null)
                    {
                        berekendRooster[rij, kolom] = BerekenOntbrekendeCelRechtstreeks(rooster[rij, kolom], berekendRooster);
                    }
                }
            }
        }

        public static void WijzigCel(string[,] rooster)
        {

            Console.WriteLine("Welke rij wil je wijzigen?");
            string rijIndexString = Console.ReadLine();
            int rijIndex;
            try {
                rijIndex = Convert.ToInt32(rijIndexString) - 1;
            }
            catch {
                Console.WriteLine("Geen mogelijke rij-index.");
                return;
            }
            Console.WriteLine("Welke kolom wil je wijzigen?");
            string kolomIndexString = Console.ReadLine();
            int kolomIndex;
            try {
                kolomIndex = Conversie.LetterVoorstellingNaarGetal(kolomIndexString) - 1;
            }
            catch {
                Console.WriteLine("Geen mogelijke kolomindex.");
                return;
            }
            Console.WriteLine("Wat wil je hier invullen?");
            string ruweInvoer = Console.ReadLine();
            var charStream = new AntlrInputStream(ruweInvoer);
            var lexer = new ExprLexer(charStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExprParser(tokenStream);
            var expression = parser.expr();
            if (expression.exception is not null || tokenStream.LA(1) != IntStreamConstants.EOF)
            {
                Console.WriteLine("Expressie volgt de grammatica niet. Wordt niet ingevuld.");
            }
            else
            {
                rooster[rijIndex, kolomIndex] = ruweInvoer;
            }
        }

        private static ExprParser stringToExprParser(string value)
        {
            var charStream = new AntlrInputStream(value);
            var lexer = new ExprLexer(charStream);
            var tokenStream = new CommonTokenStream(lexer);
            return new ExprParser(tokenStream);
        }

        public static void Main()
        {
            string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            _geladenPlugins = PluginLoader.LoadPlugins(pluginPath);

            Console.WriteLine("Hoe veel rijen telt je spreadsheet?");
            int aantalRijen = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Hoe veel kolommen telt je spreadsheet?");
            int aantalKolommen = Convert.ToInt32(Console.ReadLine());
            string[,] rooster = new string[aantalRijen, aantalKolommen];
            for (int rij = 0; rij < aantalRijen; rij++)
            {
                for (int kolom = 0; kolom < aantalKolommen; kolom++)
                {
                    rooster[rij, kolom] = "";
                }
            }
            while (true)
            {
                BerekendeCel[,] berekendRooster = RekenRoosterUit(rooster);
                ToonRooster(berekendRooster);
                WijzigCel(rooster);
            }
        }
    }
}

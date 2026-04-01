public class Conversie
{

    /* Deze functie dient enkel om kolomnamen als letters te tonen. Niet aanpassen. */
    public static string GetalVoorstellingNaarLetters(int getal)
    {
        string alfabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int aantalSymbolen = alfabet.Length;
        string resultaat = "";
        int resterend = getal;
        bool laatsteKeer = false;
        while (!laatsteKeer)
        {
            if (resterend <= alfabet.Length)
            {
                laatsteKeer = true;
            }
            int karakterIndex = (resterend - 1) % alfabet.Length;
            resultaat = alfabet[karakterIndex] + resultaat;
            resterend = (resterend - 1) / alfabet.Length;
        }
        return resultaat;
    }

    /* Deze functie dient enkel om letters naar getallen om te zetten. Niet aanpassen. */
    public static int LetterVoorstellingNaarGetal(string letters)
    {
        string alfabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int aantalSymbolen = alfabet.Length;
        int resultaat = 0;
        for (int i = letters.Length - 1; i >= 0; i--)
        {
            if (!alfabet.Contains(letters[i])) {
                throw new ArgumentException("Kolom mag alleen uit letters bestaan");
            }
            int exponent = letters.Length - i - 1;
            resultaat += (alfabet.IndexOf(letters[i]) + 1) * (int)Math.Pow(aantalSymbolen, exponent);
        }
        return resultaat;

    }
}
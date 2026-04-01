public enum CelType {
    INT,
    STRING,
    ERROR
}

public class BerekendeCel {
    private CelType celType;
    private string voorstellingWaarde;

    public string VoorstellingWaarde {
        get {
            return this.voorstellingWaarde;
        }
    }

    public CelType CelType {
        get {
            return this.celType;
        }
    }

    public BerekendeCel(string valueRepresentation, CelType celType) {
        this.voorstellingWaarde = valueRepresentation;
        this.celType = celType;
    }

}
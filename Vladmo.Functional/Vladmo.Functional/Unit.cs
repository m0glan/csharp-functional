namespace Vladmo.Functional;

public record Unit
{
    public static readonly Unit Instance = new();
    
    private Unit() { }
}
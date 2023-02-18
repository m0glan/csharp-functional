namespace Vladmo.Functional;

public static class ObjectExtensions
{
    public static T Id<T>(this T x) => x;

    public static Unit Ignore(this object _) => Unit.Instance;
}
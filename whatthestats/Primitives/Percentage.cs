namespace whatthestats.Primitives;

public readonly record struct Percentage : IComparable<Percentage>
{
    // Backing value constrained between 0.0 and 1.0
    public double Value { get; }

    public Percentage(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentException("Percentage cannot be NaN or Infinity.", nameof(value));
        }

        // Clamp or throw depending on your preference. 
        // Throwing enforces a strict invariant:
        if (value is < 0.0 or > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Percentage must be between 0.0 and 1.0.");
        }

        Value = value;
    }

    // Factory methods for convenience
    public static Percentage FromFraction(double fraction) => new(fraction);
    public static Percentage FromPercent(double percent) => new(percent / 100.0);

    // Implicit conversion FROM Percentage to double
    public static implicit operator double(Percentage p) => p.Value;

    // Explicit conversion FROM double to Percentage (forces validation)
    public static implicit operator Percentage(double value) => new(value);

    // Common arithmetic operators
    public static Percentage operator +(Percentage a, Percentage b) => new(Math.Min(1.0, a.Value + b.Value));
    public static Percentage operator -(Percentage a, Percentage b) => new(Math.Max(0.0, a.Value - b.Value));

    // Multiplying a number by a percentage returns a double
    public static double operator *(double value, Percentage percentage) => value * percentage.Value;
    public static double operator *(Percentage percentage, double value) => percentage.Value * value;

    // Comparison & Formatting
    public int CompareTo(Percentage other) => Value.CompareTo(other.Value);

    public override string ToString() => $"{Value * 100:F1}%";
    public string ToString(string? format) => (Value * 100).ToString(format) + "%";
}
namespace API.DataTypes;

public class Pair<TA, TB>
{
    public TA A { get; set; }
    public TB B { get; set; }
    
    public Pair(TA a, TB b)
    {
        this.A = a;
        this.B = b;
    }
}
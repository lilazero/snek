public class ScoreCounter
{
    public int Score { get; private set; }

    public ScoreCounter()
    {
        Score = 0;
    }

    public void Increment()
    {
        Score++;
    }

    public void Reset()
    {
        Score = 0;
    }
}

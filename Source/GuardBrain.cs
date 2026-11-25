namespace LanternHollow;

public enum GuardAction
{
    Patrol,
    Chase,
    TurnBack,
    Wait
}

public readonly record struct Senses(float DistanceX, float DistanceY, bool EdgeAhead);

public static class GuardBrain
{
    public static GuardAction Decide(Senses s, bool wasChasing)
    {
        // Determine if player is seen
        // seen when DistanceY <= VisionHeight and DistanceX <= SightRange (LoseRange if wasChasing)
        float sightRange = wasChasing ? Tuning.LoseRange : Tuning.SightRange;
        bool seen = s.DistanceY <= Tuning.VisionHeight && s.DistanceX <= sightRange;

        // Rule 1: seen and (EdgeAhead or DistanceX <= ArriveDistance) gives Wait
        if (seen && (s.EdgeAhead || s.DistanceX <= Tuning.ArriveDistance))
        {
            return GuardAction.Wait;
        }

        // Rule 2: EdgeAhead gives TurnBack
        if (s.EdgeAhead)
        {
            return GuardAction.TurnBack;
        }

        // Rule 3: seen gives Chase
        if (seen)
        {
            return GuardAction.Chase;
        }

        // Rule 4: otherwise Patrol
        return GuardAction.Patrol;
    }
}
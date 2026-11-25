using Xunit;

namespace LanternHollow.Tests;

public class GuardBrainTests
{
    [Fact]
    public void Seen_Plus_EdgeAhead_Gives_Wait_Not_TurnBack()
    {
        var senses = new Senses(50f, 50f, true);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: false);
        Assert.Equal(LanternHollow.GuardAction.Wait, result);
    }

    [Fact]
    public void EdgeAhead_Gives_TurnBack_When_Not_Seen()
    {
        var senses = new Senses(500f, 500f, true);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: false);
        Assert.Equal(LanternHollow.GuardAction.TurnBack, result);
    }

    [Fact]
    public void Seen_Gives_Chase_When_Not_EdgeAhead()
    {
        var senses = new Senses(100f, 50f, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: false);
        Assert.Equal(LanternHollow.GuardAction.Chase, result);
    }

    [Fact]
    public void Not_Seen_Gives_Patrol()
    {
        var senses = new Senses(500f, 500f, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: false);
        Assert.Equal(LanternHollow.GuardAction.Patrol, result);
    }

    [Fact]
    public void Seen_Plus_DistanceXAtMostArriveDistance_Gives_Wait()
    {
        var senses = new Senses(10f, 50f, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: false);
        Assert.Equal(LanternHollow.GuardAction.Wait, result);
    }

    [Fact]
    public void DistanceX250_NotChasing_Gives_Patrol()
    {
        var senses = new Senses(250f, 50f, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: false);
        Assert.Equal(LanternHollow.GuardAction.Patrol, result);
    }

    [Fact]
    public void DistanceX250_WasChasing_Gives_Chase()
    {
        var senses = new Senses(250f, 50f, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: true);
        Assert.Equal(LanternHollow.GuardAction.Chase, result);
    }

    [Fact]
    public void DistanceX320_WasChasing_Gives_Patrol()
    {
        var senses = new Senses(320f, 50f, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: true);
        Assert.Equal(LanternHollow.GuardAction.Patrol, result);
    }

    [Fact]
    public void DistanceY200_Gives_Patrol()
    {
        var senses = new Senses(100f, 200f, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: false);
        Assert.Equal(LanternHollow.GuardAction.Patrol, result);
    }

    [Fact]
    public void EdgeAheadTrue_Never_PatrolOrChase()
    {
        var distanceXValues = new[] { 0f, 12f, 100f, 220f, 300f, 500f };
        var distanceYValues = new[] { 0f, 96f, 200f };

        foreach (var distanceX in distanceXValues)
        {
            foreach (var distanceY in distanceYValues)
            {
                foreach (var wasChasing in new[] { false, true })
                {
                    var senses = new Senses(distanceX, distanceY, true);
                    var result = LanternHollow.GuardBrain.Decide(senses, wasChasing);
                    Assert.False(result == LanternHollow.GuardAction.Patrol, $"EdgeAhead=true, DistanceX={distanceX}, DistanceY={distanceY}, wasChasing={wasChasing} should not be Patrol");
                    Assert.False(result == LanternHollow.GuardAction.Chase, $"EdgeAhead=true, DistanceX={distanceX}, DistanceY={distanceY}, wasChasing={wasChasing} should not be Chase");
                }
            }
        }
    }

    [Fact]
    public void DistanceXExactlySightRange_NotChasing_Gives_Chase()
    {
        var senses = new Senses(Tuning.SightRange, 50f, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: false);
        Assert.Equal(LanternHollow.GuardAction.Chase, result);
    }

    [Fact]
    public void DistanceXSightRangePlusOne_NotChasing_Gives_Patrol()
    {
        var senses = new Senses(Tuning.SightRange + 1f, 50f, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: false);
        Assert.Equal(LanternHollow.GuardAction.Patrol, result);
    }

    [Fact]
    public void DistanceXExactlyLoseRange_WasChasing_Gives_Chase()
    {
        var senses = new Senses(Tuning.LoseRange, 50f, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: true);
        Assert.Equal(LanternHollow.GuardAction.Chase, result);
    }

    [Fact]
    public void DistanceXLoseRangePlusOne_WasChasing_Gives_Patrol()
    {
        var senses = new Senses(Tuning.LoseRange + 1f, 50f, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: true);
        Assert.Equal(LanternHollow.GuardAction.Patrol, result);
    }

    [Fact]
    public void DistanceYExactlyVisionHeight_Gives_Chase()
    {
        var senses = new Senses(50f, Tuning.VisionHeight, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: false);
        Assert.Equal(LanternHollow.GuardAction.Chase, result);
    }

    [Fact]
    public void DistanceXExactlyArriveDistance_Gives_Wait()
    {
        var senses = new Senses(Tuning.ArriveDistance, 50f, false);
        var result = LanternHollow.GuardBrain.Decide(senses, wasChasing: false);
        Assert.Equal(LanternHollow.GuardAction.Wait, result);
    }
}
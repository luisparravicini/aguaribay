using System;
using UnityEngine;

public enum Direction
{
    N = 0,
    E,
    S,
    W,
};

static class DirectionExtensions
{
    public static int ToBits(this Direction d)
    {
        return 1 << (int)d;
    }

    public static Direction Opposite(this Direction d)
    {
        switch (d)
        {
            case Direction.N: return Direction.S;
            case Direction.S: return Direction.N;
            case Direction.E: return Direction.W;
            case Direction.W: return Direction.E;
            default:
                throw new ArgumentOutOfRangeException(d.ToString());
        }

    }

    public static Direction Next(this Direction dir, int delta)
    {
        var size = Enum.GetValues(typeof(Direction)).Length;
        int nextDir = ((int)dir + (int)Mathf.Sign(delta)) % size;
        if (nextDir < 0) nextDir = size + nextDir;

        return (Direction)nextDir;
    }

}
using UnityEngine;


public class MazeSpec
{
    int[,] data;

    public MazeSpec(Vector2Int size)
    {
        data = new int[size.x, size.y];
    }

    public void Carve(Vector2Int src, Vector2Int dst)
    {
        var delta = dst - src;
        if (delta != Vector2Int.zero)
        {
            var d = Direction.N;
            if (delta.x == 1)
                d = Direction.E;
            if (delta.x == -1)
                d = Direction.W;
            if (delta.y == 1)
                d = Direction.N;
            if (delta.y == 0)
                d = Direction.S;
            data[src.x, src.y] |= d.ToBits();
        }
    }

    public Vector2Int? Move(Vector2Int pos, Direction dir)
    {
        var bitDir = dir.ToBits();
        if ((data[pos.x, pos.y] & bitDir) != bitDir)
            return null;

        switch (dir)
        {
            case Direction.N:
                pos.y += 1;
                break;
            case Direction.S:
                pos.y -= 1;
                break;
            case Direction.E:
                pos.x += 1;
                break;
            case Direction.W:
                pos.x -= 1;
                break;
        }
        return pos;
    }

}

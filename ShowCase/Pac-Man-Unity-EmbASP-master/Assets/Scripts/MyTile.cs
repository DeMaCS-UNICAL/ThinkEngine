public class MyTile
{
    public bool pacdot;
    public bool occupied;
    public int x;
    public int y;

    public MyTile(int i, int j)
    {
        this.x = i;
        this.y = j;
        pacdot = false;
        occupied = false;

    }
}

  public class Distance {
    private int x1;
    private int y1;
    private int x2;
    private int y2;
    private int distance;

    public Distance() {
    }

    public Distance(int x1, int y1, int x2, int y2, int distance) {
      this.x1 = x1;
      this.y1 = y1;
      this.x2 = x2;
      this.y2 = y2;
      this.distance = distance;
    }

    public int getX1() {
      return x1;
    }

    public void setX1(int value) {
      x1 = value;
    }

    public int getY1() {
      return y1;
    }

    public void setY1(int value) {
      y1 = value;
    }

    public int getX2() {
      return x2;
    }

    public void setX2(int value) {
      x2 = value;
    }

    public int getY2() {
      return y2;
    }

    public void setY2(int value) {
      y2 = value;
    }

    public int getDistance() {
      return distance;
    }

    public void setDistance(int value) {
      distance = value;
    }
  }
  
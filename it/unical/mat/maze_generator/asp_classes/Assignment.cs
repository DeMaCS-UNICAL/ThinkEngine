namespace it.unical.mat.asp_classes {
  using Id = it.unical.mat.embasp.languages.Id;
  using Param = it.unical.mat.embasp.languages.Param;

  [Id("assignment5")]
  public class Assignment {
    [Param(0)]
    private int minRow;
    
    [Param(1)]
    private int minCol;
    
    [Param(2)]
    private int maxRow;

    [Param(3)]
    private int maxCol;
    
    [Param(4)]
    private string type;

    public Assignment(int minRow1, int minCol1, int maxRow1, int maxCol1, string type1) : base() {
      this.minRow = minRow1;
      this.minCol = minCol1;
      this.maxRow = maxRow1;
      this.maxCol = maxCol1;
      this.type = type1;
    }

    public Assignment() {
    }

    public int getMinRow() {
      return minRow;
    }

    public void setMinRow(int value) {
      this.minRow = value;
    }

    public int getMinCol() {
      return minCol;
    }

    public void setMinCol(int value) {
      this.minCol = value;
    }

    public int getMaxRow() {
      return maxRow;
    }

    public void setMaxRow(int value) {
      this.maxRow = value;
    }

    public int getMaxCol() {
      return maxCol;
    }

    public void setMaxCol(int value) {
      this.maxCol = value;
    }

    public string getType() {
      return type;
    }

    public void setType(string value) {
      this.type = value;
    }


    public override string ToString() {
      return "assignment(p(" + minRow + "," + minCol + "," + maxRow + "," + maxCol + "), " + type + ").";
    }
  }

}
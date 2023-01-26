namespace it.unical.mat.asp_classes {
  using Id = it.unical.mat.embasp.languages.Id;
  using Param = it.unical.mat.embasp.languages.Param;

  [Id("object_assignment6")]
  public class ObjectAssignment {
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
    
    [Param(5)]
    private int object_id;

    public ObjectAssignment(int minRow, int minCol, int maxRow, int maxCol, string type, int object_id) : base() {
      this.minRow = minRow;
      this.minCol = minCol;
      this.maxRow = maxRow;
      this.maxCol = maxCol;
      this.type = type;
      this.object_id = object_id;
    }

    public ObjectAssignment() {
    }

    public virtual int getMinRow() {
      return minRow;
    }

    public virtual void setMinRow(int value) {
      this.minRow = value;
    }

    public virtual int getMinCol() {
      return minCol;
    }


    public virtual void setMinCol(int value) {
      this.minCol = value;
    }

    public virtual int getMaxRow() {
      return maxRow;
    }


    public virtual void setMaxRow(int value) {
      this.maxRow = value;
    }

    public virtual int getMaxCol() {
      return maxCol;
    }


    public virtual void setMaxCol(int value) {
      this.maxCol = value;
    }

    public virtual string getType() {
      return type;
    }


    public virtual void setType(string value) {
      this.type = value;
    }

    public virtual int getObject_id() {
      return object_id;
    }


    public virtual void setObject_id(int value) {
      this.object_id = value;
    }

    public override string ToString() {
      return "object_assignment(p(" + minRow + "," + minCol + "," + maxRow + "," + maxCol + ")," + type + "," + object_id + ").";
    }
  }

}
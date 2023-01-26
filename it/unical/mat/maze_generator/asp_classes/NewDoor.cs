namespace it.unical.mat.asp_classes {
  using Id = it.unical.mat.embasp.languages.Id;
  using Param = it.unical.mat.embasp.languages.Param;

  [Id("new_door")]
  public class NewDoor {
    [Param(0)]
    private int row;
    [Param(1)]
    private int column;
    [Param(2)]
    private string type;

    public NewDoor(int r, int c, string t) {
      setRow(r);
      setColumn(c);
      setType(t);
    }

    public NewDoor() {
    }

    public virtual int getRow() {
      return row;
    }

    public virtual void setRow(int value) {
      this.row = value;
    }

    public virtual int getColumn() {
      return column;
    }


    public virtual void setColumn(int value) {
      this.column = value;
    }

    public virtual string getType() {
      return type;
    }


    public virtual void setType(string value) {
      this.type = value;
      this.convertTypeFormat();
    }


    public virtual void convertTypeFormat() {
      type = type.Replace("\"", "");
    }

    public override string ToString() {
      return "new_door(" + row + "," + column + "," + type + "). ";
    }

  }

}
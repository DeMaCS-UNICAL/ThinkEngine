namespace it.unical.mat.asp_classes
{
	using Id = it.unical.mat.embasp.languages.Id;
	using Param = it.unical.mat.embasp.languages.Param;

  [Id("cell")]
  public class Cell
	{
    [Param(0)]
    private int row;

    [Param(1)]
    private int column;
    
    [Param(2)]
    private string type;

		public Cell(int r, int c, string t)
		{
      setRow(r);
      setColumn(c);
      setType(t);
		}

		public Cell()
		{
		}

    public  int getRow() {
      return row;
    }

    public void setRow(int value) {
      this.row = value;
    }

    public int getColumn() {
      return column;
    }


    public void setColumn(int value) {
      this.column = value;
    }

    public string getType() {
      return type;
    }


    public void setType(string value) {
      this.type = value;
      this.convertTypeFormat();
    }


    public void convertTypeFormat()
		{
			type = type.Replace("\"", "");
		}

		public string GVGAI
		{
			get
			{
    
				switch (type)
				{
					case "wall":
						return "w";
					case "pellet":
						return ".";
					case "hdoor":
						return "||";
					case "vdoor":
						return "=";
					default:
						throw new System.ArgumentException("Char " + type + " not implemented yet !");
				}
			}
		}

		public override string ToString()
		{
			return "cell(" + row + "," + column + "," + type + "). ";
		}

	}

}
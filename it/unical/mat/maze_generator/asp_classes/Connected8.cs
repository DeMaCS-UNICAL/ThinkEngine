namespace it.unical.mat.asp_classes {
  using Id = it.unical.mat.embasp.languages.Id;
  using Param = it.unical.mat.embasp.languages.Param;

  [Id("connected8")]
  public class Connected8 {
    [Param(0)]
    private int minRow1;

    [Param(1)]
    private int minCol1;
    
    [Param(2)]
    private int maxRow1;
    
    [Param(3)]
    private int maxCol1;
    
    [Param(4)]
    private int minRow2;
    
    [Param(5)]
    private int minCol2;
    
    [Param(6)]
    private int maxRow2;
    
    [Param(7)]
    private int maxCol2;

    public Connected8() : base() {
    }

    public virtual int getMaxCol1() {
      return maxCol1;
    }

    public virtual void setMaxCol1(int value) {
      this.maxCol1 = value;
    }

    public virtual int getMaxCol2() {
      return maxCol2;
    }

    public virtual void setMaxCol2(int value) {
      this.maxCol2 = value;
    }

    public virtual int getMaxRow1() {
      return maxRow1;
    }

    public virtual void setMaxRow1(int value) {
      this.maxRow1 = value;
    }

    public virtual int getMaxRow2() {
      return maxRow2;
    }

    public virtual void setMaxRow2(int value) {
      this.maxRow2 = value;
    }

    public virtual int getMinCol1() {
      return minCol1;
    }

    public virtual void setMinCol1(int value) {
      this.minCol1 = value;
    }

    public virtual int getMinCol2() {
      return minCol2;
    }

    public virtual void setMinCol2(int value) {
      this.minCol2 = value;
    }

    public virtual int getMinRow1() {
      return minRow1;
    }

    public virtual void setMinRow1(int value) {
      this.minRow1 = value;
    }

    public virtual int getMinRow2() {
      return minRow2;
    }

    public virtual void setMinRow2(int value) {
      this.minRow2 = value;
    }

    public override string ToString() {
      return "connected8(" + minRow1 + "," + minCol1 + "," + maxRow1 + "," + maxCol1 + "," + minRow2 + "," + minCol2 + "," + maxRow2 + "," + maxCol2 + ").";
    }

  }

}
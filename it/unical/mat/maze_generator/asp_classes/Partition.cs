using it.unical.mat.embasp.languages;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace it.unical.mat.asp_classes {

  [Id("partition4")]
  public class Partition {

    public static Partition GetPartition(string partitionTuple) {
      // Create a Pattern object
      string patternToFind = ("\\d+,\\d+,\\d+,\\d+");
      Regex r = new Regex(patternToFind);

      // Now create matcher object.
      Match m = r.Match(partitionTuple);

      if (m.Success) {
        int minRow = int.Parse(m.Groups[0].Value);
        int minCol = int.Parse(m.Groups[1].Value);
        int maxRow = int.Parse(m.Groups[2].Value);
        int maxCol = int.Parse(m.Groups[3].Value);

        return new Partition(minRow, minCol, maxRow, maxCol);
      }
      return new Partition();
    }

    [Param(0)]
    private int minRow;
    [Param(1)]
    private int minCol;
    [Param(2)]
    private int maxRow;
    [Param(3)]
    private int maxCol;

    public String Type { get; set; }

    private IList<Pair<int, int>> doors = new List<Pair<int, int>>();

    public Partition() {
    }

    public Partition(int minRow, int minCol, int maxRow, int maxCol) : base() {
      this.minRow = minRow;
      this.minCol = minCol;
      this.maxRow = maxRow;
      this.maxCol = maxCol;
    }


    public virtual int getMaxCol() {
      return maxCol;
    }

    public virtual void setMaxCol(int value) {
      this.maxCol = value;
    }

    public virtual int getMaxRow() {
      return maxRow;
    }

    public virtual void setMaxRow(int value) {
      this.maxRow = value;
    }

    public virtual int getMinCol() {
      return minCol;
    }

    public virtual void setMinCol(int value) {
      this.minCol = value;
    }

    public virtual int getMinRow() {
      return minRow;
    }

    public virtual void setMinRow(int value) {
      this.minRow = value;
    }

    public virtual int Size {
      get {
        return (maxRow - minRow + 1) * (maxCol - minCol + 1);
      }
    }

    public virtual IList<Pair<int, int>> Doors {
      get {
        return doors;
      }
    }


    

    public override string ToString() {
      return "partition(p(" + minRow + "," + minCol + "," + maxRow + "," + maxCol + ")).";
    }

    public override bool Equals(object obj) {
      var partition = obj as Partition;
      return partition != null &&
             minRow == partition.minRow &&
             minCol == partition.minCol &&
             maxRow == partition.maxRow &&
             maxCol == partition.maxCol;
    }

    public override int GetHashCode() {
      var hashCode = -231818456;
      hashCode = hashCode * -1521134295 + minRow.GetHashCode();
      hashCode = hashCode * -1521134295 + minCol.GetHashCode();
      hashCode = hashCode * -1521134295 + maxRow.GetHashCode();
      hashCode = hashCode * -1521134295 + maxCol.GetHashCode();
      return hashCode;
    }
  }

}
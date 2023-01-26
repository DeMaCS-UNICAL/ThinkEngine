using System;
using System.Collections.Generic;

namespace it.unical.mat.asp_classes {

  public sealed class CellManager {

    internal Dictionary<Pair<int, int>, Cell> cells;
    internal Dictionary<string, HashSet<Cell>> cellsType;
    internal List<Cell> allCells;

    private static CellManager instance = null;
    private static readonly object padlock = new object();

    public static CellManager Instance {
      get {
        if (instance == null)
          lock (padlock)
            if (instance == null)
              instance = new CellManager();
        return instance;
      }
    }

    public void Reset() {
      cells.Clear();
      cellsType["hdoor"].Clear();
      cellsType["hdoor"].Clear();
      allCells.Clear();
    }

    private CellManager() {
      cells = new Dictionary<Pair<int, int>, Cell>();
      cellsType = new Dictionary<string, HashSet<Cell>>();
      allCells = new List<Cell>();

      cellsType["hdoor"] = new HashSet<Cell>();
      cellsType["vdoor"] = new HashSet<Cell>();
    }

    private CellManager(Dictionary<Pair<int, int>, Cell> cells, Dictionary<string, HashSet<Cell>> cellsType, List<Cell> allCells) : base() {
      this.cells = cells;
      this.cellsType = cellsType;
      this.allCells = allCells;
    }

    public Dictionary<Pair<int, int>, Cell> Cells => cells;

    public Dictionary<string, HashSet<Cell>> CellsType => cellsType;

    public void AddCell(Cell cell) {
      cells[new Pair<int, int>(cell.getRow(), cell.getColumn())] = cell;
      this.AddCellType(cell);
      allCells.Add(cell);
    }

    private void AddCellType(Cell cell) {
      if (cellsType.ContainsKey(cell.GVGAI))
        cellsType[cell.GVGAI].Add(cell);
      else {
        HashSet<Cell> setCells = new HashSet<Cell>();
        setCells.Add(cell);
        cellsType[cell.GVGAI] = setCells;
      }

    }

    public ICollection<Cell> SetCells => cells.Values;

    public ICollection<Cell> GetSetCellsType(string type) {
      return cellsType[type];
    }

    public List<Cell> GetAllCells() {
      return allCells;
    }

  }

}
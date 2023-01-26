using AIMapGenerator;
using AIMapGenerator.it.unical.mat.asp_classes;
using it.unical.mat.asp_classes;
using it.unical.mat.embasp.@base;
using it.unical.mat.embasp.languages.asp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace it.unical.mat.map_generator {

  public class MapGenerator {

    private static bool IS_DEBUG_MODE;
    /////////////////////// CLASS FIELDS //////////////////////////
    private readonly int maxColumns;
    private readonly int maxRows;
    private readonly int minDistanceWall;
    private readonly int randomAnswersetNumber;
    private readonly int minRoomSize;
    private readonly float pruningPercentage;
    private readonly float sameOrientationPercentage;
    private int numEmptyPartitions;
    private int numPartitionsToBuild;
    private readonly CellManager matrixCells;
    private readonly string encodingFolder;
    private readonly System.Random randomGenerator;
    private readonly string[] orientation = new string[] { "vertical", "horizontal" };
    private IList<Partition> partitions;
    private IList<Connected8> connections;
    ///////////////////////////////////////////////////////////////

    public MapGenerator() {
      maxRows = MapGeneratorLayout.rowSize;
      maxColumns = MapGeneratorLayout.columnSize;
      minDistanceWall = MapGeneratorLayout.minDistanceWall;
      randomAnswersetNumber = MapGeneratorLayout.randomAnswersetNumber;
      encodingFolder = MapGeneratorLayout.encodingFolder;
      minRoomSize = MapGeneratorLayout.minRoomSize;
      pruningPercentage = MapGeneratorLayout.pruningPercentage;
      sameOrientationPercentage = MapGeneratorLayout.sameOrientationPercentage;
      MapGenerator.IS_DEBUG_MODE = MapGeneratorLayout.debug;

      if (MapGeneratorLayout.randomSeed == -1)
        randomGenerator = new System.Random();
      else
        randomGenerator = new System.Random(MapGeneratorLayout.randomSeed);

      matrixCells = CellManager.Instance;
      partitions = new List<Partition>();
      connections = new List<Connected8>();

    }

    public MapGenerator(int width, int height, int min_distance_wall, int random_answerset_number, string encodingFolder, int randomSeed) : base() {
      maxColumns = width;
      maxRows = height;
      minDistanceWall = min_distance_wall;
      randomAnswersetNumber = random_answerset_number;
      this.encodingFolder = encodingFolder + Path.DirectorySeparatorChar;
      matrixCells = CellManager.Instance;
      if (randomSeed >= 0)
        randomGenerator = new System.Random(randomSeed);
      else
        randomGenerator = new System.Random();
    }

    private NewDoor AnswerSetToCellMatrix(AnswerSets answers) {
      IList<AnswerSet> answerSetsList = answers.Answersets;

      NewDoor door = null;

      if (answerSetsList.Count > 0) {

        int index = randomGenerator.Next(answerSetsList.Count);
        AnswerSet a = answers.Answersets[index];

        try {
          foreach (object obj in a.Atoms) {
            if (obj is Cell)
              matrixCells.AddCell((Cell)obj);
            else if (obj is NewDoor)
              door = (NewDoor)obj;
          }
        }
        catch (Exception e) {
          UnityEngine.Debug.Log(e.ToString());
          UnityEngine.Debug.Log(e.StackTrace);
        }

      }
      return door;
    }

    public virtual void GenerateMap() {
      try {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        SetWallOnBorder();
        SpacePartitioning(true, new Partition(1, 1, maxRows, maxColumns));
        GeneratePartitionGraph();
        PartitioningTypeAssignment();

        if (IS_DEBUG_MODE)
          UnityEngine.Debug.Log(this.ToString());

        GenerateFloor();
        PartitionObjectTypeAssignment();

        stopwatch.Stop();
        long elapsedTime = stopwatch.Elapsed.Seconds;
        UnityEngine.Debug.Log("Elapsed Time: " + stopwatch.Elapsed.Minutes + " min, " + stopwatch.Elapsed.Seconds + " sec" + " ");
      }
      catch (Exception e) {
        Console.WriteLine(e.ToString());
        Console.Write(e.StackTrace);
      }

    }

    #region Program Flow
    private void SetWallOnBorder() {
      EmbASPManager controller = new EmbASPManager();
      controller.InitializeEmbASP();

      InputProgram input = controller.Input;
      Handler handler = controller.Handler;

      input.AddFilesPath(encodingFolder + "1-set_wall_on_border.asp");
      handler.AddProgram(input);

      InputProgram facts = new ASPInputProgram();
      facts.AddProgram("col(1.." + maxColumns + ").");
      facts.AddProgram("row(1.." + maxRows + ").");
      facts.AddProgram("max_col(" + maxColumns + ").");
      facts.AddProgram("max_row(" + maxRows + ").");

      handler.AddProgram(facts);


      Output o = handler.StartSync();
      AnswerSets answers = (AnswerSets)o;
      AnswerSetToCellMatrix(answers);
    }

    public void SpacePartitioning(bool horizontal, Partition nextPartitioned) {
      if (nextPartitioned.Size < minRoomSize) {
        AddPartition(nextPartitioned);
        return;
      }

      double rNumber = randomGenerator.NextDouble();
      //        if (rNumber < pruningPercentage * (1 - nextPartitioned.getSize() / mapSize)) {
      //        	addPartition(nextPartitioned);
      //        	return;
      //        }

      if (rNumber < sameOrientationPercentage)
        horizontal = !horizontal;

      EmbASPManager controller = new EmbASPManager();
      controller.InitializeEmbASP(randomAnswersetNumber);

      InputProgram input = controller.Input;
      Handler handler = controller.Handler;

      input.AddFilesPath(encodingFolder + "2-space_partitioning.asp");
      handler.AddProgram(input);

      InputProgram facts = new ASPInputProgram();

      int orientationIndex = (horizontal) ? 1 : 0;


      facts.AddProgram("row(" + nextPartitioned.getMinRow() + ".." + nextPartitioned.getMaxRow() + ").");
      facts.AddProgram("col(" + nextPartitioned.getMinCol() + ".." + nextPartitioned.getMaxCol() + ").");
      facts.AddProgram("max_row(" + nextPartitioned.getMaxRow() + ").");
      facts.AddProgram("max_col(" + nextPartitioned.getMaxCol() + ").");
      facts.AddProgram("min_row(" + nextPartitioned.getMinRow() + ").");
      facts.AddProgram("min_col(" + nextPartitioned.getMinCol() + ").");
      facts.AddProgram("min_distance_wall(" + minDistanceWall + ").");
      facts.AddProgram("orientation(" + orientation[orientationIndex] + ").");

      foreach (Cell cell in matrixCells.SetCells)
        if (cell.getRow() >= nextPartitioned.getMinRow() && cell.getRow() <= nextPartitioned.getMaxRow()
                && cell.getColumn() >= nextPartitioned.getMinCol()
                && cell.getColumn() <= nextPartitioned.getMaxCol()) {
          facts.AddObjectInput(cell);
        }


      handler.AddProgram(facts);

      Output o = handler.StartSync();
      AnswerSets answers = (AnswerSets)o;

      NewDoor door = AnswerSetToCellMatrix(answers, nextPartitioned);


      if (door != null) {
        if (door.getType().Equals("hdoor")) {
          SpacePartitioning(!horizontal, new Partition(nextPartitioned.getMinRow(), nextPartitioned.getMinCol(),
                  door.getRow(), nextPartitioned.getMaxCol()));
          SpacePartitioning(!horizontal, new Partition(door.getRow(), nextPartitioned.getMinCol(),
                  nextPartitioned.getMaxRow(), nextPartitioned.getMaxCol()));
        }
        else if (door.getType().Equals("vdoor")) {
          SpacePartitioning(!horizontal, new Partition(nextPartitioned.getMinRow(), nextPartitioned.getMinCol(),
                  nextPartitioned.getMaxRow(), door.getColumn()));
          SpacePartitioning(!horizontal, new Partition(nextPartitioned.getMinRow(), door.getColumn(),
                  nextPartitioned.getMaxRow(), nextPartitioned.getMaxCol()));
        }
      }
      else {

        AddPartition(nextPartitioned);

      }
    }

    private void GeneratePartitionGraph() {

      EmbASPManager controller = new EmbASPManager();
      controller.InitializeEmbASP(randomAnswersetNumber);

      InputProgram input = controller.Input;
      Handler handler = controller.Handler;

      input.AddFilesPath(encodingFolder + "3-partition_graph_generator.asp");
      handler.AddProgram(input);

      InputProgram facts = new ASPInputProgram();

      facts.AddProgram("col(1.." + maxColumns + ").");
      facts.AddProgram("row(1.." + maxRows + ").");

      foreach (Cell cell in matrixCells.SetCells)
        facts.AddObjectInput(cell);

      handler.AddProgram(facts);

      Output o = handler.StartSync();
      AnswerSets answers = (AnswerSets)o;


      IList<AnswerSet> answerSetsList = answers.Answersets;

      StringBuilder debugConnected = new StringBuilder();
      if (answerSetsList.Count > 0) {
        int index = randomGenerator.Next(answerSetsList.Count);
        AnswerSet a = answerSetsList[index];


        foreach (Object obj in a.Atoms) {
          if (obj is Connected8) {
            Connected8 connected8 = (Connected8)obj;
            connections.Add(connected8);
            debugConnected.Append(connected8.ToString() + "\n");
          }
        }
      }
      if (IS_DEBUG_MODE)
        UnityEngine.Debug.Log(debugConnected.ToString());
    }

    private void PartitioningTypeAssignment() {

      EmbASPManager controller = new EmbASPManager();
      controller.InitializeEmbASP(randomAnswersetNumber);

      InputProgram input = controller.Input;
      Handler handler = controller.Handler;

      input.AddFilesPath(encodingFolder + "4-partition_type_assignment.asp");
      handler.AddProgram(input);

      InputProgram facts = new ASPInputProgram();
      StringBuilder inputProgramString = new StringBuilder();

      facts.AddProgram("num_partitions(" + partitions.Count + ").");
      facts.AddProgram("empty_percentage_range(10,20).");
      facts.AddProgram("type(\"hollow\").");
      facts.AddProgram("type(\"empty\").");
      facts.AddProgram("type(\"corridor\").");

      inputProgramString.Append("num_partitions(" + partitions.Count + ").\n");
      inputProgramString.Append("empty_percentage_range(10,20).\n");
      inputProgramString.Append("type(\"hollow\").\n");
      inputProgramString.Append("type(\"empty\").\n");
      inputProgramString.Append("type(\"corridor\").\n");

      int start_partition_index = randomGenerator.Next(partitions.Count);
      Partition start_partition = partitions[start_partition_index];
      facts.AddProgram("start_partition(p(" + start_partition.getMinRow() + "," + start_partition.getMinCol() + "," +
                   +start_partition.getMaxRow() + "," + start_partition.getMaxCol() + ")).");

      inputProgramString.Append("start_partition(p(" + start_partition.getMinRow() + "," + start_partition.getMinCol() + "," +
                   +start_partition.getMaxRow() + "," + start_partition.getMaxCol() + ")).\n");


      foreach (Partition partition in partitions) {
        facts.AddProgram("partition(p(" + partition.getMinRow() + "," + partition.getMinCol() + "," + partition.getMaxRow() + "," + partition.getMaxCol() + ")).");
        inputProgramString.Append("partition(p(" + partition.getMinRow() + "," + partition.getMinCol() + "," + partition.getMaxRow() + "," + partition.getMaxCol() + ")).\n");
      }
      foreach (Connected8 connected8 in connections) {
        Partition partition1 = new Partition(connected8.getMinRow1(), connected8.getMinCol1(), connected8.getMaxRow1(), connected8.getMaxCol1());
        Partition partition2 = new Partition(connected8.getMinRow2(), connected8.getMinCol2(), connected8.getMaxRow2(), connected8.getMaxCol2());
        facts.AddProgram("connected(p(" + partition1.getMinRow() + "," + partition1.getMinCol() + "," + partition1.getMaxRow() + ","
                      + partition1.getMaxCol() + "),p(" + partition2.getMinRow() + "," + partition2.getMinCol() + "," +
                      partition2.getMaxRow() + "," + partition2.getMaxCol() + ")).");
        inputProgramString.Append("connected(p(" + partition1.getMinRow() + "," + partition1.getMinCol() + "," + partition1.getMaxRow() + ","
                              + partition1.getMaxCol() + "),p(" + partition2.getMinRow() + "," + partition2.getMinCol() + "," +
                              partition2.getMaxRow() + "," + partition2.getMaxCol() + ")).\n");

      }

      handler.AddProgram(facts);

      Output o = handler.StartSync();
      AnswerSets answers = (AnswerSets)o;

      IList<AnswerSet> answerSetsList = answers.Answersets;

      if (answerSetsList.Count > 0) {
        int index = randomGenerator.Next(answerSetsList.Count);
        AnswerSet a = answerSetsList[index];

        StringBuilder debugTypeAssignment = new StringBuilder();
        foreach (Object obj in a.Atoms) {

          if (obj is Assignment) {

            Assignment assignment = (Assignment)obj;

            debugTypeAssignment.Append(assignment.ToString() + "\n");
            Partition partition = new Partition(assignment.getMinRow(), assignment.getMinCol(), assignment.getMaxRow(), assignment.getMaxCol());

            int partition_index = partitions.IndexOf(partition);
            partitions[partition_index].Type = assignment.getType();

            if (assignment.getType().Equals("\"empty\""))
              numEmptyPartitions++;
            else
              numPartitionsToBuild++;
          }
        }
        if (IS_DEBUG_MODE) {
          UnityEngine.Debug.Log("ASSIGNMENTS: \n" + debugTypeAssignment.ToString());
          UnityEngine.Debug.Log("INPUT PROGRAM: " + inputProgramString.ToString());
          UnityEngine.Debug.Log("OUTPUT: " + o.OutputString + "\nERROR: " + o.ErrorsString);
        }
      }
    }

    public void GenerateFloor() {

      try {
        List<RoomBuilder> buildersEmpty = new List<RoomBuilder>();
        List<RoomBuilder> buildersRoom = new List<RoomBuilder>();
        Barrier emptyBarrier = new Barrier(numEmptyPartitions + 1);
        Barrier toBuildBarrier = new Barrier(numPartitionsToBuild + 1);
        for (int i = 0; i < partitions.Count; i++) {
          RoomBuilder roomBuilder;
          if (partitions[i].Type.Equals("\"empty\"")) {
            roomBuilder = new RoomBuilder(this, partitions[i], emptyBarrier);
            buildersEmpty.Add(roomBuilder);
          }
          else {
            roomBuilder = new RoomBuilder(this, partitions[i], toBuildBarrier);
            buildersRoom.Add(roomBuilder);
          }
        }

        foreach (RoomBuilder rb in buildersEmpty)
          rb.Start();
        emptyBarrier.SignalAndWait();

        foreach (RoomBuilder rb in buildersRoom)
          rb.Start();
        toBuildBarrier.SignalAndWait();
      }
      catch (Exception e) {
        UnityEngine.Debug.Log("MAP GENERATOR BARRIER: \nBARRIER EXCEPTION MESSAGE: " + e.Message + "\nBARRIER EXCEPTION: " + e.StackTrace);
      }
    }

    private void PartitionObjectTypeAssignment() {

      EmbASPManager controller = new EmbASPManager();
      controller.InitializeEmbASP(randomAnswersetNumber);

      InputProgram input = controller.Input;
      Handler handler = controller.Handler;

      input.AddFilesPath(encodingFolder + "6-partition_object_type_assignment.asp");
      handler.AddProgram(input);

      InputProgram facts = new ASPInputProgram();

      facts.AddProgram("maximum_locked_door_number(5).");
      facts.AddProgram("object_id(1..5).");
      facts.AddProgram("type(\"avatar\").");
      facts.AddProgram("type(\"goal\").");
      facts.AddProgram("type(\"none\").");
      facts.AddProgram("type(\"obstacle\").");
      facts.AddProgram("type(\"key\").");
      facts.AddProgram("type(\"locked\").");


      foreach (Connected8 connected8 in connections) {
        Partition partition1 = new Partition(connected8.getMinRow1(), connected8.getMinCol1(), connected8.getMaxRow1(), connected8.getMaxCol1());
        Partition partition2 = new Partition(connected8.getMinRow2(), connected8.getMinCol2(), connected8.getMaxRow2(), connected8.getMaxCol2());
        facts.AddProgram("connected(p(" + partition1.getMinRow() + "," + partition1.getMinCol() + "," + partition1.getMaxRow() + ","
                + partition1.getMaxCol() + "),p(" + partition2.getMinRow() + "," + partition2.getMinCol() + "," +
                partition2.getMaxRow() + "," + partition2.getMaxCol() + ")).");
      }

      foreach (Partition partition in partitions) {
        facts.AddProgram("assignment(p(" + partition.getMinRow() + "," + partition.getMinCol() + "," + partition.getMaxRow() + ","
        + partition.getMaxCol() + ")" + "," + partition.Type + ").");
      }

      handler.AddProgram(facts);


      Output o = handler.StartSync();
      AnswerSets answers = (AnswerSets)o;

      IList<AnswerSet> answerSetsList = answers.Answersets;

      if (answerSetsList.Count > 0) {
        int index = randomGenerator.Next(answerSetsList.Count);
        AnswerSet a = answerSetsList[index];



        StringBuilder debugObjectAssignment = new StringBuilder();

        foreach (Object obj in a.Atoms) {

          if (obj is ObjectAssignment) {
            ObjectAssignment objectAssignment = (ObjectAssignment)obj;
            debugObjectAssignment.Append(objectAssignment.ToString() + "\n");
            Partition partition = new Partition(objectAssignment.getMinRow(), objectAssignment.getMinCol(), objectAssignment.getMaxRow(), objectAssignment.getMaxCol());
            int partition_index = partitions.IndexOf(partition);
            partitions[partition_index].Type = objectAssignment.getType();
          }
        }
        if (IS_DEBUG_MODE)
          UnityEngine.Debug.Log(debugObjectAssignment.ToString());
      }
    }

    #endregion

    #region Utility Functions
    public void AddPartition(Partition nextPartitioned) {
      partitions.Add(nextPartitioned);
      Pair<int, int> door;

      for (int i = nextPartitioned.getMinRow(); i <= nextPartitioned.getMaxRow(); i++) {

        door = new Pair<int, int>(i, nextPartitioned.getMinCol());
        if (matrixCells.Cells[door].getType().Equals("vdoor"))
          nextPartitioned.Doors.Add(door);

        door = new Pair<int, int>(i, nextPartitioned.getMaxCol());
        if (matrixCells.Cells[door].getType().Equals("vdoor"))
          nextPartitioned.Doors.Add(door);
      }

      for (int i = nextPartitioned.getMinCol(); i <= nextPartitioned.getMaxCol(); i++) {

        door = new Pair<int, int>(nextPartitioned.getMinRow(), i);
        if (matrixCells.Cells[door].getType().Equals("hdoor"))
          nextPartitioned.Doors.Add(door);

        door = new Pair<int, int>(nextPartitioned.getMaxRow(), i);
        if (matrixCells.Cells[door].getType().Equals("hdoor"))
          nextPartitioned.Doors.Add(door);
      }
    }

    private NewDoor AnswerSetToCellMatrix(AnswerSets answers, Partition oldRoom) {
      IList<AnswerSet> answerSetsList = answers.Answersets;

      NewDoor door = null;

      if (answerSetsList.Count > 0) {
        int index = randomGenerator.Next(answerSetsList.Count);
        AnswerSet a = answerSetsList[index];

        foreach (Object obj in a.Atoms)
          if (obj is Cell)
            matrixCells.AddCell((Cell)obj);
          else if (obj is NewDoor)
            door = (NewDoor)obj;
      }
      return door;

    }


    public void BuildWalls(Partition partition) {

      //if (partition.Size >= 200 && !partition.Type.Equals("\"empty\"") && partition.Type.Equals("\"hollow\""))
      //  partition.Type = "\"corridor\"";

      EmbASPManager controller = new EmbASPManager();
      controller.InitializeEmbASP(randomAnswersetNumber);

      InputProgram input = controller.Input;
      Handler handler = controller.Handler;

      switch (partition.Type) {
        case "\"hollow\"":
          input.AddFilesPath(encodingFolder + "5-generate_room.asp");
          break;
        case "\"corridor\"":
          input.AddFilesPath(encodingFolder + "5-generate_corridor.asp");
          break;
        case "\"empty\"":
          input.AddFilesPath(encodingFolder + "5-generate_empty.asp");
          break;
        default:
          break;
      }

      handler.AddProgram(input);

      InputProgram facts = new ASPInputProgram();


      StringBuilder debugBuildWall = new StringBuilder();
      debugBuildWall.Append("col(" + partition.getMinCol() + ".." + partition.getMaxCol() + ").");
      debugBuildWall.Append("row(" + partition.getMinRow() + ".." + partition.getMaxRow() + ").");
      debugBuildWall.Append("min_row(" + partition.getMinRow() + ").");
      debugBuildWall.Append("max_row(" + partition.getMaxRow() + ").");
      debugBuildWall.Append("min_col(" + partition.getMinCol() + ").");
      debugBuildWall.Append("max_col(" + partition.getMaxCol() + ").");

      facts.AddProgram("col(" + partition.getMinCol() + ".." + partition.getMaxCol() + ").");
      facts.AddProgram("row(" + partition.getMinRow() + ".." + partition.getMaxRow() + ").");
      facts.AddProgram("min_row(" + partition.getMinRow() + ").");
      facts.AddProgram("max_row(" + partition.getMaxRow() + ").");
      facts.AddProgram("min_col(" + partition.getMinCol() + ").");
      facts.AddProgram("max_col(" + partition.getMaxCol() + ").");

      foreach (Pair<int, int> door_coordinate in partition.Doors) {
        if (partition.Type.Equals("\"empty\"")) {
          matrixCells.Cells[door_coordinate].setType("wall");
        }
        else {
          Cell door = matrixCells.Cells[door_coordinate];
          facts.AddProgram("cell(" + door.getRow() + "," + door.getColumn() + ",\"" + door.getType() + "\").");
          debugBuildWall.Append("cell(" + door.getRow() + "," + door.getColumn() + ",\"" + door.getType() + "\").");

        }
      }

      if (IS_DEBUG_MODE)
        UnityEngine.Debug.Log("INPUT BUILDWALLS: \n" + debugBuildWall.ToString());

      handler.AddProgram(facts);


      Output o = handler.StartSync();
      AnswerSets answers = (AnswerSets)o;


      AnswerSetToCellMatrix(answers, null);
    }

    #endregion



    public override string ToString() {
      StringBuilder builder = new StringBuilder();
      for (int i = 1; i <= maxRows; i++) {
        for (int j = 1; j <= maxColumns; j++) {
          if (matrixCells.Cells.ContainsKey(new Pair<int, int>(i, j))) {
            builder.Append(matrixCells.Cells[new Pair<int, int>(i, j)].GVGAI);
          }
          else {
            builder.Append("- ");
          }
        }
        builder.Append("\n");
      }

      builder.Append("PARTITIONS NUMBER: " + partitions.Count + "\n");

      for (int i = 0; i < partitions.Count; i++) {
        Partition partition = partitions[i];
        builder.Append("Partition " + i + " " + partition.getMinRow() + " " + partition.getMaxRow() + " " + partition.getMinCol() + " " + partition.getMaxCol() + "\n");
        for (int j = 0; j < partition.Doors.Count; j++) {
          Pair<int, int> door_coordinate = partition.Doors[j];
          Cell door = matrixCells.Cells[door_coordinate];
          builder.Append(j + 1 + " DOOR: " + door.getRow() + " " + door.getColumn() + " " + door.GVGAI + " " + door.getType() + "\n");
        }
        builder.Append("\n");
      }

      return builder.ToString() + "\n\nMapGenerator [width=" + maxColumns + ", height=" + maxRows + ", min_distance_wall=" + minDistanceWall + ", random_answerset_number=" + randomAnswersetNumber + "]\n";
    }
  }
}
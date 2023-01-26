using AIMapGenerator.it.unical.mat;
using it.unical.mat.asp_classes;
using it.unical.mat.map_generator;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AIMapGenerator {
  class MapGeneratorLayout : EditorWindow {

    #region Configuration File Parameters
    #region Min and Max Values
    readonly int minSizeMatrix = 10;
    readonly int maxSizeMatrix = 150;
    readonly int buttonWidth = 160;
    #endregion
    #region Matrix Size Settings
    public static int rowSize = 10;
    public static int columnSize = 10;
    #endregion
    #region Random
    public static int randomSeed = 1;
    public static int randomAnswersetNumber = 1;
    #endregion
    #region General Settings
    public static string encodingFolder = @"Assets\encodings\";
    public static string solver = @"Assets\solvers\clingo.exe";
    public static int minRoomSize = 50;
    public static int minDistanceWall = 3;
    public static float pruningPercentage = 0.05f;
    public static float sameOrientationPercentage = 0.20f;
    #endregion
    #region DEBUG VARIABLE
    public static bool debug = false;
    #endregion
    private List<GameObject> mapTiles = new List<GameObject>();

    #endregion


    [MenuItem("AIMapGenerator/AIMapGenerator")]
    public static void ShowWindow() {
      EditorWindow.GetWindow(typeof(MapGeneratorLayout));
      encodingFolder = Application.dataPath + @"/encodings/";
      solver = Application.dataPath + @"/solvers/clingo.exe";
    }

    void OnGUI() {

      GUILayout.Label("Matrix Size Settings", EditorStyles.boldLabel);
      rowSize = EditorGUILayout.IntSlider("Row Size", rowSize, minSizeMatrix, maxSizeMatrix);
      columnSize = EditorGUILayout.IntSlider("Column Size", columnSize, minSizeMatrix, maxSizeMatrix);

      GUILayout.Label("General Settings", EditorStyles.boldLabel);
      GUILayout.BeginHorizontal();
      encodingFolder = EditorGUILayout.TextField("Encoding folder: ", encodingFolder);
      if (GUILayout.Button("Browse")) {
        encodingFolder = EditorUtility.OpenFolderPanel("Select encoding folder", "", "").Replace("/", "\\");
        encodingFolder = encodingFolder + "\\";
      }
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      solver = EditorGUILayout.TextField("Solver: ", solver);
      if (GUILayout.Button("Browse")) {
        solver = EditorUtility.OpenFilePanel("Select solver", "", "").Replace("/", "\\");
      }
      GUILayout.EndHorizontal();


      minRoomSize = EditorGUILayout.IntSlider("minRoomSize", minRoomSize, 0, rowSize * columnSize);
      minDistanceWall = EditorGUILayout.IntSlider("Distance from wall", minDistanceWall, 0, 15);
      pruningPercentage = EditorGUILayout.Slider("Pruning percentage", pruningPercentage, 0, 1);
      sameOrientationPercentage = EditorGUILayout.Slider("Orientation Percentage", sameOrientationPercentage, 0, 1);


      GUILayout.Label("Randomicity", EditorStyles.boldLabel);
      randomSeed = EditorGUILayout.IntField("Seed:", randomSeed);
      randomAnswersetNumber = EditorGUILayout.IntField("AnswerSet Number:", randomAnswersetNumber);


      GUILayout.Label("DEBUG MODE", EditorStyles.boldLabel);
      debug = GUILayout.Toggle(debug, "Debug Mode");

      GUILayout.Label("Generator", EditorStyles.boldLabel);


      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Space(10);
      if (GUILayout.Button("Reset", GUILayout.Width(buttonWidth)))
        restoreDefaultValues();
      GUILayout.Space(10);
      if (GUILayout.Button("Generate", GUILayout.Width(buttonWidth))) {

        RepaintScene();
      }

      GUILayout.Space(10);
      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();

    }

    private void RepaintScene() {

      // Repaint in new EditorWindow
      MapGenerator map = new MapGenerator();
      map.GenerateMap();

      // Repaint in new Console
      Debug.Log(map.ToString());

      // Clear old SceneView
      foreach (GameObject go in mapTiles)
        DestroyImmediate(go);
      mapTiles.Clear();

      // Repaint in new SceneView
      GridWindow window = (GridWindow)EditorWindow.GetWindow(typeof(GridWindow));
      window.Init(mapTiles);

      // Clear old Map
      CellManager.Instance.Reset();
    }

    #region Utility Functions
    private void restoreDefaultValues() {
      rowSize = 10;
      columnSize = 10;
      randomSeed = 1;
      randomAnswersetNumber = 1;
      encodingFolder = Application.dataPath + @"/encodings/";
      solver = Application.dataPath + @"/solvers/clingo.exe";
      minRoomSize = 50;
      minDistanceWall = 3;
      pruningPercentage = 0.05f;
      sameOrientationPercentage = 0.20f;
      debug = false;
    }
    #endregion
  }

}
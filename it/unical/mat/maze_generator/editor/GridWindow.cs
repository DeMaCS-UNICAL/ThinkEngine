using it.unical.mat.asp_classes;
using it.unical.mat.map_generator;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;

namespace AIMapGenerator.it.unical.mat {
  public class GridWindow : EditorWindow {

    int maxRow = 10;
    int maxCol = 10;
    private string[,] matrix = new string[10, 10];
    int cubeSize = 10;
    private List<GameObject> mapTiles;
    float zoomScale = 1.0f;
    Vector2 vanishingPoint = new Vector2(0, 21);
    Vector2 scrollPosition = Vector2.zero;


    public void Init(List<GameObject> tiles) {

      mapTiles = tiles;
      CellManager matrixCells = CellManager.Instance;

      if (matrixCells.allCells.Count > 0) {
        maxRow = matrixCells.allCells.Max(c => c.getRow());
        maxCol = matrixCells.allCells.Max(c => c.getColumn());
        matrix = new string[maxRow, maxCol];
      }

      Debug.Log("Row: " + maxRow + " Col: " + maxCol);

      foreach (Cell cell in matrixCells.allCells) {
        if (cell.getType().Equals("wall"))
          matrix[cell.getRow() - 1, cell.getColumn() - 1] = "w";
        else if (cell.getType().Equals("vdoor"))
          matrix[cell.getRow() - 1, cell.getColumn() - 1] = "|";
        else if (cell.getType().Equals("hdoor"))
          matrix[cell.getRow() - 1, cell.getColumn() - 1] = "=";
      }

      DrawScene();
    }

    private void DrawScene() {
      for (int i = 0; i < maxRow; i++)
        for (int j = 0; j < maxCol; j++)
          if (matrix[i, j] != null && matrix[i, j].Equals("w"))
            CreateCube(new Vector3(i * cubeSize * 1.2f, 0, j * cubeSize * 1.2f), Color.black, cubeSize * 3);
          else {
            CreateCube(new Vector3(i * cubeSize * 1.2f, 0, j * cubeSize * 1.2f), Color.green, cubeSize);
          }
    }

    private GameObject CreateCube(Vector3 position, Color color, int height) {
      GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
      cube.transform.localScale = new Vector3(cubeSize, height, cubeSize);
      cube.transform.position = position;
      mapTiles.Add(cube);

      Renderer renderer = cube.GetComponent<Renderer>();
      if (renderer != null) {
        var tempMaterial = new Material(renderer.sharedMaterial);
        tempMaterial.color = color;
        renderer.sharedMaterial = tempMaterial;

        //renderer.material.color = color;
      }
      return cube;
    }

    void OnGUI() {

      Matrix4x4 oldMatrix = GUI.matrix;

      //Scale my gui matrix
      Matrix4x4 Translation = Matrix4x4.TRS(vanishingPoint, Quaternion.identity, Vector3.one);
      Matrix4x4 Scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
      GUI.matrix = Translation * Scale * Translation.inverse;

      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < maxRow; i++)
        for (int j = 0; j < maxCol; j++)
          if (matrix[i, j] != null && matrix[i, j].Equals("w"))
            EditorGUI.DrawRect(new Rect(j * cubeSize * 1.2f, i * cubeSize * 1.2f, cubeSize, cubeSize), Color.black);
          else
            EditorGUI.DrawRect(new Rect(j * cubeSize * 1.2f, i * cubeSize * 1.2f, cubeSize, cubeSize), Color.green);

      //reset the matrix
      GUI.matrix = oldMatrix;

      // Just for testing (unscaled controls at the bottom)
      GUILayout.FlexibleSpace();
     
      vanishingPoint = EditorGUILayout.Vector2Field("Vanishing point", vanishingPoint);
      zoomScale = EditorGUILayout.Slider("Zoom", zoomScale, 1.0f / 25.0f, 2.0f);
    }
  }
}

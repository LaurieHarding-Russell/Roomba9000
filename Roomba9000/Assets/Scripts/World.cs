﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct IntVector2 {
    public IntVector2 (int xpos, int ypos) {
        this.x = xpos;
        this.y = ypos;
    }

    public int x;
    public int y;
}


public class World : MonoBehaviour
{

    // FIXME, enum?
    const int NOTHING = 0;
    const int WALL = 1;
    const int CARPET = 2;

    public static int MAX_SIZE = 50;
    private int NOTHING_RATE = 55; 
    private int NOTHING_RATE_OUT_OF = 100;

    public static float TILE_SIZE = 1;

    // Start is called before the first frame update
    void Start()
    {
        int[,] map = generateRoomIslands(MAX_SIZE);
        createMesh(map);
    }

    void OnDrawGizmos() {
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawSphere(transform.position, 0.5f);
    }

    private int[,] generateRoomIslands(int size) {
        int startingCorner = (MAX_SIZE - size) / 2;
        int endCorner = startingCorner + size;

        int[,] map = new int[MAX_SIZE, MAX_SIZE]; 

        Stack<IntVector2> islandPoints = new Stack<IntVector2>();

        IntVector2 center = new IntVector2(size/2, size/2);

        IntVector2 playerIslandCenter = center;
        IntVector2 playerIslandLeft = new IntVector2(center.x - 1, center.y);
        IntVector2 playerIslandRight = new IntVector2(center.x + 1, center.y);
        IntVector2 playerIslandTop = new IntVector2(center.x, center.y + 1);
        IntVector2 playerIslandBottom = new IntVector2(center.y, center.y -1); 
        islandPoints.Push(playerIslandCenter);
        islandPoints.Push(playerIslandLeft);
        islandPoints.Push(playerIslandRight);
        islandPoints.Push(playerIslandTop);
        islandPoints.Push(playerIslandBottom);

        for (int i = 0; i < 5; i++) {
            // islandPoint;
            int x = Random.Range(1, size -1);
            int y = Random.Range(1, size -1 );
            IntVector2 island = new IntVector2(x, y);
            islandPoints.Push(island);
        }
 
        while (islandPoints.Count > 0) {
            IntVector2 current = islandPoints.Pop();
            map[current.x, current.y] = CARPET;

            IntVector2 currentTop = new IntVector2(current.x, current.y +1);
            IntVector2 currentBottom = new IntVector2(current.x, current.y -1);
            IntVector2 currentRight = new IntVector2(current.x + 1, current.y);
            IntVector2 currentLeft = new IntVector2(current.x - 1, current.y);

            createChildrenLand(map, islandPoints, currentTop);
            createChildrenLand(map, islandPoints, currentBottom);
            createChildrenLand(map, islandPoints, currentRight);
            createChildrenLand(map, islandPoints, currentLeft);
        }

        return map;
    }

    private void createChildrenLand(int[,] map, Stack<IntVector2> islandPoints, IntVector2 child) {
        int nothing = Random.Range(0,NOTHING_RATE_OUT_OF);
        if (inWorld(child) && nothing > NOTHING_RATE) {
            if (map[child.x, child.y] != CARPET){
                islandPoints.Push(child);
            }
        }
    }

    private bool inWorld(IntVector2 vector) {
        return vector.x > 0 && vector.x < MAX_SIZE
            && vector.y > 0 && vector.y < MAX_SIZE;
    }

    private void createMesh(int[,] map) {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        List<Vector3> vertArray = new List<Vector3>();
        List<Vector2> meshUV = new List<Vector2>();
        List<int> triangles = new List<int>();
        string output = "";
        if (map != null) {
            for (int x = 0; x < MAX_SIZE; x++) {
                for (int z = 0; z < MAX_SIZE; z++) {
                    if (map[x,z] == CARPET) {
                        generateSquare(x, z, vertArray, triangles, meshUV);
                    } else if (map[x,z] == WALL) {
                        generateSquare(x, z, vertArray, triangles, meshUV);
                        // generateWall(x, z, vertArray, triangles);
                    }
                    output += map[x,z];
                }
                output += '\n';
            }
        }
        // Debug.Log(output);

        Material material = GetComponent<Renderer>().material;  
        Texture2D texture = generateTexture(50, Color.yellow);
        material.mainTextureScale = new Vector2(0.02f, 0.02f);
        material.mainTexture = texture;

        mesh.vertices = vertArray.ToArray();
        mesh.uv = meshUV.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.name = "terrain"; 

        MeshCollider meshCollider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        mesh.RecalculateBounds();
        meshCollider.sharedMesh = mesh;
        
    }

    private void generateSquare(int x, int z, List<Vector3> vertArray, List<int> triangles, List<Vector2> meshUV) {
        int start = vertArray.Count;

        float offset = (TILE_SIZE * MAX_SIZE /2);

        float xPos = x * TILE_SIZE - offset;
        float zPos = z * TILE_SIZE - offset;

        vertArray.Add(new Vector3(xPos, 0, zPos));
        vertArray.Add(new Vector3(xPos + TILE_SIZE, 0, zPos));
        vertArray.Add(new Vector3(xPos, 0, zPos + TILE_SIZE));
        vertArray.Add(new Vector3(xPos + TILE_SIZE, 0, zPos + TILE_SIZE));

        triangles.Add(start);
        triangles.Add(start + 2);
        triangles.Add(start + 1);
        meshUV.Add(new Vector2(0.0f, 0.0f));
        meshUV.Add(new Vector2(1.0f, 0.0f)); 
        meshUV.Add(new Vector2(1.0f, 1.0f));
        meshUV.Add(new Vector2(0.0f, 1.0f)); 

        triangles.Add(start + 2);
        triangles.Add(start + 3);
        triangles.Add(start + 1);
    }

    private Texture2D generateTexture(int size, Color colour) {
        Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                float red = Random.Range(0.0f, 0.5f);
                float green = Random.Range(0.0f, 0.5f);
                float blue = Random.Range(0.0f, 0.5f);
                Color offsetColour = new Color(red, green, blue, 1.0f);
                texture.SetPixel(0, 0, colour - offsetColour);
            }
        }
    
        texture.Apply();
        return texture;
    }
}

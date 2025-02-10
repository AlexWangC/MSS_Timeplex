using System;
using UnityEngine;

public class scrTileGenManager : MonoBehaviour
{
    public GameObject[] TilePool;
    public GameObject[] TilePool1;
    public GameObject[] TilePool2;
    public GameObject[] TilePool3;
    private scrPanel[] panels; // got it.
    
    private void Start()
    {
        printPanelsByTimeIndexes(sortPanelsByTimeIndex(findPanels()));
        panels = sortPanelsByTimeIndex(findPanels());
        
        //testing spawn object on grid
        fillUpAllPanelsEnhanced();
        //fillUpAllPanels();
    }
    
    private void fillUpAllPanelsEnhanced() // shitty code but im overworked
    {
        int panel_count = panels.Length;
        
        switch (panel_count)
        {
            case 1:
                fillupAPanel(panels[0], TilePool);
                break;
            
            case 2:
                fillupAPanel(panels[0], TilePool);
                fillupAPanel(panels[1], TilePool1);
                break;
            
            case 3:
                fillupAPanel(panels[0], TilePool);
                fillupAPanel(panels[1], TilePool1);
                fillupAPanel(panels[2], TilePool2);
                break;
            
            case 4:
                fillupAPanel(panels[0], TilePool);
                fillupAPanel(panels[1], TilePool1);
                fillupAPanel(panels[2], TilePool2);
                fillupAPanel(panels[3], TilePool3);
                break;
        }
    }
    
    
    private void fillUpAllPanels()
    {
        foreach (scrPanel panel in panels)
        {
            fillupAPanel(panel);
        }
    }

    private void fillupAPanel(scrPanel target_panel, GameObject[] tile_pool)
    {
        Vector2 dimensions = new Vector2(target_panel.GetComponentInChildren<scrGridMakerTilted>().numBlocksX, target_panel.GetComponentInChildren<scrGridMakerTilted>().numBlocksY);
        //filling up grid row by row
        for (int i = 1; i <= dimensions.y; i++) // iterating through each column
        {
            for (int j = 1; j <= dimensions.x; j++) // iterating through each row
            {
                spawnObjectOnGrid(target_panel, new Vector2(j, i), pickRandomTile(tile_pool));
            }
        }
    }
    
    private void fillupAPanel(scrPanel target_panel)
    {
        Vector2 dimensions = target_panel.GetComponentInChildren<GridMaker>().dimensions;
        //filling up grid row by row
        for (int i = 1; i <= dimensions.y; i++) // iterating through each column
        {
            for (int j = 1; j <= dimensions.x; j++) // iterating through each row
            {
                spawnObjectOnGrid(target_panel, new Vector2(j, i), pickRandomTile());
            }
        }
        
    }
    
    private GameObject pickRandomTile(GameObject[] tile_array) // pick random tile from a certain array
    {
        if (tile_array == TilePool)
        {
            int random = UnityEngine.Random.Range(0, TilePool.Length); // first arg inclusive, sec arg exclusive
            return TilePool[random];
        } if (tile_array == TilePool1)
        {
            int random = UnityEngine.Random.Range(0, TilePool1.Length); // first arg inclusive, sec arg exclusive
            return TilePool1[random];
        } if (tile_array == TilePool2)
        {
            int random = UnityEngine.Random.Range(0, TilePool2.Length); // first arg inclusive, sec arg exclusive
            return TilePool2[random];
        } if (tile_array == TilePool3)
        {
            int random = UnityEngine.Random.Range(0, TilePool3.Length); // first arg inclusive, sec arg exclusive
            return TilePool3[random];
        }

        return null;
    }
    
    private GameObject pickRandomTile() // pick up random tile from TilePool
    {
        int rand = UnityEngine.Random.Range(0, TilePool.Length); // first arg inclusive, sec arg exclusive
        return TilePool[rand];
    }
    
    private void spawnObjectOnGrid(scrPanel target_panel, Vector2 coordinate, GameObject tile)
    {
        GameObject new_tile = Instantiate(tile, new Vector3(0, 0, 0), Quaternion.identity);
        new_tile.transform.SetParent(target_panel.transform);
        new_tile.GetComponent<GridObject>().gridPosition = coordinate;
        new_tile.GetComponent<SpriteRenderer>().sortingLayerName = "tiles";
    }
    
    #region panels
    
    private scrPanel[] findPanels()
    {
        return FindObjectsByType<scrPanel>(FindObjectsSortMode.None);
    }

    private scrPanel[] sortPanelsByTimeIndex(scrPanel[] panels_local)
    {
        int n = panels_local.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (panels_local[j].Time_index > panels_local[j + 1].Time_index)
                {
                    // Swap panels_local[j] and panels_local[j + 1]
                    scrPanel temp = panels_local[j];
                    panels_local[j] = panels_local[j + 1];
                    panels_local[j + 1] = temp;
                }
            }
        }
        return panels_local;
    }
    
    private void printPanelsByTimeIndexes(scrPanel[] panels_local)
    {
        int index = 0;
        foreach (scrPanel panel_local in panels_local)
        {
            Debug.Log(index + "th panel's time index is: " + panel_local.Time_index);
            index++;
        }
    }
    
    #endregion
}

// I put it under room, it is going to find each 
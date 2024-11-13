using System.Collections;
using System.Collections.Generic;

public class Grid
{
    private Cell[,] grid;
    /// <summary>
    /// Create a new Grid of size width * height, and populate it with empty Cells
    /// </summary>
    public Grid(int width, int height)
    {
        grid = new Cell[width,height];
        //Populate grid with empty cells
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                grid[x, y] = new Cell(x, y, CoordsToID(x, y));
            }
        }
        //Determine cell neighbors
        //Array of directions, corresponding to N/S/E/W
        int[,] directions = new int[,] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                List<Cell> neighbors = new List<Cell>();
                //Iterate through each direction, adding to current position, and adding neighbor if within grid bounds
                for(int i = 0; i < 4; i++)
                {
                    int x1 = x + directions[i, 0];
                    int y1 = y + directions[i, 1];
                    if(x1 >= 0 && x1 < width && y1 >= 0 && y1 < height)
                    {
                        neighbors.Add(grid[x1,y1]);
                    }
                }
                //Add neighbor list to grid cell
                grid[x, y].SetNeighbors(neighbors);
            }
        }
    }
    /// <summary>
    /// Converts the given X and Y coordinate into a unique ID number for the Cell at that
    /// position
    /// </summary>
    private int CoordsToID(int x, int y)
    {
        return x + y * grid.GetLength(0);
    }
    public int GetWidth() { return grid.GetLength(0); }
    public int GetHeight() { return grid.GetLength(1); }
    /// <summary>
    /// Returns the Cell at the given coordinates, or null if the coordinates are outside of
    /// the Grid
    /// </summary>
    public Cell GetCell(int x, int y)
    {
        if(x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            return grid[x, y];
        }
        return null;
    }
    /// <summary>
    /// Sets the Building for the Cell at positon X, Y. Does nothing if the given coordinates
    /// are outside of the Grid
    /// </summary>
    public void SetCellBuilding(int x, int y, Building b)
    {
        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            grid[x, y].SetBuilding(b);
        }
    }

}
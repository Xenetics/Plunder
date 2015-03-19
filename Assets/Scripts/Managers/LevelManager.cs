using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LevelManager : MonoBehaviour 
{
    private static LevelManager instance = null;
    public static LevelManager Instance { get { return instance; } }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        
    }

    void Start()
    {
        rooms = new List<Room>();
        BuildRooms(); // builds the room with no objects
    }

    bool filled = false;

    void LateUpdate()
    {
        if(!filled)
        {
            filled = true;
            BuildLayout(diff * diff); // add layout to last room made
        }
    }

    public class Room
    {
        public GameObject RoomContainer;
        public int width;
        public int height;
        public int tileCount;
        public GameObject[] floor;
        public GameObject[] layout;
    }

    [SerializeField]
    private GameObject[] objects;

    private List<Room> rooms;

    private float floorLevel = 0;
    private float objectLevel = 1;
    private float obstacleLevel = 1.5f;
    private int diff = 20;
    private int obstacleForEvery = 4; // 1 obstacle spawns for every 4 tiles of floor showing
    private int chestForEvery = 20; // 1 chest spawns for every 20 tiles of floor showing

    private void BuildRooms()
    {
        Room newRoom = BuildLevel(diff);
        rooms.Add(newRoom);
    }

    private Room BuildLevel(int difficulty)
    {
        Room newRoom = new Room();
        newRoom.RoomContainer = new GameObject();
        newRoom.RoomContainer.name = "RoomContainer_" + difficulty;
        newRoom.RoomContainer.transform.position = Vector3.zero;
        newRoom.width = difficulty;
        newRoom.height = difficulty;
        newRoom.tileCount = newRoom.width * newRoom.height;
        newRoom.floor = BuildFloor(newRoom.tileCount, newRoom.RoomContainer);
        newRoom.layout = BuildWalls(newRoom.tileCount, newRoom.RoomContainer);

        return newRoom;
    }

    private GameObject[] BuildFloor(int tileCount, GameObject container)
    {
        GameObject[] floor = new GameObject[tileCount];

        float tileCountSquared = Mathf.Sqrt(tileCount);
        Vector3 spawn = new Vector3(-tileCountSquared * 0.5f, floorLevel, tileCountSquared * 0.5f);
        
        for (int i = 0; i < tileCount; i++ )
        {
            floor[i] = AddItem('F');
            floor[i].transform.position = spawn;
            floor[i].transform.parent = container.transform;
            spawn = IncrementSpawn(spawn, tileCountSquared);
        }
        return floor;
    }

    private GameObject[] BuildWalls(int tileCount, GameObject container)
    {
        GameObject[] border = new GameObject[tileCount];

        float tileCountSquared = Mathf.Sqrt(tileCount);
        Vector3 spawn = new Vector3(-tileCountSquared * 0.5f, obstacleLevel, tileCountSquared * 0.5f);

        for (int i = 0; i < tileCount; i++) // adds walls
        {
            bool addWall = false;

            if ((i - tileCountSquared) < 0)
            {
                addWall = true;
            }
            else if ((i % tileCountSquared) == 0)
            {
                addWall = true;
            }
            else if(((i + 1) % tileCountSquared * 0.5f) == 0)
            {
                addWall = true;
            }
            else if ((i + tileCountSquared) >= tileCount)
            {
                addWall = true;
            }
            
            if (addWall)
            {
                border[i] = AddItem('W');
                border[i].transform.position = spawn;
                border[i].transform.parent = container.transform;
            }
            spawn = IncrementSpawn(spawn, tileCountSquared);
        }

        return border;
    }

    private void BuildLayout(int tileCount)
    {
        float tileCountSquared = Mathf.Sqrt(tileCount);

        float floorCount = (tileCount - (tileCountSquared * 2) - ((tileCountSquared - 2.0f) * 2));
        float obstacleCount = floorCount / obstacleForEvery;
        float chestCount = floorCount / chestForEvery;

        Vector3 spawn = new Vector3(-tileCountSquared * 0.5f, obstacleLevel, tileCountSquared * 0.5f);
        Vector3 aheadSpawn = new Vector3(spawn.x, spawn.y, spawn.z + 1);
        Vector3 behindSpawn = new Vector3(spawn.x, spawn.y, spawn.z - 1);
        Vector3 rightSpawn = new Vector3(spawn.x + 1, spawn.y, spawn.z);
        Vector3 leftSpawn = new Vector3(spawn.x - 1, spawn.y, spawn.z);

        int obstaclesPlaced = 0;
        do
        {
            spawn = new Vector3(-tileCountSquared * 0.5f, obstacleLevel, tileCountSquared * 0.5f);

            for (int i = 0; i < tileCount; i++) // spawn obstacles
            {
                int surround = 0;
                bool emptySpot = false;

                if (rooms[rooms.Count - 1].layout[i] == null)
                {
                    emptySpot = true;
                }
                else
                {
                    emptySpot = false;
                }

                if (emptySpot)
                {
                    if (Physics.Raycast(spawn, aheadSpawn, 1))
                    {
                        surround++;
                    }
                    if (Physics.Raycast(spawn, behindSpawn, 1))
                    {
                        surround++;
                    }
                    if (Physics.Raycast(spawn, rightSpawn, 1))
                    {
                        surround++;
                    }
                    if (Physics.Raycast(spawn, leftSpawn, 1))
                    {
                        surround++;
                    }
                }

                if (surround < 3 && emptySpot)
                {
                    int rando = Random.Range(1, 10);
                    if (rando <= 4)
                    {
                        rooms[rooms.Count - 1].layout[i] = AddItem('W');
                        rooms[rooms.Count - 1].layout[i].transform.position = spawn;
                        rooms[rooms.Count - 1].layout[i].transform.parent = rooms[rooms.Count - 1].RoomContainer.transform;
                        obstaclesPlaced++;
                    }
                }
                spawn = IncrementSpawn(spawn, tileCountSquared);

                if (obstaclesPlaced >= obstacleCount)
                {
                    break;
                }
            }
        } while (obstaclesPlaced < obstacleCount);

        int chestsPlaced = 0;
        do
        {
            spawn = new Vector3(-tileCountSquared * 0.5f, objectLevel, tileCountSquared * 0.5f);

            for (int i = 0; i < tileCount; i++) // spawn chests
            {
                int surround = 0;
                bool emptySpot = false;

                if (rooms[rooms.Count - 1].layout[i] == null)
                {
                    emptySpot = true;
                }
                else
                {
                    emptySpot = false;
                }

                if (emptySpot)
                {
                    if (Physics.Raycast(spawn, aheadSpawn, 1))
                    {
                        surround++;
                    }
                    if (Physics.Raycast(spawn, behindSpawn, 1))
                    {
                        surround++;
                    }
                    if (Physics.Raycast(spawn, rightSpawn, 1))
                    {
                        surround++;
                    }
                    if (Physics.Raycast(spawn, leftSpawn, 1))
                    {
                        surround++;
                    }
                }

                if (surround < 3 && emptySpot)
                {
                    int rando = Random.Range(1, 10);
                    if (rando <= 1)
                    {
                        rooms[rooms.Count - 1].layout[i] = AddItem('C');
                        rooms[rooms.Count - 1].layout[i].transform.position = spawn;
                        rooms[rooms.Count - 1].layout[i].transform.parent = rooms[rooms.Count - 1].RoomContainer.transform;
                        chestsPlaced++;
                    }
                }
                spawn = IncrementSpawn(spawn, tileCountSquared);

                if (chestsPlaced >= chestCount)
                {
                    break;
                }
            }
        } while (chestsPlaced < chestCount);

        spawn = new Vector3(-tileCountSquared * 0.5f, objectLevel, tileCountSquared * 0.5f);

        for (int i = 0; i < tileCount; i++) // spawn chests
        {
            int surround = 0;
            bool emptySpot = false;
            bool placed = false;

            if (rooms[rooms.Count - 1].layout[i] == null)
            {
                emptySpot = true;
            }
            else
            {
                emptySpot = false;
            }

            if (emptySpot)
            {
                if (Physics.Raycast(spawn, aheadSpawn, 1))
                {
                    surround++;
                }
                if (Physics.Raycast(spawn, behindSpawn, 1))
                {
                    surround++;
                }
                if (Physics.Raycast(spawn, rightSpawn, 1))
                {
                    surround++;
                }
                if (Physics.Raycast(spawn, leftSpawn, 1))
                {
                    surround++;
                }
            }

            if (surround == 0 && emptySpot)
            {
                int rando = Random.Range(1, 10);
                if (rando <= 5)
                {
                    rooms[rooms.Count - 1].layout[i] = AddItem('P');
                    rooms[rooms.Count - 1].layout[i].transform.position = new Vector3(spawn.x, obstacleLevel, spawn.z);
                    rooms[rooms.Count - 1].layout[i].transform.parent = rooms[rooms.Count - 1].RoomContainer.transform;
                    chestsPlaced++;
                    placed = true;
                }
            }
            spawn = IncrementSpawn(spawn, tileCountSquared);

            if (placed)
            {
                break;
            }
        }
        // place obstacles first
        // then chests
        // then player
        // then the door
            // for loop
            // iterate through the spaces in the border array 
            // cast a ray in the 4 directions surrounding it
            // use info to place item and rotate it accordingly

    }

    private Vector3 IncrementSpawn(Vector3 oldSpawn, float tileCountSquared) // in crements a spawn point to make a square
    {
        Vector3 newSpawn = oldSpawn;
        
        if(oldSpawn.x < (tileCountSquared * 0.5f) - 1)
        {
            newSpawn.x += 1;
        }
        else if(oldSpawn.x == (tileCountSquared * 0.5f) - 1)
        {
            newSpawn.x = -(tileCountSquared * 0.5f);
            newSpawn.z -= 1;
        }

        return newSpawn;
    }

    private GameObject AddItem(char item) // instantiates an item out of the array of objects
    {
        GameObject newObject;
        switch(item)
        {
            case 'W':
                for(int i = 0; i < objects.Length; i++)
                {
                    if(objects[i].name == "Wall")
                    {
                        newObject = Instantiate(objects[i]);
                        newObject.name = objects[i].name;
                        return newObject;
                    }
                }
                break;
            case 'F':
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].name == "FloorTile")
                    {
                        newObject = Instantiate(objects[i]);
                        newObject.name = objects[i].name;
                        return newObject;
                    }
                }
                break;
            case 'C':
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].name == "Chest")
                    {
                        newObject = Instantiate(objects[i]);
                        newObject.name = objects[i].name;
                        return newObject;
                    }
                }
                break;
            case 'P':
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].name == "Player")
                    {
                        newObject = Instantiate(objects[i]);
                        newObject.name = objects[i].name;
                        return newObject;
                    }
                }
                break;
        }
        return new GameObject();
    }
}

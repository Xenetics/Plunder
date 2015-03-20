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
        
    }

    public class Room
    {
        public GameObject RoomContainer;
        public int width;
        public int height;
        public int tileCount;
        public GameObject[] floor;
        public GameObject[] layout;
        public GameObject door;
        public GameObject player;
    }

    [SerializeField]
    private GameObject[] objects;

    private List<Room> rooms;

    private const float FLOOR_LEVEL = 0;
    private const float OBJECT_LEVEL = 1;
    private const float OBSTACLE_LEVEL = 1.5f;
    private const int START_DIFF = 10;
    private static int _Diff = 10;
    private static int _TileCount = _Diff * _Diff;
    private const int MAX_ROOMS = 10;
    private const int OBSTACLE_FOR_EVERY = 4; // 1 obstacle spawns for every 4 tiles of floor showing
    private const int CHEST_FOR_EVERY = 20; // 1 chest spawns for every 20 tiles of floor showing

    private static bool _NotStarted = true;

    public static int _CurrentLevel = 0;
    private static bool _LockFound = false;
    private static bool _WinningChest = false;
    private static bool _LastChest = false;

    public void StartGame()
    {
        rooms = new List<Room>();
        BuildRooms(); // builds the room with no objects
    }

    void Update()
    {
        if(GameManager.WhatState() == "playing" && _NotStarted)
        {
            StartGame();
            _NotStarted = false;
            PlayerCamera.Instance.SetPlayer(rooms[_CurrentLevel].player);
        }
        
        if(_LockFound)
        {
            rooms[_CurrentLevel].door.GetComponent<Door>().ForceOpen();
        }

        if(_CurrentLevel >= MAX_ROOMS)
        {
            GameManager.Instance.NewGameState(GameManager.Instance.stateGameMenu);
            Application.LoadLevel("menu");
            _CurrentLevel = 0;
            _NotStarted = true;
            _Diff = START_DIFF;
            _TileCount = _Diff * _Diff;
            foreach(Room rm in rooms)
            {
                Destroy(rm.RoomContainer);
            }
            rooms.Clear();
        }
    }

    public void NextLevel()
    {
        rooms[_CurrentLevel].RoomContainer.gameObject.SetActive(false);
        _CurrentLevel++;
        _LockFound = false;
        rooms[_CurrentLevel].RoomContainer.gameObject.SetActive(true);
        PlayerCamera.Instance.SetPlayer(rooms[_CurrentLevel].player);
    }

    private void BuildRooms()
    {
        for (int i = 0; i < MAX_ROOMS; i++)
        {
            Room newRoom = BuildLevel();
            rooms.Add(newRoom);
            BuildLayout();
            BuildDoor();
            BuildPlayer();
            rooms[rooms.Count - 1].RoomContainer.gameObject.SetActive(false);
            _WinningChest = false;
            _LastChest = false;
            IncrementDiff();
        }
        rooms[0].RoomContainer.gameObject.SetActive(true);
    }

    private void IncrementDiff()
    {
        _Diff += 2;
        _TileCount = _Diff * _Diff;
    }

    private Room BuildLevel()
    {
        Room newRoom = new Room();
        newRoom.RoomContainer = new GameObject();
        newRoom.RoomContainer.name = "RoomContainer_" + _Diff;
        newRoom.RoomContainer.transform.position = Vector3.zero;
        newRoom.width = _Diff;
        newRoom.height = _Diff;
        newRoom.tileCount = newRoom.width * newRoom.height;
        newRoom.floor = BuildFloor(newRoom.RoomContainer);
        newRoom.layout = BuildWalls(newRoom.RoomContainer);

        return newRoom;
    }

    private GameObject[] BuildFloor(GameObject container)
    {
        GameObject[] floor = new GameObject[_TileCount];

        Vector3 spawn = new Vector3(-_Diff * 0.5f, FLOOR_LEVEL, _Diff * 0.5f);

        for (int i = 0; i < _TileCount; i++)
        {
            floor[i] = AddItem('F');
            floor[i].transform.position = spawn;
            floor[i].transform.parent = container.transform;
            spawn = IncrementSpawn(spawn);
        }
        return floor;
    }

    private GameObject[] BuildWalls(GameObject container)
    {
        GameObject[] border = new GameObject[_TileCount];

        Vector3 spawn = new Vector3(-_Diff * 0.5f, OBSTACLE_LEVEL, _Diff * 0.5f);

        for (int i = 0; i < _TileCount; i++) // adds walls
        {
            bool addWall = false;

            if ((i - _Diff) < 0)
            {
                addWall = true;
            }
            else if ((i % _Diff) == 0)
            {
                addWall = true;
            }
            else if (((i + 1) % _Diff * 0.5f) == 0)
            {
                addWall = true;
            }
            else if ((i + _Diff) >= _TileCount)
            {
                addWall = true;
            }
            
            if (addWall)
            {
                border[i] = AddItem('W');
                border[i].transform.position = spawn;
                border[i].transform.parent = container.transform;
            }
            spawn = IncrementSpawn(spawn);
        }

        return border;
    }

    private void BuildLayout()
    {
        float floorCount = (_TileCount - (_Diff * 2) - ((_Diff - 2.0f) * 2));
        float obstacleCount = floorCount / OBSTACLE_FOR_EVERY;
        float chestCount = floorCount / CHEST_FOR_EVERY;

        Vector3 spawn = new Vector3(-_Diff * 0.5f, OBSTACLE_LEVEL, _Diff * 0.5f);
        Vector3 aheadSpawn = new Vector3(spawn.x, spawn.y, spawn.z + 1);
        Vector3 behindSpawn = new Vector3(spawn.x, spawn.y, spawn.z - 1);
        Vector3 rightSpawn = new Vector3(spawn.x + 1, spawn.y, spawn.z);
        Vector3 leftSpawn = new Vector3(spawn.x - 1, spawn.y, spawn.z);

        int obstaclesPlaced = 0;
        do
        {
            spawn = new Vector3(-_Diff * 0.5f, OBSTACLE_LEVEL, _Diff * 0.5f);

            for (int i = 0; i < _TileCount; i++) // spawn obstacles
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
                spawn = IncrementSpawn(spawn);

                if (obstaclesPlaced >= obstacleCount)
                {
                    break;
                }
            }
        } while (obstaclesPlaced < obstacleCount);

        int chestsPlaced = 0;
        do
        {
            spawn = new Vector3(-_Diff * 0.5f, OBJECT_LEVEL, _Diff * 0.5f);

            for (int i = 0; i < _TileCount; i++) // spawn chests
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
                        if(chestsPlaced == chestCount - 1)
                        {
                            _LastChest = true;
                        }
                        rooms[rooms.Count - 1].layout[i] = AddItem('C');
                        rooms[rooms.Count - 1].layout[i].transform.position = spawn;
                        rooms[rooms.Count - 1].layout[i].transform.parent = rooms[rooms.Count - 1].RoomContainer.transform;
                        chestsPlaced++;
                    }
                }
                spawn = IncrementSpawn(spawn);

                if (chestsPlaced >= chestCount)
                {
                    break;
                }
            }
        } while (chestsPlaced < chestCount);
    }

    private void BuildDoor()
    {
        Vector3 spawn = new Vector3(-_Diff * 0.5f, OBSTACLE_LEVEL, _Diff * 0.5f);
        Vector3 aheadSpawn = new Vector3(spawn.x, spawn.y, spawn.z + 1);
        Vector3 behindSpawn = new Vector3(spawn.x, spawn.y, spawn.z - 1);
        Vector3 rightSpawn = new Vector3(spawn.x + 1, spawn.y, spawn.z);
        Vector3 leftSpawn = new Vector3(spawn.x - 1, spawn.y, spawn.z);

        bool placed = false;
        do
        {
            spawn = new Vector3(-_Diff * 0.5f, OBSTACLE_LEVEL, _Diff * 0.5f);
            for (int i = 0; i < _TileCount; i++) // adds walls
            {
                int surround = 0;
                bool doorable = false;
                int wall = 0;


                if (i < _Diff)
                {
                    doorable = true;
                    wall = 1;
                }
                else if ((i % _Diff) == 0)
                {
                    doorable = true;
                    wall = 2;
                }
                else if (((i + 1) % _Diff * 0.5f) == 0)
                {
                    doorable = true;
                    wall = 3;
                }
                else if (i > (_TileCount - _Diff))
                {
                    doorable = true;
                    wall = 4;
                }

                if (((i + 1) - _Diff) == 0 || i == 0 || (i + _Diff) >= _TileCount || (i + 1) == (_TileCount - _Diff))
                {
                    doorable = false;
                }

                if (doorable)
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

                if (doorable && surround < 3 && !placed)
                {
                    int rando = Random.Range(1, 10);
                    if (rando <= 1)
                    {
                        Destroy(rooms[rooms.Count - 1].layout[i]);
                        rooms[rooms.Count - 1].layout[i] = AddItem('D');
                        rooms[rooms.Count - 1].layout[i].transform.position = spawn;
                        rooms[rooms.Count - 1].layout[i].transform.rotation = Quaternion.Euler(rooms[rooms.Count - 1].layout[i].transform.rotation.x, DoorRotation(wall), rooms[rooms.Count - 1].layout[i].transform.rotation.z);
                        rooms[rooms.Count - 1].layout[i].transform.parent = rooms[rooms.Count - 1].RoomContainer.transform;
                        rooms[rooms.Count - 1].door = rooms[rooms.Count - 1].layout[i];
                        placed = true;
                    }
                }
                spawn = IncrementSpawn(spawn);
            }
        } while (!placed);
    }

    private float DoorRotation(int wall)
    {
        switch(wall)
        {
            case 1:
                return 0;
            case 2:
                return 270;
            case 3:
                return 90;
            case 4:
                return 180;
        }
        return 0;
    }

    private void BuildPlayer()
    {
        Vector3 spawn = new Vector3(-_Diff * 0.5f, OBSTACLE_LEVEL, _Diff * 0.5f);
        Vector3 aheadSpawn = new Vector3(spawn.x, spawn.y, spawn.z + 1);
        Vector3 behindSpawn = new Vector3(spawn.x, spawn.y, spawn.z - 1);
        Vector3 rightSpawn = new Vector3(spawn.x + 1, spawn.y, spawn.z);
        Vector3 leftSpawn = new Vector3(spawn.x - 1, spawn.y, spawn.z);

        for (int i = 0; i < _TileCount; i++) // spawn chests
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
                    rooms[rooms.Count - 1].layout[i].transform.position = spawn;
                    rooms[rooms.Count - 1].layout[i].transform.parent = rooms[rooms.Count - 1].RoomContainer.transform;
                    rooms[rooms.Count - 1].player = rooms[rooms.Count - 1].layout[i];
                    placed = true;
                }
            }
            spawn = IncrementSpawn(spawn);

            if (placed)
            {
                break;
            }
        }
    }

    private Vector3 IncrementSpawn(Vector3 oldSpawn) // in crements a spawn point to make a square
    {
        Vector3 newSpawn = oldSpawn;

        if (oldSpawn.x < (_Diff * 0.5f) - 1)
        {
            newSpawn.x += 1;
        }
        else if (oldSpawn.x == (_Diff * 0.5f) - 1)
        {
            newSpawn.x = -(_Diff * 0.5f);
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
                        if (!_WinningChest && Random.Range(1, 100) < 40)
                        {
                            newObject.name = objects[i].name + "_W";
                            _WinningChest = true;
                        }
                        else if(_LastChest)
                        {
                            newObject.name = objects[i].name + "_W";
                            _WinningChest = true;
                        }
                        else
                        {
                            newObject.name = objects[i].name + "_L";
                        }
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
            case 'D':
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].name == "Door")
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

    public void Unlock()
    {
        _LockFound = true;
    }
}

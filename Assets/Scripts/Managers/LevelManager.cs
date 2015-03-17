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
        BuildRooms();
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
    private float terrainLevel = 1.5f;
    private int diff = 10;

    private void BuildRooms()
    {
        int difficulty = 10;
        Room newRoom = BuildLevel(difficulty);
        rooms.Add(newRoom);
    }

    private Room BuildLevel(int difficulty)
    {
        Room newRoom = new Room();
        newRoom.RoomContainer = new GameObject();
        newRoom.RoomContainer.name = "RoomContainer";
        newRoom.RoomContainer.transform.position = Vector3.zero;
        newRoom.width = difficulty;
        newRoom.height = difficulty;
        newRoom.tileCount = newRoom.width * newRoom.height;
        newRoom.floor = BuildFloor(newRoom.tileCount);
        foreach(GameObject floor in newRoom.floor)
        {
            floor.transform.parent = newRoom.RoomContainer.transform;
        }
        newRoom.layout = BuildLayout(newRoom.tileCount);
        foreach(GameObject item in newRoom.layout)
        {
            item.transform.parent = newRoom.RoomContainer.transform;
        }

        return newRoom;
    }

    private GameObject[] BuildFloor(int tileCount)
    {
        GameObject[] floor = new GameObject[tileCount];

        float tileCountSquared = Mathf.Sqrt(tileCount);
        Vector3 spawn = new Vector3(-tileCountSquared * 0.5f, floorLevel, tileCountSquared * 0.5f);
        
        for (int i = 0; i < tileCount; i++ )
        {
            floor[i] = AddItem('F');
            floor[i].transform.position = spawn;
            spawn = IncrementSpawn(spawn, tileCountSquared);
        }
        return floor;
    }

    private GameObject[] BuildLayout(int tileCount)
    {
        GameObject[] layout = new GameObject[tileCount];

        float tileCountSquared = Mathf.Sqrt(tileCount);
        Vector3 spawn = new Vector3(-tileCountSquared * 0.5f, terrainLevel, tileCountSquared * 0.5f);

        for (int i = 0; i <= tileCount; i++) // adds walls
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
                layout[i] = AddItem('W');
                layout[i].transform.position = spawn;
            }
            spawn = IncrementSpawn(spawn, tileCountSquared);
        }

        return layout;
    }


    private Vector3 IncrementSpawn(Vector3 oldSpawn, float tileCountSquared)
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

    /*
    public void BuildLevel(Room room)
    {
        GameObject levelContainer = new GameObject();
        levelContainer.name = "levelContainer";
        levelContainer.transform.position = Vector3.zero;

        Vector2 startPoint = new Vector2(0, 0);
        Vector2 currentSpawn = new Vector2(startPoint.x, startPoint.y);

        for(int i = 0; i < challenges[challenge].tileCount ; i++)
        {
            if (challenges[challenge].layout[i] != 'e')
            {
                GameObject item = AddItem(challenges[challenge].layout[i]);
                item.transform.position = currentSpawn;
                item.transform.parent = levelContainer.transform;
            }
            currentSpawn = incrementSpawn(challenge, startPoint, currentSpawn);
        }
    }
    */

    private GameObject AddItem(char item)
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
        }
        return new GameObject();
    }
}

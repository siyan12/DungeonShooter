using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class LevelRoomRate
{
    public int R;
    public int G;
    public int B;
    public int Y;
}

public class MapCreator : MonoBehaviour {

    public float _roomDistance = 3f;
    public float _roomRandomDis;
    public int _levelRoomMaxNum = 5;
    public int _pathNum = 4;

    public Transform _roomParent;
    public Transform _levelParent;
    public LineRenderer _line;

    public GameObject[] _roomArray;
    public LevelRoomRate[] _levelRoomRate;


    Transform[] levelArrayL;
    int[] _starters;




    private void Start()
    {
       //tarter length
        _starters = new int[_pathNum];


        //create all levels in a loop
        for (int u = 0; u < _levelRoomRate.Length; u++)
        {
            InsLevel(u);
        }
        
        
    }

    //Create a level of Rooms
    void InsLevel(int num) {

        //create level parent
        Transform levelParentT = Instantiate(_levelParent, _roomParent) as Transform;

        //create array of rooms in a level
        Transform[] levelArrayC;

        //create room if is the last level
        if (num == _levelRoomRate.Length-1)
        {
            //levelArrayC length
            levelArrayC = new Transform[1];

            //create last room
            Vector3 position = new Vector3(0, 0, _roomDistance * num + 1f);
            GameObject roomT = Instantiate(_roomArray[4], position, Quaternion.identity, levelParentT) as GameObject;

            //add to arrT
            levelArrayC[0] = roomT.transform;
        }
        //create room if not the last level
        else
        {
            //levelArrayC length
            levelArrayC = new Transform[_levelRoomMaxNum];

            //create array of room weights
            int weightSum = _levelRoomRate[num].R + _levelRoomRate[num].G + _levelRoomRate[num].B + _levelRoomRate[num].Y;
            int[] roomRandomArr = new int[weightSum];
            for (int k = 0; k < roomRandomArr.Length; k++)
            {
                if (k < _levelRoomRate[num].R)
                {
                    roomRandomArr[k] = 0;
                }
                else if (_levelRoomRate[num].R <= k && k < _levelRoomRate[num].R + _levelRoomRate[num].G)
                {
                    roomRandomArr[k] = 1;
                }
                else if (_levelRoomRate[num].R + _levelRoomRate[num].G <= k && k < weightSum - _levelRoomRate[num].Y)
                {
                    roomRandomArr[k] = 2;
                }
                else
                {
                    roomRandomArr[k] = 3;
                }
            }

            //create rooms in a loop
            for (int i = 0; i < _levelRoomMaxNum; i++)
            {
                //create one room

                //room position
                Vector3 position = new Vector3(_roomDistance * i, 0, _roomDistance * num);

                //random array
                int result = roomRandomArr[Random.Range(0, weightSum)];

                //random room offset
                float randomOffsetX = Random.Range(-_roomRandomDis, _roomRandomDis);
                float randomOffsetY = Random.Range(-_roomRandomDis, _roomRandomDis);
                Vector3 randomOffset = new Vector3(randomOffsetX, 0, randomOffsetY);

                //instanciate room
                GameObject roomT = Instantiate(_roomArray[result], position + randomOffset, Quaternion.identity, levelParentT) as GameObject;

                //add to arrT
                levelArrayC[i] = roomT.transform;

               
            }

            //Move level parent
            levelParentT.transform.Translate((1 - _levelRoomMaxNum)*_roomDistance/2, 0, 0);
            
        }

        //Choose to line
        //set starter if is first level
        if (num == 0)
        {
            //chose starters
            //set arr
            int[] levelNums = new int[_levelRoomMaxNum];
            for(int h=0;h< _levelRoomMaxNum; h++)
            {
                levelNums[h] = h;
            }

            //pike ? random num
            for (int q = 0; q < _pathNum; q++)
            {
                _starters[q] = levelNums[Random.Range(0, _levelRoomMaxNum)];
            }
                        
        }
        //line up if not first level
        else if (num > 0)
        {
            int i1;
            int i2;

            int[] i1a = new int[0];
            int[] i2a = new int[0];
            //int q = 0;

            //link in a loop
            for (int q = 0; q < _pathNum; q++)
            {
                //line one

                i1 = _starters[q]; //pick down one

                Transform tran1 = levelArrayL[i1]; //find Trans in Arrs through int1,2

                //line if is last level
                if (num == _levelRoomRate.Length - 1)
                {
                    i2 = 0;
                    _starters[q] = i2; //chengge starter   
                }
                //line if not last level
                else
                {
                    //kill cross line
                    bool noLeft = false;
                    bool noRight = false;
                    //test if this line near any older line
                    if (q>0) //not first line
                    {
                        //test one older line                       
                        for (int m = 0; m < q; m++)
                        {
                            if (i1 - i1a[m] == 1 && i2a[m] - i1a[m] == 1)
                                noLeft = true;
                            if (i1 - i1a[m] == -1 && i2a[m] - i1a[m] == -1)
                                noRight = true;
                        }
                    }

                    //left end
                    if (i1 == 0)
                    {
                        if (noRight)
                        {
                            i2 = i1;
                        }
                        else
                        {
                            i2 = Random.Range(i1, i1 + 2);
                        }

                    }
                    //right nend
                    else if (i1 == _levelRoomMaxNum-1)
                    {
                        if (noLeft)
                        {
                            i2 = i1;
                        }
                        else
                        {
                            i2 = Random.Range(i1 - 1, i1 + 1);
                        }
                    }
                    //rest in middle
                    else
                    {
                        if (noRight && noLeft)
                        {
                            i2 = i1;
                        }
                        else if (noRight)
                        {
                            i2 = Random.Range(i1 - 1, i1 + 1);
                        }
                        else if (noLeft)
                        {
                            i2 = Random.Range(i1, i1 + 2);
                        }
                        else
                        {
                            i2 = Random.Range(i1 - 1, i1 + 2);
                        }
                    }


                    _starters[q] = i2; //chengge starter                    

                }

                Transform tran2 = levelArrayC[i2]; //find Trans in Arrs through int1,2
                InsLine(tran1, tran2, tran1);

                //add i1 i2 to array
                int[] i1aT = new int[q+1];
                int[] i2aT = new int[q+1];
                for (int j = 0; j < i1a.Length; j++)
                {
                    i1aT[j] = i1a[j];
                    i2aT[j] = i2a[j];
                }
                i1aT[q] = i1;
                i1a = i1aT;
                i2aT[q] = i2;
                i2a = i2aT;

            }



        }
        
        //kill room without starters in a loop
        for(int n = 0; n < levelArrayC.Length; n++)
        {

            bool noLine = true;
            for (int g = 0; g < _starters.Length; g++)
            {
                if (_starters[g] == n)
                    noLine = false;
            }
            if (noLine)
            {
                Destroy(levelArrayC[n].gameObject);
            }
            
        }


        //Save last level arr
        levelArrayL = levelArrayC;
    }


    void InsLine (Transform t1, Transform t2, Transform tP)
    {
        LineRenderer line = Instantiate(_line, tP) as LineRenderer;
        Vector3[] positions = new Vector3[] { t1.position, t2.position };

        line.SetPositions(positions);

    }
}

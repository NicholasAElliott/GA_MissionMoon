using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class statsManager : MonoBehaviour {

    public satellite[] launchedSatellites;
    public satellite[] nexGenSatellites;
    public int totalSatellitePopulation;
    public GameObject satelliteGO;
    public GameObject moonGO;
    public GameObject moonSpinGO;
    private int generationID;
    private string statString;
    public string statPrintString;
    private float presentFitness;
    private float timeItirator;

    private GA_Implementation GAI_Script;

    // public Text statTextGO;
    public int moonCount, earthCount, boundryCount, inPersuitCount;

    public GameObject StatsText1GO;

    // Use this for initialization
    void Start ()
    {

        timeItirator = 0;
        generationID = 1;
        statString = "Generation 1 :";
        presentFitness = 999999.9999f;

        // connect to GUI script
        GAI_Script = gameObject.GetComponent<GA_Implementation>();
        GAI_Script.beginPopulation();

        resetAndLaunchGeneration();



        /*
        // create 100 satellites and update stats
        launchedSatellites = new satellite[totalSatellitePopulation];
        // instrances have to be instanciated with moonGO and EarthGO
        for (int i = 0; i < launchedSatellites.Length; i++)
        {
            launchedSatellites[i] = new satellite(i, (Random.Range(0.0f, 360.0f)),(Random.Range(0.0f, 180.0f)) , Random.Range(100.0f, 350.0f));

        }

        resetAndLaunchGeneration();

        */

    }
	

    void Update ()
    {
        timeItirator += Time.deltaTime;
        Transform tf = GetComponent<Transform>();

        if (timeItirator > 21.0f)
        {
            timeItirator = 0;
            this.performCrossoverAndMutation(); 
        }
    }
    // update stats
    public void upsateStat(int cID, int sID, float journeyTime, int targetCollided, float minDistFromMoon)
    {
        //Debug.Log(ID + " " + journeyTime + " " + targetCollided + " " + minDistFromMoon);

        GAI_Script.launchedChromosomes[cID].C_Satellites[sID].recordStats(journeyTime, targetCollided, minDistFromMoon);
        //launchedSatellites[ID].recordStats(ID, journeyTime,targetCollided, minDistFromMoon);
        



        switch (targetCollided)
        {
            case (0):
                inPersuitCount++;
                break;
            case (1):
                earthCount++;
                break;
            case (2):
                boundryCount++;
                break;
            case (3):
                moonCount++;
                break;
            default:
                break;
        }

        //string countStat = "\n\nEarth : " + earthCount.ToString() + "\nboundry : " + boundryCount.ToString() + "\nMoon : " + moonCount.ToString() + "\n StoppedInJourney: " + inPersuitCount.ToString();

        //statPrintString = statString + countStat;




        
        

        
    }

    public void performCrossoverAndMutation()
    {

        GAI_Script.performCrossOverAndMutation();


        



        // Update stats on screen
        generationID++;
        statPrintString = "\nfitness : " + GAI_Script.presentFitness.ToString() + "\nMoon Hits : " + moonCount.ToString() + "\nGeneration : " + generationID.ToString();
        Debug.Log("Method 1 \n" + statPrintString);
        Text b = StatsText1GO.GetComponent<Text>();
        b.text += statPrintString;


        resetAndLaunchGeneration();

    }

    // defining the satellites...!!
    public class satellite
    {
        // the theeta, alpha and initial speed
        public float T,A,V;
        public float minimumDistanceFromMoon;
        public float JourneyTime;
        public int ID;
        // 0-still in journey, 1-hit earth, 2-hit boundry, 3-hit moon
        public int TargetCollided;
        public bool isInPersuit;

        //construction
        public satellite(int id, float t, float a, float v)
        {
            this.T = t;
            this.A = a;
            this.V = v;
            this.ID = id;

            this.minimumDistanceFromMoon = 9999999;
            this.JourneyTime = 0.0f;
            this.TargetCollided = 0;
            this.isInPersuit = true;
        }

        public void recordStats(int id,float journeyTime, int targetCollided, float minDistFromMoon)
        {
            if (id != ID)
                Debug.Log("wrong ID detected, logical error!!");
            this.JourneyTime = journeyTime;
            this.TargetCollided = targetCollided;
            this.minimumDistanceFromMoon = minDistFromMoon;
            this.isInPersuit = false;
        }

        // calculate fitness
        public void calcFitness()
        {
            

        }


        

    }

    public void resetAndLaunchGeneration()
    {

        // reset moon position
        moonSpinGO.transform.rotation = Quaternion.identity;

        moonCount = 0;
        earthCount = 0;
        boundryCount = 0;
        inPersuitCount = 0;


        // instrances have to be instanciated with moonGO and EarthGO
        for (int i = 0; i < GAI_Script.launchedChromosomes.Length; i++)
        {

            for (int j = 0; j < 5; j++)
            {
                GameObject currentGO;
                currentGO = Instantiate(satelliteGO);
                satelliteController SC = currentGO.GetComponent<satelliteController>();
                SC.launchSatellites(i, j, GAI_Script.launchedChromosomes[i].C_Satellites[j].T, GAI_Script.launchedChromosomes[i].C_Satellites[j].A, GAI_Script.launchedChromosomes[i].C_Satellites[j].V);
                SC.moonGO = moonGO;
                SC.earthGO = this.gameObject;

            }

            


        }

        


    }

    /*
    // implementing GA 

    public void CrossOverAndMutation()
    {


        nexGenSatellites = new satellite[totalSatellitePopulation];

        #region        // find the least nine min distance from moon values and record their ID
        int[] selectedID = new int[9];
        float[] selectedT = new float[9];
        selectedT[0] = ( launchedSatellites[0].minimumDistanceFromMoon);
        for (int i = 1; i < 9; i++)
            selectedT[i] = 999999.99f;
        selectedID[0] = 0;
        for (int i = 1; i < totalSatellitePopulation; i++)
        {

                     
            // comparing the first slot
            if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[0])
            {
                // move everything one step down and save present moon dist in first slot
                selectedT[8] = selectedT[7];
                selectedID[8] = selectedID[7];

                selectedT[7] = selectedT[6];
                selectedID[7] = selectedID[6];

                selectedT[6] = selectedT[5];
                selectedID[6] = selectedID[5];

                selectedT[5] = selectedT[4];
                selectedID[5] = selectedID[4];

                selectedT[4] = selectedT[3];
                selectedID[4] = selectedID[3];

                selectedT[3] = selectedT[2];
                selectedID[3] = selectedID[2];

                selectedT[2] = selectedT[1];
                selectedID[2] = selectedID[1];

                selectedT[1] = selectedT[0];
                selectedID[1] = selectedID[0];

                selectedT[0] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[0] = launchedSatellites[i].ID;
            }
            else if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[1])
            {
                // move last 3 one step down and save present moon dist in second slot
                selectedT[8] = selectedT[7];
                selectedID[8] = selectedID[7];

                selectedT[7] = selectedT[6];
                selectedID[7] = selectedID[6];

                selectedT[6] = selectedT[5];
                selectedID[6] = selectedID[5];

                selectedT[5] = selectedT[4];
                selectedID[5] = selectedID[4];

                selectedT[4] = selectedT[3];
                selectedID[4] = selectedID[3];

                selectedT[3] = selectedT[2];
                selectedID[3] = selectedID[2];

                selectedT[2] = selectedT[1];
                selectedID[2] = selectedID[1];

                selectedT[1] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[1] = launchedSatellites[i].ID;
            }
            else if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[2])
            {
                selectedT[8] = selectedT[7];
                selectedID[8] = selectedID[7];

                selectedT[7] = selectedT[6];
                selectedID[7] = selectedID[6];

                selectedT[6] = selectedT[5];
                selectedID[6] = selectedID[5];

                selectedT[5] = selectedT[4];
                selectedID[5] = selectedID[4];

                selectedT[4] = selectedT[3];
                selectedID[4] = selectedID[3];

                selectedT[3] = selectedT[2];
                selectedID[3] = selectedID[2];

                selectedT[2] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[2] = launchedSatellites[i].ID;
            }
            else if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[3])
            {
                selectedT[8] = selectedT[7];
                selectedID[8] = selectedID[7];

                selectedT[7] = selectedT[6];
                selectedID[7] = selectedID[6];

                selectedT[6] = selectedT[5];
                selectedID[6] = selectedID[5];

                selectedT[5] = selectedT[4];
                selectedID[5] = selectedID[4];

                selectedT[4] = selectedT[3];
                selectedID[4] = selectedID[3];

                selectedT[3] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[3] = launchedSatellites[i].ID;
            }
            else if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[4])
            {
                selectedT[8] = selectedT[7];
                selectedID[8] = selectedID[7];

                selectedT[7] = selectedT[6];
                selectedID[7] = selectedID[6];

                selectedT[6] = selectedT[5];
                selectedID[6] = selectedID[5];

                selectedT[5] = selectedT[4];
                selectedID[5] = selectedID[4];

                selectedT[4] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[4] = launchedSatellites[i].ID;
            }
            else if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[5])
            {
                selectedT[8] = selectedT[7];
                selectedID[8] = selectedID[7];

                selectedT[7] = selectedT[6];
                selectedID[7] = selectedID[6];

                selectedT[6] = selectedT[5];
                selectedID[6] = selectedID[5];

                selectedT[5] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[5] = launchedSatellites[i].ID;
            }
            else if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[6])
            {
                selectedT[8] = selectedT[7];
                selectedID[8] = selectedID[7];

                selectedT[7] = selectedT[6];
                selectedID[7] = selectedID[6];

                selectedT[6] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[6] = launchedSatellites[i].ID;
            }
            else if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[7])
            {
                selectedT[8] = selectedT[7];
                selectedID[8] = selectedID[7];

                selectedT[7] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[7] = launchedSatellites[i].ID;
            }
            else if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[8])
            {
                selectedT[8] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[8] = launchedSatellites[i].ID;
            }

        }

        Debug.Log(selectedT[0].ToString() + " - " + selectedT[1].ToString());

        #endregion


        presentFitness = launchedSatellites[selectedID[0]].minimumDistanceFromMoon;


        // satellite devision 0,90,170,240,300,350,390,420,440
        // select the first ID and crossover the first 90 satellites
        nexGenSatellites[0] = launchedSatellites[selectedID[0]];
        nexGenSatellites[0].ID = 0;
        for (int i = 1; i < 90; i++)
        {
            nexGenSatellites[i] = new satellite(i, (Random.Range(launchedSatellites[selectedID[0]].T - 10.0f, launchedSatellites[selectedID[0]].T + 10.0f)), (Random.Range(launchedSatellites[selectedID[0]].A - 10.0f, launchedSatellites[selectedID[0]].A + 10.0f)), (Random.Range(launchedSatellites[selectedID[0]].V - 15.0f, launchedSatellites[selectedID[0]].V + 15.0f)));
            
        }

        // select the second ID and crossover the 90-170 satellites
        nexGenSatellites[90] = launchedSatellites[selectedID[1]];
        nexGenSatellites[90].ID = 90;
        for (int i = 91; i < 170; i++)
        {
            nexGenSatellites[i] = new satellite(i, (Random.Range(launchedSatellites[selectedID[1]].T - 10.0f, launchedSatellites[selectedID[1]].T + 10.0f)), (Random.Range(launchedSatellites[selectedID[1]].A - 10.0f, launchedSatellites[selectedID[1]].A + 10.0f)), (Random.Range(launchedSatellites[selectedID[0]].V - 15.0f, launchedSatellites[selectedID[0]].V + 15.0f)));
            
        }

        // select the third ID and crossover the 170- 240 satellites
        nexGenSatellites[170] = launchedSatellites[selectedID[2]];
        nexGenSatellites[170].ID = 170;
        for (int i = 171; i < 240; i++)
        {
            nexGenSatellites[i] = new satellite(i, (Random.Range(launchedSatellites[selectedID[2]].T - 10.0f, launchedSatellites[selectedID[2]].T + 10.0f)), (Random.Range(launchedSatellites[selectedID[2]].A - 10.0f, launchedSatellites[selectedID[2]].A + 10.0f)), (Random.Range(launchedSatellites[selectedID[0]].V - 15.0f, launchedSatellites[selectedID[0]].V + 15.0f)));
            
        }


        // select the third ID and crossover the  240 - 300 satellites
        nexGenSatellites[240] = launchedSatellites[selectedID[3]];
        nexGenSatellites[240].ID = 240;
        for (int i = 241; i < 300; i++)
        {
            nexGenSatellites[i] = new satellite(i, (Random.Range(launchedSatellites[selectedID[3]].T - 10.0f, launchedSatellites[selectedID[3]].T + 10.0f)), (Random.Range(launchedSatellites[selectedID[3]].A - 10.0f, launchedSatellites[selectedID[3]].A + 10.0f)), (Random.Range(launchedSatellites[selectedID[0]].V - 15.0f, launchedSatellites[selectedID[0]].V + 15.0f)));

        }


        // select the third ID and crossover the 300 - 350 satellites
        nexGenSatellites[300] = launchedSatellites[selectedID[4]];
        nexGenSatellites[300].ID = 300;
        for (int i = 301; i < 350; i++)
        {
            nexGenSatellites[i] = new satellite(i, (Random.Range(launchedSatellites[selectedID[4]].T - 10.0f, launchedSatellites[selectedID[4]].T + 10.0f)), (Random.Range(launchedSatellites[selectedID[4]].A - 10.0f, launchedSatellites[selectedID[4]].A + 10.0f)), (Random.Range(launchedSatellites[selectedID[0]].V - 15.0f, launchedSatellites[selectedID[0]].V + 15.0f)));

        }

        // select the third ID and crossover the 350 - 390 satellites
        nexGenSatellites[350] = launchedSatellites[selectedID[5]];
        nexGenSatellites[350].ID = 350;
        for (int i = 351; i < 390; i++)
        {
            nexGenSatellites[i] = new satellite(i, (Random.Range(launchedSatellites[selectedID[5]].T - 10.0f, launchedSatellites[selectedID[5]].T + 10.0f)), (Random.Range(launchedSatellites[selectedID[5]].A - 10.0f, launchedSatellites[selectedID[5]].A + 10.0f)), (Random.Range(launchedSatellites[selectedID[0]].V - 15.0f, launchedSatellites[selectedID[0]].V + 15.0f)));

        }

        // select the third ID and crossover the 390 - 420 satellites
        nexGenSatellites[390] = launchedSatellites[selectedID[6]];
        nexGenSatellites[390].ID = 390;
        for (int i = 391; i < 420; i++)
        {
            nexGenSatellites[i] = new satellite(i, (Random.Range(launchedSatellites[selectedID[6]].T - 10.0f, launchedSatellites[selectedID[6]].T + 10.0f)), (Random.Range(launchedSatellites[selectedID[6]].A - 10.0f, launchedSatellites[selectedID[6]].A + 10.0f)), (Random.Range(launchedSatellites[selectedID[0]].V - 15.0f, launchedSatellites[selectedID[0]].V + 15.0f)));

        }

        // select the third ID and crossover the 420 - 440 satellites
        nexGenSatellites[420] = launchedSatellites[selectedID[7]];
        nexGenSatellites[420].ID = 420;
        for (int i = 421; i < 440; i++)
        {
            nexGenSatellites[i] = new satellite(i, (Random.Range(launchedSatellites[selectedID[7]].T - 10.0f, launchedSatellites[selectedID[7]].T + 10.0f)), (Random.Range(launchedSatellites[selectedID[7]].A - 10.0f, launchedSatellites[selectedID[7]].A + 10.0f)), (Random.Range(launchedSatellites[selectedID[0]].V - 15.0f, launchedSatellites[selectedID[0]].V + 15.0f)));

        }

        // select the third ID and crossover the 440 - 450 satellites
        nexGenSatellites[440] = launchedSatellites[selectedID[8]];
        nexGenSatellites[440].ID = 440;
        for (int i = 441; i < 450; i++)
        {
            nexGenSatellites[i] = new satellite(i, (Random.Range(launchedSatellites[selectedID[8]].T - 10.0f, launchedSatellites[selectedID[8]].T + 10.0f)), (Random.Range(launchedSatellites[selectedID[8]].A - 10.0f, launchedSatellites[selectedID[8]].A + 10.0f)), (Random.Range(launchedSatellites[selectedID[0]].V - 15.0f, launchedSatellites[selectedID[0]].V + 15.0f)));

        }


        // Mutation for the 80- 100 satellites
        for (int i = 450; i < 500; i++)
        {
            nexGenSatellites[i] = new satellite(i, (Random.Range(0, 360)), (Random.Range(0, 180)), Random.Range(100.0f, 350.0f));
            
        }


        launchedSatellites = nexGenSatellites;
        // reset moon position, and launch next gen
        generationID++;
        statString += "\nFitness = " + presentFitness.ToString() + "\n Generation : " + generationID.ToString();
        resetAndLaunchGeneration();
    }


    */

    /*
    
    previously for 100 pop and 4 selected
    
    
    
     public void CrossOverAndMutation()
    {


        nexGenSatellites = new satellite[totalSatellitePopulation];

        // find the least four min distance from moon values and record their ID
        int[] selectedID = new int[4];
        float[] selectedT = new float[4];
        selectedT[0] = ( launchedSatellites[0].minimumDistanceFromMoon);
        selectedT[1] = 999999;
        selectedT[2] = 999999;
        selectedT[3] = 999999;
        selectedID[0] = 0;
        for (int i = 1; i < 100; i++)
        {
            
            // comparing the first slot
            if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[0])
            {
                // move everything one step down and save present moon dist in first slot
                selectedT[3] = selectedT[2];
                selectedID[3] = selectedID[2];

                selectedT[2] = selectedT[1];
                selectedID[2] = selectedID[1];

                selectedT[1] = selectedT[0];
                selectedID[1] = selectedID[0];

                selectedT[0] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[0] = launchedSatellites[i].ID;
            }
            else if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[1])
            {
                // move last 3 one step down and save present moon dist in second slot
                selectedT[3] = selectedT[2];
                selectedID[3] = selectedID[2];

                selectedT[2] = selectedT[1];
                selectedID[2] = selectedID[1];

                selectedT[1] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[1] = launchedSatellites[i].ID;
            }
            else if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[2])
            {
                // move last 3 one step down and save present moon dist in second slot
                selectedT[3] = selectedT[2];
                selectedID[3] = selectedID[2];

                selectedT[2] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[2] = launchedSatellites[i].ID;
            }
            else if (launchedSatellites[i].minimumDistanceFromMoon < selectedT[3])
            {
                selectedT[3] = launchedSatellites[i].minimumDistanceFromMoon;
                selectedID[3] = launchedSatellites[i].ID;
            }

        }


        presentFitness = launchedSatellites[selectedID[0]].minimumDistanceFromMoon;

        // select the first ID and crossover the first 20 satellites
        for (int i = 0; i < 20; i++)
        {
            nexGenSatellites[i] = new satellite((Random.Range(launchedSatellites[selectedID[0]].T - 10, launchedSatellites[selectedID[0]].T + 10)), (Random.Range(launchedSatellites[selectedID[0]].A - 10, launchedSatellites[selectedID[0]].A + 10)), 300);
            // nexGenSatellites[i].T = Random.Range(launchedSatellites[selectedID[0]].T - 10, launchedSatellites[selectedID[0]].T + 10);
            // nexGenSatellites[i].V = 15;
            // nexGenSatellites[i].A = 90;
        }

        // select the second ID and crossover the 20 - 40 satellites
        for (int i = 20; i < 40; i++)
        {
            nexGenSatellites[i] = new satellite((Random.Range(launchedSatellites[selectedID[1]].T - 10, launchedSatellites[selectedID[1]].T + 10)), (Random.Range(launchedSatellites[selectedID[0]].A - 10, launchedSatellites[selectedID[0]].A + 10)), 300);
            //nexGenSatellites[i].T = Random.Range(launchedSatellites[selectedID[1]].T - 10, launchedSatellites[selectedID[1]].T + 10);
            //nexGenSatellites[i].V = 15;
            //nexGenSatellites[i].A = 90;
        }

        // select the third ID and crossover the 40 - 60 satellites
        for (int i = 40; i < 60; i++)
        {
            nexGenSatellites[i] = new satellite((Random.Range(launchedSatellites[selectedID[2]].T - 10, launchedSatellites[selectedID[2]].T + 10)), (Random.Range(launchedSatellites[selectedID[0]].A - 10, launchedSatellites[selectedID[0]].A + 10)), 300);
            //nexGenSatellites[i].T = Random.Range(launchedSatellites[selectedID[2]].T - 10, launchedSatellites[selectedID[2]].T + 10);
            //nexGenSatellites[i].V = 15;
            //nexGenSatellites[i].A = 90;
        }

        // select the forth ID and crossover the 60 - 80 satellites
        for (int i = 60; i < 80; i++)
        {
            nexGenSatellites[i] = new satellite((Random.Range(launchedSatellites[selectedID[3]].T - 10, launchedSatellites[selectedID[3]].T + 10)), (Random.Range(launchedSatellites[selectedID[0]].A - 10, launchedSatellites[selectedID[0]].A + 10)), 300);
            //nexGenSatellites[i].T = Random.Range(launchedSatellites[selectedID[3]].T - 10, launchedSatellites[selectedID[3]].T + 10);
            //nexGenSatellites[i].V = 15;
           // nexGenSatellites[i].A = 90;
        }

        // Mutation for the 80- 100 satellites
        for (int i = 80; i < 100; i++)
        {
            nexGenSatellites[i] = new satellite((Random.Range(0, 360)), (Random.Range(0, 180)), 300);
            //nexGenSatellites[i].T = Random.Range(0, 360);
            //nexGenSatellites[i].V = 15;
            //nexGenSatellites[i].A = 90;
        }


        launchedSatellites = nexGenSatellites;
        // reset moon position, and launch next gen
        generationID++;
        statString += "\nFitness = " + presentFitness.ToString() + "\n Generation : " + generationID.ToString();
        resetAndLaunchGeneration();
    }
     
    */


}

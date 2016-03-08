using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GA_Red : MonoBehaviour {

    public GameObject satelliteGO;
    public GameObject moonSpinGO;
    public GameObject moonGO;
    public GameObject StatsText2GO;
    public int mutationRate;
    public int populationSize;
    // 1- dist, 2dist+jtime,3- dist+V
    public int FitnessMethod;
    public int fitnessRateChange;

    private satellite[] launchSatellites;
    private int moonCount;
    private int earthCount;
    private int boundryCount;
    private int inPersuitCount;
    private float timeItirator;
    private float presentFitness;
    private int GenerationID;
    private string statPrintString;

    // Use this for initialization
    void Start () {

        this.beginPopulation();
        this.timeItirator = 0;

        this.presentFitness = 0;
        this.GenerationID = 1;
        this.statPrintString = "";
	}
	
	// Update is called once per frame
	void Update () {







        timeItirator += Time.deltaTime;
        Transform tf = GetComponent<Transform>();

        if(timeItirator > 21.0f)
        {
            timeItirator = 0;
            this.SelectionCrossoverMutation();
        }

    }


    // create Population
    void beginPopulation()
    {
        launchSatellites = new satellite[populationSize];
        for (int i = 0; i < launchSatellites.Length; i++)
        {
            launchSatellites[i] = new satellite(i);
        }

        resetAndLaunchGeneration();
    }

    // perform selection and cross over and mutation after simulation is done
    void SelectionCrossoverMutation()
    {

        //calculate fitness

        satellite[] nexGenSat = new satellite[launchSatellites.Length];
        // perform selection
        float totalFitness = 0;

        for (int i = 0; i < launchSatellites.Length; i++)
        {
            if (launchSatellites[i].fitness > presentFitness)
                presentFitness = launchSatellites[i].fitness;
            totalFitness += launchSatellites[i].fitness;
        }



        

        for (int i = 0; i < (launchSatellites.Length / 2); i++)
        {

            // now select two satellites (rollette selection)


            // generate two random numbers from 0 to Tot fitness 
            float rand1 = Random.Range(0.0f, totalFitness);
            float rand2 = Random.Range(0.0f, totalFitness);
            int index1 = 0, index2 = 0;

            // find first indexes
            float tempFitness = 0;
            for (int j = 0; j < launchSatellites.Length; j++)
            {
                tempFitness += launchSatellites[j].fitness;
                if (rand1 < tempFitness)
                {
                    index1 = j;
                    j = launchSatellites.Length;

                }

            }

            // find first indexes
            tempFitness = 0;
            for (int j = 0; j < launchSatellites.Length; j++)
            {
                tempFitness += launchSatellites[j].fitness;
                if (rand2 < tempFitness)
                {
                    index2 = j;
                    j = launchSatellites.Length;

                }

            }

            int Rand = Random.Range(0, 10);
            // perform crossover
            if (Rand > 5)
            {
                nexGenSat[i] = new satellite(i, launchSatellites[index1].T, launchSatellites[index2].A, launchSatellites[index2].V);
                nexGenSat[((launchSatellites.Length / 2) + i)] = new satellite(((launchSatellites.Length / 2) + i), launchSatellites[index2].T, launchSatellites[index1].A, launchSatellites[index1].V);

            }
            else
            {
                nexGenSat[i] = new satellite(i, launchSatellites[index1].T, launchSatellites[index1].A, launchSatellites[index2].V);
                nexGenSat[((launchSatellites.Length / 2) + i)] = new satellite(((launchSatellites.Length / 2) + i), launchSatellites[index2].T, launchSatellites[index2].A, launchSatellites[index1].V);

            }
            
        }


        launchSatellites = nexGenSat;







        // perform mutation 1sat - tot random, 5 sat - T+-10, 5 sat - A+-10, 5 sat V+-10
        int iLength = 0, jLength = 0;

        jLength = (mutationRate / 2) * (populationSize / 50);
        iLength = (mutationRate / 4) * (populationSize / 50);

        for (int i = 0; i < iLength; i++)
        {
            int rand = Random.Range(0, launchSatellites.Length);
            launchSatellites[rand] = new satellite(rand, Random.Range(0.0f, 360.0f), Random.Range(0.0f, 180.0f), Random.Range(300.0f, 420.0f));
        }


        for (int j = 0; j < jLength; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                int rand1 = Random.Range(0, launchSatellites.Length);
                launchSatellites[rand1].T = launchSatellites[rand1].T + Random.Range(-10.0f, +10.0f);

                int rand2 = Random.Range(0, launchSatellites.Length);
                launchSatellites[rand2].A = launchSatellites[rand2].A + Random.Range(-5.0f, +5.0f);

                int rand3 = Random.Range(0, launchSatellites.Length);
                launchSatellites[rand3].V = launchSatellites[rand3].V + Random.Range(-10.0f, +10.0f);
            }
        }
        








        // Update stats on screen
        GenerationID++;
        statPrintString = "\nfitness : " + presentFitness.ToString() + "\nMoon Hits : " + moonCount.ToString() + "\nGeneration : "+GenerationID.ToString();
        Debug.Log("Method 2" +  statPrintString);
        Text b = StatsText2GO.GetComponent<Text>();
        b.text += statPrintString;
        // relaunch satellites start simulation again
        resetAndLaunchGeneration();


        

    }



    // launch and reset satellites
    public void resetAndLaunchGeneration()
    {

        // reset moon position
        moonSpinGO.transform.rotation = Quaternion.identity;
        moonCount = 0;
        earthCount = 0;
        boundryCount = 0;
        inPersuitCount = 0;
        presentFitness = 0;
        // instrances have to be instanciated with moonGO and EarthGO
        for (int i = 0; i < launchSatellites.Length; i++)
        {
                GameObject currentGO;
                currentGO = Instantiate(satelliteGO);
                satelliteControllerRed SC = currentGO.GetComponent<satelliteControllerRed>();
                SC.launchSatellites(i, launchSatellites[i].T, launchSatellites[i].A, launchSatellites[i].V);
                SC.moonGO = moonGO;
                SC.earthGO = this.gameObject;
        }
    }




    // defining the satellites...!!
    public class satellite
    {
        // the theeta, alpha and initial speed
        public float T, A, V;
        public float minimumDistanceFromMoon;
        public float JourneyTime;
        public int ID;
        // 0-still in journey, 1-hit earth, 2-hit boundry, 3-hit moon
        public int TargetCollided;
        public bool isInPersuit;
        public float fitness;

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

        public void recordStats(int id, float journeyTime, int targetCollided, float minDistFromMoon, int fitnesMethod, int frc)
        {
            if (id != ID)
                Debug.Log("wrong ID detected, logical error!!");
            this.JourneyTime = journeyTime;
            this.TargetCollided = targetCollided;
            this.minimumDistanceFromMoon = minDistFromMoon;
            this.isInPersuit = false;

            calcFitness(fitnesMethod, frc);

        }

        // calculate fitness
        public void calcFitness(int method, int rate)
        {
            // method distance
            if (method == 1)
            {
                // fitness to be calculated
                this.fitness = 1000 - (minimumDistanceFromMoon / 4);
            }
            else if (method == 2)
            {
                // fitness for dist + Journey time
                float d = 1000 - (minimumDistanceFromMoon / 4);

                float t = 0;
                if (TargetCollided != 1)
                {
                    t = (100 + (JourneyTime * (-10 / 3))) * (rate/10);

                }
                    
                d += t;
                this.fitness = d;

            }
            

        }

        public satellite(int id)
        {
            this.ID = id;
            this.T = Random.Range(0.0f, 360.0f);
            this.A = Random.Range(0.0f, 180.0f);
            this.V = Random.Range(300.0f, 420.0f);


            this.minimumDistanceFromMoon = 9999999;
            this.JourneyTime = 0.0f;
            this.TargetCollided = 0;
            this.isInPersuit = true;
        }


    }







    // update stats
    public void upsateStat(int ID, float journeyTime, int targetCollided, float minDistFromMoon)
    {
        //Debug.Log(ID + " " + journeyTime + " " + targetCollided + " " + minDistFromMoon);

        //GAI_Script.launchedChromosomes[cID].C_Satellites[sID].recordStats(journeyTime, targetCollided, minDistFromMoon);
        launchSatellites[ID].recordStats(ID, journeyTime,targetCollided, minDistFromMoon, this.FitnessMethod, this.fitnessRateChange);

        //also update fitness over here



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

        string countStat = "\n\nEarth : " + earthCount.ToString() + "\nboundry : " + boundryCount.ToString() + "\nMoon : " + moonCount.ToString() + "\n StoppedInJourney: " + inPersuitCount.ToString();

        //statPrintString = statString + countStat;


        


        /*
        if ((earthCount + moonCount + boundryCount + inPersuitCount) >= totalSatellitePopulation)
        {
            performCrossoverAndMutation();

        }
        */

    }


}

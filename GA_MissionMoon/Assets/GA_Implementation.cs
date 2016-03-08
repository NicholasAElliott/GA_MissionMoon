using UnityEngine;
using System.Collections;

public class GA_Implementation : MonoBehaviour {

    public chromosome[] launchedChromosomes;
    public chromosome[] nextGenChromosomes;
    public float presentFitness;
	// Use this for initialization
	void Start () {

        // Start Population
        presentFitness = 0;

        // perform simpulation and get min distance from moon for all satelites


        

	
	}
	
	// Update is called once per frame
	void Update () {


       
	
	}

    // start the population with random values
    public void beginPopulation()
    {
        // initilize satellites randomly for all chromosomes
        launchedChromosomes = new chromosome[20];
        for (int i = 0; i < launchedChromosomes.Length; i++)
        {
            launchedChromosomes[i] = new chromosome();
            for (int j = 0; j < 5; j++)
            {
                launchedChromosomes[i].C_Satellites[j] = new satellite(i, j, (Random.Range(i * 72.0f, (i + 1) * 72.0f)), (Random.Range(0.0f, 180.0f)), 500);

            }
        }

        resetIndexes();
        // Simulate the chromosomes

    }

    public void resetIndexes()
    {
        for (int i = 0; i < 20; i++)
        {
            launchedChromosomes[i].ChromosomeID = i;
            for (int j = 0; j < 5; j++)
            {
                launchedChromosomes[i].C_Satellites[j].cID = i;
                launchedChromosomes[i].C_Satellites[j].sID = j;
            }
        }
    }

    // call this after every simulation is ended
    public void performCrossOverAndMutation()
    {
        for (int i = 0; i < launchedChromosomes.Length; i++)
        {
            launchedChromosomes[i].calculateFitness();
        }

        for (int i = 0; i < launchedChromosomes.Length; i++)
        {
            if (launchedChromosomes[i].fitness > presentFitness)
                presentFitness = launchedChromosomes[i].fitness;
        }

        nextGenChromosomes = new chromosome[launchedChromosomes.Length];

        // perform selection perform one point crossover after point3 for every chromosome
        float TotAvgFitness = 0;
        for (int i = 0; i < launchedChromosomes.Length; i++)
        {
            TotAvgFitness += launchedChromosomes[i].fitness;
        }
        Debug.Log("0-" + launchedChromosomes[0].fitness + ",1-" + launchedChromosomes[1].fitness + ",2-" + launchedChromosomes[2].fitness + ",Tot" + TotAvgFitness.ToString());



        //TotAvgFitness = TotAvgFitness / 20;

        for (int i = 0; i < 10; i++)
        {
            // create swap chromosomes
            chromosome swapChromosome = new chromosome();
            chromosome swapChromosome2 = new chromosome();

            // generate two random numbers from 0 to Tot fitness avg
            float rand1 = Random.Range(0.0f, TotAvgFitness);
            float rand2 = Random.Range(0.0f, TotAvgFitness);
            int index1 = 0, index2 = 0;

            // find first indexes
            float tempFitness = 0;
            for (int j = 0; j < launchedChromosomes.Length; j++)
            {
                tempFitness += launchedChromosomes[j].fitness;
                if (rand1 < tempFitness)
                {
                    index1 = j;
                    j = 20;

                }
                   
            }

            // find second indexes
            tempFitness = 0;
            for (int j = 0; j < launchedChromosomes.Length; j++)
            {
                tempFitness += launchedChromosomes[j].fitness;
                if (rand2 < tempFitness)
                {
                    index2 = j;
                    j = 20;

                }
                    
            }

            // using indexes perform crossover
            swapChromosome.changeSatellites(i, launchedChromosomes[index1].C_Satellites[0], launchedChromosomes[index1].C_Satellites[1], launchedChromosomes[index1].C_Satellites[2], launchedChromosomes[index2].C_Satellites[3], launchedChromosomes[index2].C_Satellites[4]);
            swapChromosome2.changeSatellites(i+10, launchedChromosomes[index2].C_Satellites[0], launchedChromosomes[index2].C_Satellites[1], launchedChromosomes[index2].C_Satellites[2], launchedChromosomes[index1].C_Satellites[3], launchedChromosomes[index1].C_Satellites[4]);







            // assign the two new chromosomes to new pop
            swapChromosome.ChromosomeID = i;
            nextGenChromosomes[i] = swapChromosome;
            swapChromosome2.ChromosomeID = i + 10;
            nextGenChromosomes[i + 10] = swapChromosome2;
        }





        // perform mutation 5% by changing 5 satellites randomly one from each cromosome selected randomly from 20 
        for (int j = 0; j < 5; j++)
        {
            nextGenChromosomes[Random.Range(0, 20)].C_Satellites[Random.Range(0, 5)].T = Random.Range(0, 360);
            nextGenChromosomes[Random.Range(0, 20)].C_Satellites[Random.Range(0, 5)].A = Random.Range(0, 180);
        }




        // assign new pop
        launchedChromosomes = nextGenChromosomes;
        resetIndexes();
        // simulate the new population
    }

    // implementing the chromosome
    public class chromosome
    {
        public satellite[] C_Satellites;
        public int ChromosomeID;
        public float fitness;

        public chromosome()
        {
            fitness = 0;
            // initate the five satellites

            C_Satellites = new satellite[5];

        }



        public float calculateFitness()
        {
            // calculate fitness fot the complete chromosome


            // Take average of all fitness values in a single chromosome and assign avgFitness

            float sumFitness = 0;

            for (int i = 0; i < C_Satellites.Length; i++)
            {
                sumFitness += C_Satellites[i].minimumDistanceFromMoon;
            }

            this.fitness = (1000 - (sumFitness / 20.0f));
            return fitness;
        }

        public void RandomInitilizeSatellites(int cID)
        {
            this.ChromosomeID = cID;
            for (int i = 0; i < C_Satellites.Length; i++)
            {
                C_Satellites[i] = new satellite(cID, i, (Random.Range(i* 72.0f, (i+1) * 72.0f)), (Random.Range(0.0f, 180.0f)), 320);

            }

        }

        public void changeSatellites(int cid,  satellite sat1, satellite sat2, satellite sat3, satellite sat4, satellite sat5)
        {

            // changing one point crossover to uniform random crossover
            C_Satellites[0] = sat5;
            C_Satellites[0].cID = cid;
            C_Satellites[0].sID = 0;
            C_Satellites[1] = sat4;
            C_Satellites[1].cID = cid;
            C_Satellites[1].sID = 1;
            C_Satellites[2] = sat3;
            C_Satellites[2].cID = cid;
            C_Satellites[2].sID = 3;
            C_Satellites[3] = sat4;
            C_Satellites[3].cID = cid;
            C_Satellites[3].sID = 3;
            C_Satellites[4] = sat2;
            C_Satellites[4].cID = cid;
            C_Satellites[4].sID = 4;

            fitness = 0;

        }
           
    }

    // defining the satellites...!!
    public class satellite
    {
        // the theeta, alpha and initial speed
        public float T, A, V;
        public float minimumDistanceFromMoon;
        public float JourneyTime;
        public int cID, sID;
        // 0-still in journey, 1-hit earth, 2-hit boundry, 3-hit moon
        public int TargetCollided;
        public bool isInPersuit;

        //construction
        public satellite(int cid,int sid, float t, float a, float v)
        {
            this.T = t;
            this.A = a;
            this.V = v;
            this.cID = cid;
            this.sID = sid;

            this.minimumDistanceFromMoon = 9999999;
            this.JourneyTime = 0.0f;
            this.TargetCollided = 0;
            this.isInPersuit = true;
        }

        public void recordStats(float journeyTime, int targetCollided, float minDistFromMoon)
        {
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

}
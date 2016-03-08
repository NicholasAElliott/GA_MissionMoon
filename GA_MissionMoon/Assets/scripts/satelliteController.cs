using UnityEngine;
using System.Collections;

public class satelliteController : MonoBehaviour {

    public int chromosomeID, SatelliteID;
    public float t,a,v;
    public bool isSatelliteInPersuit;
    public GameObject moonGO;
    public GameObject earthGO;
    private Vector3 Temp;
    public float minDistanceFromMoon;
    private float journeyTime;
    // Use this for initialization




    //gravity pull
    private float distance;
    private Vector2 direction;
    private Vector3 temp;

    void Start () {
        isSatelliteInPersuit = true;
        journeyTime = 0;

        //launchSatellites(t, a, v);



        this.direction = new Vector2(0.0f, 0.0f);
        Temp = new Vector3(0.0f, 0.0f, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {
        // if satellite is launched and in space do....
        if (isSatelliteInPersuit)
        {
            journeyTime += Time.deltaTime;
            Transform tf = GetComponent<Transform>();

            // calculate min distance from moon
            temp = (moonGO.transform.position - tf.position);
            if (minDistanceFromMoon > temp.magnitude)
                minDistanceFromMoon = temp.magnitude;
            // update states of min distance moon to stats manager





            // gravity from earth
            
            // set the direction towards the centre of the earth
            Temp = (earthGO.transform.position - tf.position);
            direction = new Vector2(Temp.normalized.x, Temp.normalized.y);
            // get the distance between the moon and the earth
            //speed = transform.position.magnitude;
            distance = Temp.magnitude;
            GetComponent<Rigidbody2D>().AddForce(direction * (36 * 36 * 3900000.0f / (Mathf.Pow(distance, 2.0f))), ForceMode2D.Force);




            // gravity from moon
            //Transform transform = GetComponent<Transform>();
            // set the direction towards the centre of the moon
            Temp = (moonGO.transform.position - tf.position);
            direction = new Vector2(Temp.normalized.x, Temp.normalized.y);
            distance = Temp.magnitude;
            GetComponent<Rigidbody2D>().AddForce(direction * (36 * 36 * 480000.0f / (Mathf.Pow(distance, 2.0f))), ForceMode2D.Force);


            





            if (journeyTime > 20.0f)
            {
                statsManager SM = earthGO.GetComponent<statsManager>();
                //Debug.Log("Satellit ID : " + this.ID.ToString() + " : Simulation Time Complected");
                isSatelliteInPersuit = false;
                SM.upsateStat(this.chromosomeID, this.SatelliteID, this.journeyTime, 0, this.minDistanceFromMoon);

                //destroy the GO
                Destroy(this.gameObject);
            }

        }
	
	}


    // launches satellite from surface of earth at an angle alpha with an initial speed. theeta determines the location on the circumferance 
    public void launchSatellites(int ChromosomeID, int satelliteID, float theta, float alpha, float initialSpeed)
    {

        this.chromosomeID = ChromosomeID;
        this.SatelliteID = satelliteID;
        // move the satellite to the location Re(cos*i + sin*j)
        transform.position = new Vector3((64 *  (Mathf.Cos(0.0174533f * theta))), (64 * (Mathf.Sin(0.0174533f * theta))), 0);

        //set the initial speed at an angle alpha
        float beeta = 0.0174533f * (alpha + theta - 90);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2 (initialSpeed * Mathf.Cos(beeta), initialSpeed * Mathf.Sin(beeta));


        // satellite is launched
        isSatelliteInPersuit = true;
    }


    //detect collisions 
    void OnTriggerEnter2D(Collider2D col)
    {
        statsManager SM = earthGO.GetComponent<statsManager>();
        if (col.gameObject.tag == "earthTag")
        {
           // Debug.Log("Satellit ID : " + this.ID.ToString() + " : hit the earth. ");
            isSatelliteInPersuit = false;
            SM.upsateStat(this.chromosomeID, this.SatelliteID, this.journeyTime, 1, this.minDistanceFromMoon);

            //destroy the GO
            Destroy(this.gameObject);
        }
            
        if (col.gameObject.tag == "moonTag")
        {
            Debug.Log("Satellit ID : (" + this.chromosomeID.ToString() + "-" + this.SatelliteID.ToString() + ") : hit the moon. ");
            isSatelliteInPersuit = false;
            SM.upsateStat(this.chromosomeID, this.SatelliteID, this.journeyTime, 3, this.minDistanceFromMoon);

            //destroy the GO
            Destroy(this.gameObject);
        }

    }
    void OnTriggerExit2D(Collider2D col)
    {
        statsManager SM = earthGO.GetComponent<statsManager>();
        if (col.gameObject.tag == "boundryTag")
        {
           // Debug.Log("Satellit ID : " + this.ID.ToString() + " : hit the boundry. ");
            isSatelliteInPersuit = false;
            SM.upsateStat(this.chromosomeID, this.SatelliteID, this.journeyTime, 2, this.minDistanceFromMoon);

            // destroy the GO
            Destroy(this.gameObject);
        }
    }
}

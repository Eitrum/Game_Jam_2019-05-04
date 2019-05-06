using UnityEngine;

public enum PlanetType { 
    Mars = 0,
    Jupiter = 1,
    Uranus = 2,
    Saturn = 3,
    Pluto = 4,
    Earth = 5
}


public class FMODScript : MonoBehaviour
{
    public string bgmString = "event:/Soundtrack/bgm";
    public FMOD.Studio.EventInstance bgmEvent;


    public string winnerString;

    public string[] planetEventStrings = { 
        "event:/marsWin",
        "event:/jupiterWin",
        "event:/uranusWin",
        "event:/saturnWin",
        "event:/plutoWin",
        "event:/earthWin",
    };
    public FMOD.Studio.EventInstance[] planetEvents;

    // Start is called before the first frame update
    void Start()
    {
        bgmEvent = FMODUnity.RuntimeManager.CreateInstance(bgmString);

        int planets = planetEventStrings.Length;
        planetEvents = new FMOD.Studio.EventInstance[planets];

        for (int iPlanet = 0, nPlanet = planets; iPlanet < nPlanet; ++iPlanet)
        {
            planetEvents[iPlanet] = FMODUnity.RuntimeManager.CreateInstance(planetEventStrings[iPlanet]);
        }

        GameManager.OnRoundStart += OnRoundStart;
        GameManager.OnRoundEnd += OnRoundEnd;
        GameManager.OnRestart += OnRestart;
        GameManager.OnCountDown += OnCountDown;
        bgmEvent.start();
    }

    void OnCountDown(int count)
    {
        if (count == 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/go");
        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/countdown");
        }
    }

    void OnRestart()
    {
        for (int iPlanet = 0, nPlanet = planetEventStrings.Length; iPlanet < nPlanet; ++iPlanet)
        {
            planetEvents[iPlanet].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    void OnRoundStart()
    {
        bgmEvent.setPaused(false);
    }

    void OnRoundEnd(Player winner)
    {
        bgmEvent.setPaused(true);
        if (winner != null)
        {
            PlanetType? planet = null;
            switch (winner.playerName)
            {
                case "Jupiter":
                    planet = PlanetType.Jupiter;
                    break;
                case "Mars":
                    planet = PlanetType.Mars;
                    break;
                case "Uranus":
                    planet = PlanetType.Uranus;
                    break;
                case "Saturn":
                    planet = PlanetType.Saturn;
                    break;
                case "Pluto":
                    planet = PlanetType.Pluto;
                    break;
                default:
                    break;
            }
            if (planet.HasValue)
            {
                planetEvents[(int)planet.Value].start();
            }
        }
        else
        {
            planetEvents[(int)PlanetType.Earth].start();
        }
    }
}

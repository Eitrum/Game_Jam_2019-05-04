using UnityEngine;
using UnityEngine.UI;

public class NamePlate : MonoBehaviour
{
    public Text front;
    public Text back;

    public void SetName(string name)
    {
        front.text = name;
        back.text = name;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Debug = UnityEngine.Debug;    //System.Diagnostics and UnityEngine both use "Debug" so this directive declares that Debug statements should be treated as UnityEngine.Debug.Log


    public class SliderDisplay : MonoBehaviour
{
    public GameObject sliderTarget;  //Create a reference for the UI object whose value is to be tracked (e.g. Slider)
    public UnityEngine.UI.Text textDisplay;        //Create a reference for the virtual robot object in the scene
    [SerializeField]
    public int offset;
    [SerializeField]
    public float multiplier;


    // Start is called before the first frame update
    void Start()
    {
        textDisplay.text = (offset + sliderTarget.gameObject.GetComponent<Slider>().value * multiplier).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showSliderValue()
    {
        textDisplay.text = (offset + sliderTarget.gameObject.GetComponent<Slider>().value * multiplier).ToString();
    }
}

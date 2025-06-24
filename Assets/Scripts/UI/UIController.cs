using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public Slider alphaSlider, betaSlider, evaporationSlider, antsSlider, iterationsSlider;
    public TextMeshProUGUI alphaText, betaText, evaporationText, antsText, iterationsText;
    public Button startButton;
    public Button addNodeButton;
    public Button clearGraphButton;

    public ACOController acoController;

    void Start()
    {
        alphaSlider.onValueChanged.AddListener(val => alphaText.text = $"alpha: {val:F2}");
        betaSlider.onValueChanged.AddListener(val => betaText.text = $"beta: {val:F2}");
        evaporationSlider.onValueChanged.AddListener(val => evaporationText.text = $"evaporation rate: {val:F2}");
        antsSlider.onValueChanged.AddListener(val => antsText.text = $"num ants: {Mathf.RoundToInt(val)}");
        iterationsSlider.onValueChanged.AddListener(val => iterationsText.text = $"num iterations: {Mathf.RoundToInt(val)}");

        startButton.onClick.AddListener(() =>
        {
            acoController.alpha = alphaSlider.value;
            acoController.beta = betaSlider.value;
            acoController.evaporation = evaporationSlider.value;
            acoController.numAnts = Mathf.RoundToInt(antsSlider.value);
            acoController.iterations = Mathf.RoundToInt(iterationsSlider.value);
            acoController.StartSimulation();
        });

        addNodeButton.onClick.AddListener(() =>
        {
            acoController.graph.ToggleNodePlacement();
        });

        clearGraphButton.onClick.AddListener(() =>
        {
            acoController.graph.ClearGraph();
        });

        // Initialize sliders with default values
        alphaSlider.onValueChanged.Invoke(alphaSlider.value);
        betaSlider.onValueChanged.Invoke(betaSlider.value);
        evaporationSlider.onValueChanged.Invoke(evaporationSlider.value);
        antsSlider.onValueChanged.Invoke(antsSlider.value);
        iterationsSlider.onValueChanged.Invoke(iterationsSlider.value);
    }
}

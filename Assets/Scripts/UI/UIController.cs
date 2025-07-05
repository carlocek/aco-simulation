using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public Slider alphaSlider, betaSlider, evaporationSlider, antsSlider, iterationsSlider, simulationSpeedSlider;
    public TextMeshProUGUI alphaText, betaText, evaporationText, antsText, iterationsText, simulationSpeedText;
    public TextMeshProUGUI logText;
    public Button startButton;
    private TextMeshProUGUI startButtonText;
    public Button addNodeButton;
    public Button clearGraphButton;

    public ACOController acoController;

    void Start()
    {
        acoController.OnLog += AppendLog;
        startButtonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
        alphaSlider.onValueChanged.AddListener(val => alphaText.text = $"alpha: {val:F2}");
        betaSlider.onValueChanged.AddListener(val => betaText.text = $"beta: {val:F2}");
        evaporationSlider.onValueChanged.AddListener(val => evaporationText.text = $"evaporation rate: {val:F2}");
        antsSlider.onValueChanged.AddListener(val => antsText.text = $"num ants: {Mathf.RoundToInt(val)}");
        iterationsSlider.onValueChanged.AddListener(val => iterationsText.text = $"num iterations: {Mathf.RoundToInt(val)}");
        simulationSpeedSlider.onValueChanged.AddListener(val =>
        {
            simulationSpeedText.text = $"simulation speed: {val:F2} frames/sec";
            if (acoController != null)
                acoController.stepDelay = 1f / val;
        });

        startButton.onClick.AddListener(() =>
        {
            acoController.alpha = alphaSlider.value;
            acoController.beta = betaSlider.value;
            acoController.evaporation = evaporationSlider.value;
            acoController.numAnts = Mathf.RoundToInt(antsSlider.value);
            acoController.iterations = Mathf.RoundToInt(iterationsSlider.value);
            acoController.stepDelay = 1f / simulationSpeedSlider.value;
            acoController.ToggleSimulation();
            if (!acoController.simulationRunning)
                startButtonText.text = "Start Simulation";
            else if (acoController.simulationRunning && !acoController.simulationPaused)
                startButtonText.text = "Pause Simulation";
            else
                startButtonText.text = "Resume Simulation";
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
        simulationSpeedSlider.onValueChanged.Invoke(simulationSpeedSlider.value);
    }

    public void AppendLog(string message)
    {
        if (logText == null)
            return;
        string[] lines = (logText.text + "\n" + message).Split('\n');
        int maxLines = 3;
        if (lines.Length > maxLines)
            lines = lines[^maxLines..]; // ultime righe
        logText.text = string.Join("\n", lines);
    }
}

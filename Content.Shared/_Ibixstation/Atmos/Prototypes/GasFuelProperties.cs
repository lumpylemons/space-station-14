using Content.Shared.Atmos;

namespace Content.Shared._Ibixstation.Atmos.Prototypes;

[DataDefinition]
public partial struct GasFuelProperties
{
    [DataField] public float MinIgnitionTemperature = 373.15f; // Defaults to 100°C
    [DataField] public float BurnRateDelta = 0f;
    [DataField] public float EnergyReleased = 0f;
    [DataField] public float OxygenRatio = 0f;
    [DataField] public float DissociationOnsetTemperature = 2000f;
    [DataField] public float MaxFlameTemperature = 0f;
    [DataField] public Dictionary<Gas, float> StandardProducts = new();
    [DataField] public Dictionary<Gas, float> DissociationProducts = new();
    [DataField] public List<FireVisualMilestone> FireVisualTempThresholds = new();

    public GasFuelProperties()
    {
    }
}

[DataDefinition]
public partial struct FireVisualMilestone
{
    [DataField] public float Temperature = 373.15f;
    [DataField] public Color Color = Color.FromHex("#FFB600");
    [DataField] public float Opacity = 0.8f;

    public FireVisualMilestone()
    {
    }
}

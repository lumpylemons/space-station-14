using Content.Shared.Atmos;

namespace Content.Shared._Ibixstation.Atmos.Prototypes;

/// <summary>
/// Defines the behavior and combustion parameters of a gas when it acts as an oxidizer.
/// </summary>
[DataDefinition]
public partial struct GasOxidizerProperties
{
    /// <summary>
    /// The effectiveness or strength of this gas as an oxidizer compared to standard oxygen.
    /// </summary>
    [DataField] public float Power = 1.0f;

    /// <summary>
    /// Products produced specifically by the breakdown or reaction of this oxidizer during combustion.
    /// </summary>
    [DataField] public Dictionary<Gas, float> Byproducts = new();
}

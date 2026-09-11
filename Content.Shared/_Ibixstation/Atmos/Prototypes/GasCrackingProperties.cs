using Content.Shared.Atmos;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

namespace Content.Shared._Ibixstation.Atmos.Prototypes;

[DataDefinition]
public partial struct GasCrackingProperties
{
    /// <summary>
    /// The threshold temperature (in Kelvin) where thermal cracking begins.
    /// </summary>
    [DataField("onsetTemperature")]
    public float OnsetTemperature;

    /// <summary>
    /// How fast the gas cracks per tick. Higher values limit conversion speed.
    /// </summary>
    [DataField("crackingRateDelta")]
    public float CrackingRateDelta;

    /// <summary>
    /// Thermal energy absorbed by the reaction in Joules per mole cracked (Endothermic).
    /// </summary>
    [DataField("energyAbsorbed")]
    public float EnergyAbsorbed;

    /// <summary>
    /// The gases and ratios produced when this gas decomposes.
    /// </summary>
    [DataField("products", customTypeSerializer: typeof(DictionarySerializer<Gas, float>))]
    public Dictionary<Gas, float> Products = default!;
}

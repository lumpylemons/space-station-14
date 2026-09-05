using Content.Shared.Atmos;

namespace Content.Shared._Ibixstation.Atmos;

/// <summary>
///     Atmospheric constants and parameters specific to Ibixstation reactions.
/// </summary>
public static class IbixAtmospherics
{
    // =========================================================================
    // Hydrogen & Water Vapor Equilibrium
    // =========================================================================

    /// <summary>
    ///     Divisor scaling the base burn rate per tick for Tritium/Hydrogen combustion.
    ///     [Ibixstation]
    /// </summary>
    public const float HydrogenBurnRateDelta = 10f;

    /// <summary>
    ///     Minimum temperature in Kelvin required to initiate endothermic water vapor decomposition.
    ///     [Ibixstation]
    /// </summary>
    public const float WaterDecompositionMinTemp = 1800f;

    /// <summary>
    ///     Arrhenius activation energy constant controlling the exponential rate scaling of water decomposition.
    ///     [Ibixstation]
    /// </summary>
    public const float WaterDecompositionActivationEnergy = 7500f;

    /// <summary>
    ///     Amount of energy absorbed per mole of decomposed water vapor.
    ///     [Ibixstation]
    /// </summary>
    public const float WaterDecompositionEnergyAbsorbed = Atmospherics.FireHydrogenEnergyReleased;

    // =========================================================================
    // Sabatier Synthesis
    // =========================================================================

    /// <summary>
    ///     Reaction rates and constants for Sabatier Reaction.
    ///     [Ibixstation]
    /// </summary>
    public const float SabatierReactionRate = 10f;

    /// <summary>
    ///     Energy released per mole of Methane produced (165 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float SabatierEnergyReleased = 165000f;

    // =========================================================================
    // Carbon Chain & Combustion Equilibrium
    // =========================================================================

    /// <summary>
    ///     Energy released per mole of Methane burned (675 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float FireMethaneEnergyReleased = 675000f;

    /// <summary>
    ///     Divisor scaling the base burn rate per tick for Methane combustion.
    ///     [Ibixstation]
    /// </summary>
    public const float MethaneBurnRateDelta = 18f;

    /// <summary>
    ///     Energy released per mole of Carbon Monoxide burned (255 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float FireCOEnergyReleased = 255000f;

    /// <summary>
    ///     Divisor scaling the base burn rate per tick for Carbon Monoxide combustion.
    ///     [Ibixstation]
    /// </summary>
    public const float COBurnRateDelta = 22f;

    /// <summary>
    ///     Minimum temperature in Kelvin required to initiate CO2 thermal decomposition.
    ///     [Ibixstation]
    /// </summary>
    public const float CO2DecompositionMinTemp = 1815.15f;

    /// <summary>
    ///     Arrhenius activation energy constant controlling the exponential rate scaling of CO2 decomposition.
    ///     [Ibixstation]
    /// </summary>
    public const float CO2DecompositionActivationEnergy = 8140f;

    /// <summary>
    ///     Amount of energy absorbed per mole of decomposed CO2.
    ///     [Ibixstation]
    /// </summary>
    public const float CO2DecompositionEnergyAbsorbed = FireCOEnergyReleased;
}

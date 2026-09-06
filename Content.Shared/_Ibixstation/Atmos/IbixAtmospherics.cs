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
    // CO2 Methanation
    // =========================================================================

    /// <summary>
    ///     Reaction rates and constants for Sabatier Reaction.
    ///     [Ibixstation]
    /// </summary>
    public const float CO2MethanationReactionRate = 20f;

    /// <summary>
    ///     Energy released per mole of Methane produced (165 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float CO2MethanationEnergyReleased = 165000f;

    // =========================================================================
    // CO Methanation
    // =========================================================================

    /// <summary>
    ///     Reaction rate divisor for CO Methanation.
    ///     [Ibixstation]
    /// </summary>
    public const float COMethanationReactionRate = 10f;

    /// <summary>
    ///     Energy released per mole of Methane produced via CO methanation (206 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float COMethanationEnergyReleased = 206000f;

    // =========================================================================
    // Water-Gas Shift
    // =========================================================================

    /// <summary>
    ///     Reaction rate divisor for Water-Gas Shift.
    ///     [Ibixstation]
    /// </summary>
    public const float WaterGasShiftReactionRate = 25f;

    /// <summary>
    ///     Energy released per mole of CO shifted to H2 via water vapor (41 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float WaterGasShiftEnergyReleased = 41000f;

    // =========================================================================
    // Cyanogen Synthesis Pathways & Combustion
    // =========================================================================

    /// <summary>
    ///     Reaction rate divisor scaling the per-tick conversion for oxidative cyanogen synthesis.
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenOxidativeReactionRate = 25f;

    /// <summary>
    ///     Energy released per mole of Cyanogen produced via oxidative synthesis (180 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenOxidativeEnergyReleased = 180000f;

    /// <summary>
    ///     Reaction rate divisor scaling the base per-tick conversion for pyrolytic cyanogen synthesis.
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenPyrolysisReactionRate = 35f;

    /// <summary>
    ///     Arrhenius activation energy constant controlling exponential scaling for thermal pyrolysis.
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenPyrolysisActivationEnergy = 9500f;

    /// <summary>
    ///     Amount of energy absorbed per mole of Cyanogen produced via endothermic pyrolysis (320 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenPyrolysisEnergyAbsorbed = 320000f;

    /// <summary>
    ///     Reaction rate divisor scaling the per-tick conversion for CO-route cyanogen synthesis.
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenCOReactionRate = 30f;

    /// <summary>
    ///     Energy released per mole of Cyanogen produced via carbon monoxide synthesis (45 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenCOEnergyReleased = 45000f;

    /// <summary>
    ///     Energy released per mole of Cyanogen burned (1095 kJ/mol).
    ///     Generates an extremely hot, hyper-exothermic clean flame.
    ///     [Ibixstation]
    /// </summary>
    public const float FireCyanogenEnergyReleased = 1095000f;

    /// <summary>
    ///     Divisor scaling the base burn rate per tick for Cyanogen combustion.
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenBurnRateDelta = 15f;

    // =========================================================================
    // Carbon Chain & Combustion
    // =========================================================================

    /// <summary>
    ///     Energy released per mole of Methane burned (802 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float FireMethaneEnergyReleased = 802000f;

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

    // =========================================================================
    // Ammonia Synthesis, Combustion & Decomposition
    // =========================================================================

    /// <summary>
    ///     Energy released per mole of Ammonia burned (383 kJ/mol).
    ///     Produces a relatively weak, slow-burning flame.
    ///     [Ibixstation]
    /// </summary>
    public const float FireAmmoniaEnergyReleased = 383000f;

    /// <summary>
    ///     Divisor scaling the base burn rate per tick for Ammonia combustion.
    ///     High value means it burns much slower per tick compared to Methane or Hydrogen.
    ///     [Ibixstation]
    /// </summary>
    public const float AmmoniaBurnRateDelta = 35f;

    /// <summary>
    ///     Reaction rate divisor for Ammonia Synthesis.
    ///     [Ibixstation]
    /// </summary>
    public const float HighPressureAmmoniaSynthesisReactionRate = 15f;

    /// <summary>
    ///     Energy released per mole of Ammonia synthesized (46.2 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float HighPressureAmmoniaSynthesisEnergyReleased = 46200f;

    /// <summary>
    ///     Energy absorbed per mole of Ammonia decomposed (46.2 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float AmmoniaDecompositionEnergyAbsorbed = 46200f;
}

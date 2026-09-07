using Content.Shared.Atmos;

namespace Content.Shared._Ibixstation.Atmos;

/// <summary>
///     Atmospheric constants and parameters specific to Ibixstation reactions.
/// </summary>
public static class IbixAtmospherics
{
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

    public const float AmmoniaDecompositionReactionRate = 25f;

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

    /// <summary>
    ///     Reaction rate divisor scaling the per-tick conversion for Reverse Water-Gas Shift.
    ///     [Ibixstation]
    /// </summary>
    public const float ReverseWaterGasShiftReactionRate = 25f;

    /// <summary>
    ///     Arrhenius activation energy constant controlling exponential scaling for Reverse Water-Gas Shift.
    ///     [Ibixstation]
    /// </summary>
    public const float ReverseWaterGasShiftActivationEnergy = 4800f;

    /// <summary>
    ///     Amount of energy absorbed per mole of CO2 shifted to CO via hydrogen (41 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float ReverseWaterGasShiftEnergyAbsorbed = 41000f;

    /// <summary>
    ///     Reaction rate divisor scaling the per-tick conversion for Steam Methane Reforming.
    ///     [Ibixstation]
    /// </summary>
    public const float SteamMethaneReformingReactionRate = 18f;

    /// <summary>
    ///     Arrhenius activation energy constant controlling exponential scaling for Steam Methane Reforming.
    ///     [Ibixstation]
    /// </summary>
    public const float SteamMethaneReformingActivationEnergy = 6200f;

    /// <summary>
    ///     Amount of energy absorbed per mole of Methane reformed via water vapor (206 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float SteamMethaneReformingEnergyAbsorbed = 206000f;

    /// <summary>
    ///     Required ratio of Plasma to Methane (2 moles Plasma per 1 mole Methane).
    ///     [Ibixstation]
    /// </summary>
    public const float FrezonProductionPlasmaRatio = 2.0f;

    /// <summary>
    ///     Multiplier used to calculate Nitrogen loss during inefficient synthesis.
    ///     [Ibixstation]
    /// </summary>
    public const float FrezonProductionNitrogenLossRatio = 4.5f;

    /// <summary>
    ///     Required ratio of Nitrogen to Frezon for cooling (5 moles N2 per 1 mole Frezon).
    ///     5 Oxygen atoms from spent Frezon bind with 10 Nitrogen atoms to form Nitrous Oxide.
    ///     [Ibixstation]
    /// </summary>
    public const float FrezonNitrogenCoolRatio = 5.0f;

    /// <summary>
    ///     Yield multiplier of Nitrous Oxide produced per mole of Frezon spent (5 moles N2O per 1 mole Frezon).
    ///     [Ibixstation]
    /// </summary>
    public const float FrezonN2OCoolOutputRatio = 5.0f;

    /// <summary>
    ///     Energy absorbed per mole of Frezon burned during coolant reaction.
    ///     Increased significantly to ensure Frezon rapidly drops room/canister temperatures to near-absolute zero
    ///     despite the heat capacity of generated Nitrous Oxide.
    ///     [Ibixstation]
    /// </summary>
    public const float FrezonCoolEnergyReleased = -960000f;

    /// <summary>
    ///     1 / X of the methane is converted into Frezon each tick
    ///     [Ibixstation]
    /// </summary>
    public const float FrezonProductionConversionRate = 5f;

    /// <summary>
    ///     Endothermic energy absorbed per mole of Frezon produced to offset
    ///     injecting 2 moles of Plasma at room temp (293.15K to 73.15K = 88 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float FrezonProductionEnergyAbsorbed = -48000f;

    /// <summary>
    ///     The minimum temperature threshold below which Frezon production cooling scale drops to zero.
    ///     [Ibixstation]
    /// </summary>
    public const float FrezonProductionCoolingLowerTemperature = 23.15f;

    /// <summary>
    ///     The target temperature threshold at which Frezon production cooling achieves maximum scale.
    ///     [Ibixstation]
    /// </summary>
    public const float FrezonProductionCoolingMidTemperature = 73.15f;
}

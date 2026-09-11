using Content.Shared.Atmos;

namespace Content.Shared._Ibixstation.Atmos;

/// <summary>
///     Atmospheric constants and parameters specific to Ibixstation reactions.
/// </summary>
public static class IbixAtmospherics
{
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
    public const float CO2MethanationReactionRate = 4f;

    /// <summary>
    ///     Energy released per mole of Methane produced (165 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float CO2MethanationEnergyReleased = 165000f;

    /// <summary>
    ///     Reaction rate divisor for CO Methanation.
    ///     [Ibixstation]
    /// </summary>
    public const float COMethanationReactionRate = 2f;

    /// <summary>
    ///     Energy released per mole of Methane produced via CO methanation (206 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float COMethanationEnergyReleased = 206000f;

    /// <summary>
    ///     Reaction rate divisor for Water-Gas Shift.
    ///     [Ibixstation]
    /// </summary>
    public const float WaterGasShiftReactionRate = 3f;

    /// <summary>
    ///     Energy released per mole of CO shifted to H2 via water vapor (41 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float WaterGasShiftEnergyReleased = 41000f;

    /// <summary>
    ///     Reaction rate divisor scaling the per-tick conversion for oxidative cyanogen synthesis.
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenOxidativeReactionRate = 4f;

    /// <summary>
    ///     Energy released per mole of Cyanogen produced via oxidative synthesis (180 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenOxidativeEnergyReleased = 180000f;

    /// <summary>
    ///     Reaction rate divisor scaling the base per-tick conversion for pyrolytic cyanogen synthesis.
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenPyrolysisReactionRate = 5f;

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
    public const float CyanogenCOReactionRate = 4.5f;

    /// <summary>
    ///     Energy released per mole of Cyanogen produced via carbon monoxide synthesis (45 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenCOEnergyReleased = 45000f;

    /// <summary>
    ///     Arrhenius activation energy constant controlling the exponential rate scaling of CO2 decomposition.
    ///     [Ibixstation]
    /// </summary>
    public const float CO2DecompositionActivationEnergy = 8140f;

    public const float AmmoniaDecompositionReactionRate = 4f;

    /// <summary>
    ///     Reaction rate divisor for Ammonia Synthesis.
    ///     [Ibixstation]
    /// </summary>
    public const float HighPressureAmmoniaSynthesisReactionRate = 3f;

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
    public const float ReverseWaterGasShiftReactionRate = 4f;

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
    public const float SteamMethaneReformingReactionRate = 3.8f;

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
    public const float FrezonProductionConversionRate = 8f;

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

    /// <summary>
    ///     Collection of all gases with thermal cracking behaviors.
    ///     Restricting loop lookups to this array prevents looping through unnecessary gases.
    ///     [Ibixstation]
    /// </summary>
    public static readonly Gas[] CrackableGases = new[]
    {
        Gas.Ammonia,
        Gas.WaterVapor,
        Gas.CarbonDioxide,
        Gas.NitrousOxide
    };
}

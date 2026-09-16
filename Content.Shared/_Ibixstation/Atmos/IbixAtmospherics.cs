using Content.Shared.Atmos;

namespace Content.Shared._Ibixstation.Atmos;

/// <summary>
///     Atmospheric constants and parameters specific to Ibixstation reactions.
/// </summary>
public static class IbixAtmospherics
{
    /// <summary>
    ///     Reaction rate divisor for Sabatier Reaction. Highly selective but multi-step.
    ///     Slower than simple shifts but faster than complex toxin creation.
    ///     [Ibixstation]
    /// </summary>
    public const float CO2MethanationReactionRate = 16f;

    /// <summary>
    ///     Minimum dynamic gas pressure threshold required to initiate the CO2 Sabatier Methanation process (1,000 kPa).
    ///     [Ibixstation]
    /// </summary>
    public const float CO2MethanationMinimumPressure = 1000000f;

    /// <summary>
    ///     Energy released per mole of Methane produced (165 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float CO2MethanationEnergyReleased = 165000f;

    /// <summary>
    ///     Reaction rate divisor for CO Methanation. Considerably faster than CO2 methanation due to simpler bond mechanisms.
    ///     [Ibixstation]
    /// </summary>
    public const float COMethanationReactionRate = 12f;

    /// <summary>
    ///     Minimum dynamic gas pressure threshold required to initiate the exothermic CO Methanation loop (1,000 kPa).
    ///     [Ibixstation]
    /// </summary>
    public const float COMethanationMinimumPressure = 1000000f;

    /// <summary>
    ///     Energy released per mole of Methane produced via CO methanation (206 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float COMethanationEnergyReleased = 206000f;

    /// <summary>
    ///     Reaction rate divisor for Water-Gas Shift. One of the fastest gas-phase shift reactions, placed right at the baseline.
    ///     [Ibixstation]
    /// </summary>
    public const float WaterGasShiftReactionRate = 10f;

    /// <summary>
    ///     Energy released per mole of CO shifted to H2 via water vapor (41 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float WaterGasShiftEnergyReleased = 41000f;

    /// <summary>
    ///     Reaction rate divisor scaling the base per-tick conversion for methane/ammonia cyanogen synthesis.
    ///     A highly complex molecular reorganization that acts as a maximum slower bottleneck process.
    ///     [Ibixstation]
    /// </summary>
    public const float MethaneAmmoniaCyanogenSynthesisReactionRate = 25f;

    /// <summary>
    ///     Maximum dynamic gas pressure threshold beyond which the volume-expanding Methane/Ammonia Cyanogen Synthesis shuts down (4,000 kPa).
    ///     [Ibixstation]
    /// </summary>
    public const float MethaneAmmoniaCyanogenSynthesisMaximumPressure = 4000000f;

    /// <summary>
    ///     Amount of energy absorbed per mole of Cyanogen produced via methane/ammonia (320 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float MethaneAmmoniaCyanogenSynthesisEnergyAbsorbed = 320000f;

    /// <summary>
    ///     Reaction rate divisor scaling the per-tick conversion for CO/Ammonia cyanogen synthesis.
    ///     Slightly faster than the Methane route but still a complex multi-molecule process.
    ///     [Ibixstation]
    /// </summary>
    public const float COAmmoniaCyanogenSynthesisReactionRate = 22f;

    /// <summary>
    ///     Energy absorbed per mole of Cyanogen produced via carbon monoxide synthesis (45 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float COAmmoniaCyanogenSynthesisEnergyAbsorbed = 45000f;

    /// <summary>
    ///     Reaction rate divisor for Ammonia Synthesis. Moderately slow under non-optimal atmospheric conditions.
    ///     [Ibixstation]
    /// </summary>
    public const float HaberBoschSynthesisReactionRate = 18f;

    /// <summary>
    ///     Minimum dynamic gas pressure threshold required to compress and initiate the Haber-Bosch chemical shift (1,000 kPa).
    ///     [Ibixstation]
    /// </summary>
    public const float HaberBoschSynthesisMinimumPressure = 1000000f;

    /// <summary>
    ///     Energy released per mole of Ammonia synthesized (46.2 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float HaberBoschSynthesisEnergyReleased = 46200f;

    /// <summary>
    ///     Reaction rate divisor scaling the per-tick conversion for Reverse Water-Gas Shift.
    ///     Slightly slower than the forward exothermic shift at equivalent energies.
    ///     [Ibixstation]
    /// </summary>
    public const float ReverseWaterGasShiftReactionRate = 12f;

    /// <summary>
    ///     Amount of energy absorbed per mole of CO2 shifted to CO via hydrogen (41 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float ReverseWaterGasShiftEnergyAbsorbed = 41000f;

    /// <summary>
    ///     Reaction rate divisor scaling the per-tick conversion for Steam Methane Reforming.
    ///     A heavily endothermic process that reacts sluggishly compared to simple shifts.
    ///     [Ibixstation]
    /// </summary>
    public const float MethaneSteamReformingReactionRate = 24f;

    /// <summary>
    ///     Maximum dynamic gas pressure threshold beyond which the volume-expanding Steam Methane Reforming reaction is suppressed (4,000 kPa).
    ///     [Ibixstation]
    /// </summary>
    public const float MethaneSteamReformingMaximumPressure = 4000000f;

    /// <summary>
    ///     Amount of energy absorbed per mole of Methane reformed via water vapor (206 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float MethaneSteamReformingEnergyAbsorbed = 206000f;

    /// <summary>
    ///     Reaction rate divisor scaling the per-tick conversion for Cyanogen Hydrolysis.
    ///     Water vapor attacks cyanogen readily, breaking it down at a moderate rate.
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenHydrolysisReactionRate = 15f;

    /// <summary>
    ///     Energy released per mole of Cyanogen consumed via hydrolysis (68 kJ/mol).
    ///     [Ibixstation]
    /// </summary>
    public const float CyanogenHydrolysisEnergyReleased = 68000f;

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
    public const float FrezonProductionConversionRate = 30f;

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
    ///     Reaction rate divisor scaling the per-tick conversion of Hydrogen to Tritium under ionizing radiation.
    ///     [Ibixstation]
    /// </summary>
    public const float HydrogenTritiumRadiationReactionRate = 50f;

    /// <summary>
    ///     Amount of thermal energy released per mole of Hydrogen transmuted to Tritium via neutron capture (scaled down from 818,000,000 MJ for gameplay).
    ///     [Ibixstation]
    /// </summary>
    public const float HydrogenTritiumRadiationEnergyReleased = 817f;

    public const float LightningBaseEnergyScaleDivisor = 10f;
    public const float LightningOzoneThermalCeiling = 473.15f;
    public const float LightningOzoneReactionRate = 5f;
    public const float LightningFixationReactionRate = 10f;
    public const float LightningCarbonDissociationRate = 8f;
    public const float LightningWaterSplittingRate = 12f;

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

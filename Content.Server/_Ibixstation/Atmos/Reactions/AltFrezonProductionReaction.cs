using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using Content.Shared._Ibixstation.Atmos;
using JetBrains.Annotations;

namespace Content.Server.Atmos.Reactions;

/// <summary>
///     Produces Frezon from Methane and Plasma, with Nitrogen acting as a catalytic bed.
///     Hydrogen is stripped from converted Methane and released as H2 gas.
///     Inefficient reactions collapse unreacted mass directly into Nitrogen gas.
/// </summary>
[UsedImplicitly]
public sealed partial class AltFrezonProductionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var temperature = mixture.Temperature;

        // SS14 standard ramp scaling
        var scale = (temperature - IbixAtmospherics.FrezonProductionCoolingLowerTemperature) /
                    (IbixAtmospherics.FrezonProductionCoolingMidTemperature - IbixAtmospherics.FrezonProductionCoolingLowerTemperature);

        if (scale <= 0f)
            return ReactionResult.NoReaction;

        scale = Math.Min(1f, scale);

        var initialN2 = mixture.GetMoles(Gas.Nitrogen);
        var initialMethane = mixture.GetMoles(Gas.Methane);
        var initialPlasma = mixture.GetMoles(Gas.Plasma);

        var efficiency = temperature / Atmospherics.FrezonProductionMaxEfficiencyTemperature;
        var loss = 1.0f - efficiency;

        // How much the catalyst (N2) will allow us to produce
        // Less N2 is required the more efficient it is.
        var catalystLimit = initialN2 * (Atmospherics.FrezonProductionNitrogenRatio / efficiency);
        var methaneLimitByPlasma = initialPlasma / IbixAtmospherics.FrezonProductionPlasmaRatio;

        // Amount of methane & plasma that are reacting
        var methaneBurned = Math.Min(initialMethane, Math.Min(methaneLimitByPlasma, catalystLimit));
        var plasmaBurned = methaneBurned * IbixAtmospherics.FrezonProductionPlasmaRatio;

        var methaneConversion = methaneBurned / IbixAtmospherics.FrezonProductionConversionRate;
        var plasmaConversion = plasmaBurned / IbixAtmospherics.FrezonProductionConversionRate;

        if (methaneConversion <= 0f)
            return ReactionResult.NoReaction;

        // 1 mole of Methane converts to 1 mole of Frezon at 100% efficiency
        var frezonProduced = methaneConversion * efficiency;

        // Each lost mole batch produces 4.5 moles of N2 (9 atoms)
        var lossProduced = (methaneConversion * loss) * 4.5f;

        // 1 mole of CH4 yields 2 moles of H2 gas (4 Hydrogen atoms stripped off)
        var hydrogenProduced = methaneConversion * 2.0f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.Methane, -methaneConversion);
        mixture.AdjustMoles(Gas.Plasma, -plasmaConversion);
        mixture.AdjustMoles(Gas.Frezon, frezonProduced);
        mixture.AdjustMoles(Gas.Nitrogen, lossProduced);
        mixture.AdjustMoles(Gas.Hydrogen, hydrogenProduced);

        // Apply temperature scale directly to cooling energy
        var energyReleased = frezonProduced * IbixAtmospherics.FrezonProductionEnergyAbsorbed * scale;
        energyReleased /= heatScale;

        if (energyReleased < 0f)
        {
            var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
            if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
            {
                mixture.Temperature = (temperature * oldHeatCapacity + energyReleased) / newHeatCapacity;
            }
        }

        return ReactionResult.Reacting;
    }
}

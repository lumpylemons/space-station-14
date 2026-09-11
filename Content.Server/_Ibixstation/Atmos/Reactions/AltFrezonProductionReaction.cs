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
        var initialN2 = mixture.GetMoles(Gas.Nitrogen);
        var initialMethane = mixture.GetMoles(Gas.Methane);
        var initialPlasma = mixture.GetMoles(Gas.Plasma);

        var efficiency = temperature / Atmospherics.FrezonProductionMaxEfficiencyTemperature;
        var loss = 1.0f - efficiency;

        // How much the catalyst (N2) will allow us to produce
        // Less N2 is required the more efficient it is.
        var catalystLimit = initialN2 * (Atmospherics.FrezonProductionNitrogenRatio / efficiency);
        var methaneLimit = Math.Min(initialPlasma, catalystLimit) / Atmospherics.FrezonProductionTritRatio;

        // Amount of methane & plasma that are reacting
        var methaneBurned = Math.Min(initialMethane, Math.Min(methaneLimit, catalystLimit));
        var plasmaBurned = methaneBurned * IbixAtmospherics.FrezonProductionPlasmaRatio;

        var methaneConversion = methaneBurned / IbixAtmospherics.FrezonProductionConversionRate;
        var plasmaConversion = plasmaBurned / IbixAtmospherics.FrezonProductionConversionRate;

        var frezonProduced = methaneConversion * efficiency;
        var lossProduced = methaneConversion * loss * 2.5f;
        var hydrogenProduced = methaneConversion * 2.0f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.Methane, -methaneConversion);
        mixture.AdjustMoles(Gas.Plasma, -plasmaConversion);
        mixture.AdjustMoles(Gas.Frezon, frezonProduced);
        mixture.AdjustMoles(Gas.Nitrogen, lossProduced);
        mixture.AdjustMoles(Gas.Hydrogen, hydrogenProduced);

        // Apply temperature scale directly to cooling energy
        var energyReleased = frezonProduced * IbixAtmospherics.FrezonProductionEnergyAbsorbed;
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

using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using Content.Shared._Ibixstation.Atmos;
using JetBrains.Annotations;
using Content.Server.Atmos;

namespace Content.Server._Ibixstation.Atmos.Reactions;

/// <summary>
///     Takes in Nitrogen and Frezon and cools down the surrounding area.
///     Spent Frezon oxygen binds with Nitrogen to produce Nitrous Oxide at a 5:1 ratio.
/// </summary>
[UsedImplicitly]
public sealed partial class AltFrezonCoolantReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        var temperature = mixture.Temperature;

        var energyModifier = 1f;
        var scale = (temperature - Atmospherics.FrezonCoolLowerTemperature) /
                    (Atmospherics.FrezonCoolMidTemperature - Atmospherics.FrezonCoolLowerTemperature);

        if (scale > 1f)
        {
            // Scale energy but not frezon usage if we're in a very, very hot place
            energyModifier = Math.Min(scale, Atmospherics.FrezonCoolMaximumEnergyModifier);
            scale = 1f;
        }
        else if (scale > 0f)
        {
            // Apply a square-root curve so the scale does not collapse at sub-zero temperatures.
            // Keeps burn throughput high enough to reach 23.15 K without burning through excess Nitrogen.
            scale = MathF.Sqrt(scale);
        }

        if (scale <= 0f)
            return ReactionResult.NoReaction;

        var initialNit = mixture.GetMoles(Gas.Nitrogen);
        var initialFrezon = mixture.GetMoles(Gas.Frezon);

        var burnRate = initialFrezon * scale / Atmospherics.FrezonCoolRateModifier;

        var energyReleased = 0f;
        if (burnRate > Atmospherics.MinimumHeatCapacity)
        {
            var frezonAmt = Math.Min(burnRate, initialFrezon);
            var nitAmt = Math.Min(frezonAmt * IbixAtmospherics.FrezonNitrogenCoolRatio, initialNit);

            // Re-cap Frezon to match available Nitrogen ratio
            frezonAmt = nitAmt / IbixAtmospherics.FrezonNitrogenCoolRatio;

            if (frezonAmt <= 0f)
                return ReactionResult.NoReaction;

            var n2OProduced = frezonAmt * IbixAtmospherics.FrezonN2OCoolOutputRatio;

            mixture.AdjustMoles(Gas.Nitrogen, -nitAmt);
            mixture.AdjustMoles(Gas.Frezon, -frezonAmt);
            mixture.AdjustMoles(Gas.NitrousOxide, n2OProduced);

            energyReleased = frezonAmt * IbixAtmospherics.FrezonCoolEnergyReleased * energyModifier;
        }

        energyReleased /= heatScale; // adjust energy to make sure speedup doesn't cause mega temperature rise
        if (energyReleased >= 0f)
            return ReactionResult.NoReaction;

        var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
            mixture.Temperature = (temperature * oldHeatCapacity + energyReleased) / newHeatCapacity;

        return ReactionResult.Reacting;
    }
}

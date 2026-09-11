using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class HaberBoschSynthesisReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Pressure < IbixAtmospherics.HaberBoschSynthesisMinimumPressure)
            return ReactionResult.NoReaction;

        var nNitrogen = mixture.GetMoles(Gas.Nitrogen);
        var nHydrogen = mixture.GetMoles(Gas.Hydrogen);
        var nTotal = mixture.TotalMoles;

        var maxNitrogenByHydrogen = nHydrogen / 3f;
        var reactingNitrogen = MathF.Min(nNitrogen, maxNitrogenByHydrogen);

        var fNitrogen = nNitrogen / nTotal;
        var fHydrogen = nHydrogen / nTotal;

        var collisionRate = fNitrogen * fHydrogen;

        collisionRate *= (mixture.Pressure / Atmospherics.OneAtmosphere);

        var deltaNitrogen = (reactingNitrogen / IbixAtmospherics.HaberBoschSynthesisReactionRate) * collisionRate;

        var deltaHydrogen = deltaNitrogen * 3f;
        var deltaAmmonia = deltaNitrogen * 2f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.Nitrogen, -deltaNitrogen);
        mixture.AdjustMoles(Gas.Hydrogen, -deltaHydrogen);
        mixture.AdjustMoles(Gas.Ammonia, deltaAmmonia);

        var energyReleased = deltaAmmonia * IbixAtmospherics.HaberBoschSynthesisEnergyReleased;
        energyReleased /= heatScale;

        if (energyReleased > 0f)
        {
            var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
            if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
            {
                mixture.Temperature = (mixture.Temperature * oldHeatCapacity + energyReleased) / newHeatCapacity;
            }
        }

        return ReactionResult.Reacting;
    }
}

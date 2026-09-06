using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class AmmoniaDecompositionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var temperature = mixture.Temperature;
        var nAmmonia = mixture.GetMoles(Gas.Ammonia);
        var nTotal = mixture.TotalMoles;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        var deltaAmmonia = nAmmonia / IbixAtmospherics.AmmoniaDecompositionReactionRate * (nAmmonia / nTotal);
        var ammoniaDecomposed = MathF.Min(deltaAmmonia, nAmmonia);

        var deltaNitrogen = ammoniaDecomposed * 0.5f;
        var deltaHydrogen = ammoniaDecomposed * 1.5f;

        mixture.AdjustMoles(Gas.Ammonia, -ammoniaDecomposed);
        mixture.AdjustMoles(Gas.Nitrogen, deltaNitrogen);
        mixture.AdjustMoles(Gas.Hydrogen, deltaHydrogen);

        var energyAbsorbed = ammoniaDecomposed * IbixAtmospherics.AmmoniaDecompositionEnergyAbsorbed;
        energyAbsorbed /= heatScale;

        var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
        {
            mixture.Temperature = MathF.Max(
                Atmospherics.TCMB,
                (temperature * oldHeatCapacity - energyAbsorbed) / newHeatCapacity
            );
        }

        return ReactionResult.Reacting;
    }
}

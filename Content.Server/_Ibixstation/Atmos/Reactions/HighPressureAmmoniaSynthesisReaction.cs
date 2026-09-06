using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class HighPressureAmmoniaSynthesisReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var pressure = mixture.Pressure;
        if (pressure < 8500f)
            return ReactionResult.NoReaction;

        var nNitrogen = mixture.GetMoles(Gas.Nitrogen);
        var nHydrogen = mixture.GetMoles(Gas.Hydrogen);
        var nTotal = mixture.TotalMoles;

        var maxByNitrogen = nNitrogen;
        var maxByHydrogen = nHydrogen / 3f;
        var maxReactingUnits = MathF.Min(maxByNitrogen, maxByHydrogen);

        var fNitrogen = nNitrogen / nTotal;
        var fHydrogen = nHydrogen / nTotal;

        var pressureDelta = MathF.Max(0f, pressure - 8500f);
        var pressureModifier = MathF.Pow(pressureDelta / 500f, 2f);

        var rate = fNitrogen * MathF.Pow(fHydrogen, 3f) * pressureModifier;
        var deltaUnits = maxReactingUnits / IbixAtmospherics.HighPressureAmmoniaSynthesisReactionRate * rate;

        var deltaNitrogen = deltaUnits;
        var deltaHydrogen = deltaUnits * 3f;
        var deltaAmmonia = deltaUnits * 2f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.Nitrogen, -deltaNitrogen);
        mixture.AdjustMoles(Gas.Hydrogen, -deltaHydrogen);
        mixture.AdjustMoles(Gas.Ammonia, deltaAmmonia);

        var energyReleased = deltaAmmonia * IbixAtmospherics.HighPressureAmmoniaSynthesisEnergyReleased;
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

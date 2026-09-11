using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class CO2MethanationReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Pressure < IbixAtmospherics.CO2MethanationMinimumPressure)
            return ReactionResult.NoReaction;

        var nCO2 = mixture.GetMoles(Gas.CarbonDioxide);
        var nH2 = mixture.GetMoles(Gas.Hydrogen);
        var nTotal = mixture.TotalMoles;

        var maxCO2ByHydrogen = nH2 / 4f;
        var reactingCO2 = MathF.Min(nCO2, maxCO2ByHydrogen);

        var fCO2 = nCO2 / nTotal;
        var fHydrogen = nH2 / nTotal;

        var collisionRate = fCO2 * fHydrogen;

        collisionRate *= (mixture.Pressure / Atmospherics.OneAtmosphere);

        var deltaCO2 = (reactingCO2 / IbixAtmospherics.CO2MethanationReactionRate) * collisionRate;

        var deltaHydrogen = deltaCO2 * 4f;
        var deltaMethane = deltaCO2;
        var deltaWater = deltaCO2 * 2f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.CarbonDioxide, -deltaCO2);
        mixture.AdjustMoles(Gas.Hydrogen, -deltaHydrogen);
        mixture.AdjustMoles(Gas.Methane, deltaMethane);
        mixture.AdjustMoles(Gas.WaterVapor, deltaWater);

        var energyReleased = deltaMethane * IbixAtmospherics.CO2MethanationEnergyReleased;
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

using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class WaterGasShiftReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var nCO = mixture.GetMoles(Gas.CarbonMonoxide);
        var nWater = mixture.GetMoles(Gas.WaterVapor);
        var nTotal = mixture.TotalMoles;

        var maxCOByWater = nWater;
        var reactingCO = MathF.Min(nCO, maxCOByWater);

        var fCO = nCO / nTotal;
        var fWater = nWater / nTotal;

        var collisionRate = fCO * fWater;

        var thermalBlend = MathF.Max(0f, 1f - ((mixture.Temperature - 300f) / 700f));
        collisionRate *= thermalBlend;

        var deltaCO = (reactingCO / IbixAtmospherics.WaterGasShiftReactionRate) * collisionRate;

        var deltaWater = deltaCO;
        var deltaCO2 = deltaCO;
        var deltaHydrogen = deltaCO;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.CarbonMonoxide, -deltaCO);
        mixture.AdjustMoles(Gas.WaterVapor, -deltaWater);
        mixture.AdjustMoles(Gas.CarbonDioxide, deltaCO2);
        mixture.AdjustMoles(Gas.Hydrogen, deltaHydrogen);

        var energyReleased = deltaCO2 * IbixAtmospherics.WaterGasShiftEnergyReleased;
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

using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class ReverseWaterGasShiftReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var nCO2 = mixture.GetMoles(Gas.CarbonDioxide);
        var nHydrogen = mixture.GetMoles(Gas.Hydrogen);
        var nTotal = mixture.TotalMoles;

        var maxCO2ByHydrogen = nHydrogen;
        var reactingCO2 = MathF.Min(nCO2, maxCO2ByHydrogen);

        var fCO2 = nCO2 / nTotal;
        var fHydrogen = nHydrogen / nTotal;

        var collisionRate = fCO2 * fHydrogen;

        var thermalBlend = MathF.Min(1f, MathF.Max(0f, (mixture.Temperature - 1000f) / 1000f));
        collisionRate *= thermalBlend;

        var deltaCO2 = (reactingCO2 / IbixAtmospherics.ReverseWaterGasShiftReactionRate) * collisionRate;

        var deltaHydrogen = deltaCO2;
        var deltaCO = deltaCO2;
        var deltaWater = deltaCO2;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.CarbonDioxide, -deltaCO2);
        mixture.AdjustMoles(Gas.Hydrogen, -deltaHydrogen);
        mixture.AdjustMoles(Gas.CarbonMonoxide, deltaCO);
        mixture.AdjustMoles(Gas.WaterVapor, deltaWater);

        var energyAbsorbed = deltaCO * IbixAtmospherics.ReverseWaterGasShiftEnergyAbsorbed;
        energyAbsorbed /= heatScale;

        if (energyAbsorbed > 0f)
        {
            var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
            if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
            {
                mixture.Temperature = (mixture.Temperature * oldHeatCapacity - energyAbsorbed) / newHeatCapacity;
            }
        }

        return ReactionResult.Reacting;
    }
}

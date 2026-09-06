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

        var reactingMoles = MathF.Min(nCO, nWater);

        var fCO = nCO / nTotal;
        var fWater = nWater / nTotal;

        var limitingFraction = MathF.Min(fCO, fWater);

        var shiftRate = reactingMoles / IbixAtmospherics.WaterGasShiftReactionRate * limitingFraction;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.CarbonMonoxide, -shiftRate);
        mixture.AdjustMoles(Gas.WaterVapor, -shiftRate);
        mixture.AdjustMoles(Gas.CarbonDioxide, shiftRate);
        mixture.AdjustMoles(Gas.Hydrogen, shiftRate);

        var energyReleased = shiftRate * IbixAtmospherics.WaterGasShiftEnergyReleased;
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

using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class CyanogenCOSynthesisReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var nCO = mixture.GetMoles(Gas.CarbonMonoxide);
        var nAmmonia = mixture.GetMoles(Gas.Ammonia);
        var nTotal = mixture.TotalMoles;

        var maxByCO = nCO / 2f;
        var maxByAmmonia = nAmmonia / 2f;

        var reactingUnits = MathF.Min(maxByCO, maxByAmmonia);

        var fCO = nCO / nTotal;
        var fAmmonia = nAmmonia / nTotal;

        var rate = fCO * fAmmonia;

        var deltaCyanogen = reactingUnits / IbixAtmospherics.CyanogenCOReactionRate * rate;

        var deltaCO = deltaCyanogen * 2f;
        var deltaAmmonia = deltaCyanogen * 2f;
        var deltaWater = deltaCyanogen * 2f;
        var deltaHydrogen = deltaCyanogen;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.CarbonMonoxide, -deltaCO);
        mixture.AdjustMoles(Gas.Ammonia, -deltaAmmonia);
        mixture.AdjustMoles(Gas.Cyanogen, deltaCyanogen);
        mixture.AdjustMoles(Gas.WaterVapor, deltaWater);
        mixture.AdjustMoles(Gas.Hydrogen, deltaHydrogen);

        var energyReleased = deltaCyanogen * IbixAtmospherics.CyanogenCOEnergyReleased;
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

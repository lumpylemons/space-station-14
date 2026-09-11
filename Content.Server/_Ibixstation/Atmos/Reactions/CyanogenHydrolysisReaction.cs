using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class CyanogenHydrolysisReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var nCyanogen = mixture.GetMoles(Gas.Cyanogen);
        var nWater = mixture.GetMoles(Gas.WaterVapor);
        var nTotal = mixture.TotalMoles;

        var maxCyanogenByWater = nWater / 2f;
        var reactingCyanogen = MathF.Min(nCyanogen, maxCyanogenByWater);

        var fCyanogen = nCyanogen / nTotal;
        var fWater = nWater / nTotal;

        var collisionRate = fCyanogen * fWater;

        var deltaCyanogen = (reactingCyanogen / IbixAtmospherics.CyanogenHydrolysisReactionRate) * collisionRate;

        var deltaWater = deltaCyanogen * 2f;
        var deltaCO2 = deltaCyanogen;
        var deltaCO = deltaCyanogen;
        var deltaAmmonia = deltaCyanogen * 2f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.Cyanogen, -deltaCyanogen);
        mixture.AdjustMoles(Gas.WaterVapor, -deltaWater);
        mixture.AdjustMoles(Gas.CarbonDioxide, deltaCO2);
        mixture.AdjustMoles(Gas.CarbonMonoxide, deltaCO);
        mixture.AdjustMoles(Gas.Ammonia, deltaAmmonia);

        var energyReleased = deltaCyanogen * IbixAtmospherics.CyanogenHydrolysisEnergyReleased;
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

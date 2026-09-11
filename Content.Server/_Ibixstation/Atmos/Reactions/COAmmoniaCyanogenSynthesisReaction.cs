using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class COAmmoniaCyanogenSynthesisReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var nCO = mixture.GetMoles(Gas.CarbonMonoxide);
        var nAmmonia = mixture.GetMoles(Gas.Ammonia);
        var nTotal = mixture.TotalMoles;

        var maxCOByAmmonia = nAmmonia;
        var reactingCO = MathF.Min(nCO, maxCOByAmmonia);

        var fCO = nCO / nTotal;
        var fAmmonia = nAmmonia / nTotal;

        var collisionRate = fCO * fAmmonia;

        var deltaCO = (reactingCO / IbixAtmospherics.COAmmoniaCyanogenSynthesisReactionRate) * collisionRate;

        var deltaAmmonia = deltaCO;
        var deltaCyanogen = deltaCO / 2f;
        var deltaWater = deltaCO;
        var deltaHydrogen = deltaCO;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.CarbonMonoxide, -deltaCO);
        mixture.AdjustMoles(Gas.Ammonia, -deltaAmmonia);
        mixture.AdjustMoles(Gas.Cyanogen, deltaCyanogen);
        mixture.AdjustMoles(Gas.WaterVapor, deltaWater);
        mixture.AdjustMoles(Gas.Hydrogen, deltaHydrogen);

        var energyAbsorbed = deltaCyanogen * IbixAtmospherics.COAmmoniaCyanogenSynthesisEnergyAbsorbed;
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

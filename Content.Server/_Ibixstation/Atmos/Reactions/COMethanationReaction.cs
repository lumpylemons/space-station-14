using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class COMethanationReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var nCO = mixture.GetMoles(Gas.CarbonMonoxide);
        var nTrit = mixture.GetMoles(Gas.Hydrogen);
        var nTotal = mixture.TotalMoles;

        var maxCOByTrit = nTrit / 3f;
        var reactingCO = MathF.Min(nCO, maxCOByTrit);

        var fCO = nCO / nTotal;
        var fTrit = nTrit / nTotal;

        var rate = fCO * MathF.Pow(fTrit, 2);

        var deltaCO = reactingCO / IbixAtmospherics.COMethanationReactionRate * rate;

        var deltaTrit = deltaCO * 3f;
        var deltaMethane = deltaCO;
        var deltaWater = deltaCO;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.CarbonMonoxide, -deltaCO);
        mixture.AdjustMoles(Gas.Hydrogen, -deltaTrit);
        mixture.AdjustMoles(Gas.Methane, deltaMethane);
        mixture.AdjustMoles(Gas.WaterVapor, deltaWater);

        var energyReleased = deltaMethane * IbixAtmospherics.COMethanationEnergyReleased;
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

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
        if (mixture.Pressure < IbixAtmospherics.COMethanationMinimumPressure)
            return ReactionResult.NoReaction;

        var nCO = mixture.GetMoles(Gas.CarbonMonoxide);
        var nH2 = mixture.GetMoles(Gas.Hydrogen);
        var nTotal = mixture.TotalMoles;

        var maxCOByH2 = nH2 / 3f;
        var reactingCO = MathF.Min(nCO, maxCOByH2);

        var fCO = nCO / nTotal;
        var fH2 = nH2 / nTotal;

        var collisionRate = fCO * fH2;

        collisionRate *= (mixture.Pressure / Atmospherics.OneAtmosphere);

        var deltaCO = (reactingCO / IbixAtmospherics.COMethanationReactionRate) * collisionRate;

        var deltaH2 = deltaCO * 3f;
        var deltaMethane = deltaCO;
        var deltaWater = deltaCO;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.CarbonMonoxide, -deltaCO);
        mixture.AdjustMoles(Gas.Hydrogen, -deltaH2);
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

using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class SabatierReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var nCO2 = mixture.GetMoles(Gas.CarbonDioxide);
        var nTrit = mixture.GetMoles(Gas.Tritium);
        var nTotal = mixture.TotalMoles;

        if (nCO2 <= 0f || nTrit <= 0f || nTotal <= 0f)
            return ReactionResult.NoReaction;

        var maxCO2ByTrit = nTrit / 4f;
        var reactingCO2 = MathF.Min(nCO2, maxCO2ByTrit);

        var fCO2 = nCO2 / nTotal;
        var fTrit = nTrit / nTotal;

        var rate = fCO2 * MathF.Pow(fTrit, 2);

        var deltaCO2 = reactingCO2 / IbixAtmospherics.SabatierReactionRate * rate;

        if (deltaCO2 <= 0f || nCO2 - deltaCO2 < 0)
            return ReactionResult.NoReaction;

        var deltaTrit = deltaCO2 * 4f;
        var deltaMethane = deltaCO2;
        var deltaWater = deltaCO2 * 2f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.CarbonDioxide, -deltaCO2);
        mixture.AdjustMoles(Gas.Tritium, -deltaTrit);
        mixture.AdjustMoles(Gas.Methane, deltaMethane);
        mixture.AdjustMoles(Gas.WaterVapor, deltaWater);

        var energyReleased = deltaMethane * IbixAtmospherics.SabatierEnergyReleased;
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

using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class MethaneSteamReformingReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Pressure > IbixAtmospherics.MethaneSteamReformingMaximumPressure)
            return ReactionResult.NoReaction;

        var nMethane = mixture.GetMoles(Gas.Methane);
        var nWater = mixture.GetMoles(Gas.WaterVapor);
        var nTotal = mixture.TotalMoles;

        var maxMethaneByWater = nWater;
        var reactingMethane = MathF.Min(nMethane, maxMethaneByWater);

        var fMethane = nMethane / nTotal;
        var fWater = nWater / nTotal;

        var collisionRate = fMethane * fWater;

        collisionRate /= MathF.Max(1.0f, mixture.Pressure / Atmospherics.OneAtmosphere);

        var deltaMethane = (reactingMethane / IbixAtmospherics.MethaneSteamReformingReactionRate) * collisionRate;

        var deltaWater = deltaMethane;
        var deltaCO = deltaMethane;
        var deltaHydrogen = deltaMethane * 3f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.Methane, -deltaMethane);
        mixture.AdjustMoles(Gas.WaterVapor, -deltaWater);
        mixture.AdjustMoles(Gas.CarbonMonoxide, deltaCO);
        mixture.AdjustMoles(Gas.Hydrogen, deltaHydrogen);

        var energyAbsorbed = deltaMethane * IbixAtmospherics.MethaneSteamReformingEnergyAbsorbed;
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

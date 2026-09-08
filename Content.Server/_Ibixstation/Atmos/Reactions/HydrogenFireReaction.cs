using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

/// <summary>
///     Exothermic combustion of Hydrogen and Oxygen to form Water Vapor.
/// </summary>
[UsedImplicitly]
public sealed partial class HydrogenFireReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var dynamicIgnitionThreshold = GasReactionHelpers.GetDynamicIgnitionThreshold(mixture, atmosphereSystem);
        if (mixture.Temperature < dynamicIgnitionThreshold)
            return ReactionResult.NoReaction;

        var energyReleased = 0f;
        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        var temperature = mixture.Temperature;
        var location = holder as TileAtmosphere;
        mixture.ReactionResults[(byte)GasReaction.Fire] = 0;

        var initialHydrogen = mixture.GetMoles(Gas.Hydrogen);
        var initialOxygen = mixture.GetMoles(Gas.Oxygen);

        var burnRate = MathF.Min(initialHydrogen, initialOxygen * 2f) / IbixAtmospherics.HydrogenBurnRateDelta;

        if (burnRate > Atmospherics.MinimumHeatCapacity)
        {
            var tritBurned = MathF.Min(burnRate, initialHydrogen);
            var oxyBurned = tritBurned * 0.5f;

            mixture.AdjustMoles(Gas.Hydrogen, -tritBurned);
            mixture.AdjustMoles(Gas.Oxygen, -oxyBurned);
            mixture.AdjustMoles(Gas.WaterVapor, tritBurned);

            energyReleased = tritBurned * IbixAtmospherics.FireHydrogenEnergyReleased;
            energyReleased /= heatScale;
            mixture.ReactionResults[(byte)GasReaction.Fire] += tritBurned + oxyBurned;
        }

        if (energyReleased > 0)
        {
            var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
            if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
                mixture.Temperature = (temperature * oldHeatCapacity + energyReleased) / newHeatCapacity;
        }

        if (location != null && mixture.Temperature > Atmospherics.FireMinimumTemperatureToExist)
        {
            atmosphereSystem.HotspotExpose(location, mixture.Temperature, mixture.Volume);
        }

        return mixture.ReactionResults[(byte)GasReaction.Fire] > 0 ? ReactionResult.Reacting : ReactionResult.NoReaction;
    }
}

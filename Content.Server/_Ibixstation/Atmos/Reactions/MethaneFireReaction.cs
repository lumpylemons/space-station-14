using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class MethaneFireReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var energyReleased = 0f;
        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        var temperature = mixture.Temperature;
        var location = holder as TileAtmosphere;
        mixture.ReactionResults[(byte)GasReaction.Fire] = 0;

        var initialMethane = mixture.GetMoles(Gas.Methane);
        var initialOxygen = mixture.GetMoles(Gas.Oxygen);

        var burnRate = MathF.Min(initialMethane, initialOxygen / 2f) / IbixAtmospherics.MethaneBurnRateDelta;

        if (burnRate > Atmospherics.MinimumHeatCapacity)
        {
            var methaneBurned = MathF.Min(burnRate, initialMethane);
            var oxyBurned = methaneBurned * 2f;

            mixture.AdjustMoles(Gas.Methane, -methaneBurned);
            mixture.AdjustMoles(Gas.Oxygen, -oxyBurned);
            mixture.AdjustMoles(Gas.CarbonDioxide, methaneBurned);
            mixture.AdjustMoles(Gas.WaterVapor, methaneBurned * 2f);

            energyReleased = methaneBurned * IbixAtmospherics.FireMethaneEnergyReleased;
            energyReleased /= heatScale;
            mixture.ReactionResults[(byte)GasReaction.Fire] += methaneBurned + oxyBurned;
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

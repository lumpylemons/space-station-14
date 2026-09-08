using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class AmmoniaFireReaction : IGasReactionEffect
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

        var initialAmmonia = mixture.GetMoles(Gas.Ammonia);
        var initialOxygen = mixture.GetMoles(Gas.Oxygen);

        var burnRate = MathF.Min(initialAmmonia, initialOxygen / 0.75f) / IbixAtmospherics.AmmoniaBurnRateDelta;

        if (burnRate > Atmospherics.MinimumHeatCapacity)
        {
            var ammoniaBurned = MathF.Min(burnRate, initialAmmonia);
            var oxyBurned = ammoniaBurned * 0.75f;

            mixture.AdjustMoles(Gas.Ammonia, -ammoniaBurned);
            mixture.AdjustMoles(Gas.Oxygen, -oxyBurned);
            mixture.AdjustMoles(Gas.Nitrogen, ammoniaBurned * 0.5f);
            mixture.AdjustMoles(Gas.WaterVapor, ammoniaBurned * 1.5f);

            energyReleased = ammoniaBurned * IbixAtmospherics.FireAmmoniaEnergyReleased;
            energyReleased /= heatScale;
            mixture.ReactionResults[(byte)GasReaction.Fire] += ammoniaBurned + oxyBurned;
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

using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos.Prototypes;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class FireReaction : IGasReactionEffect
{
    private struct FuelBurnData
    {
        public Gas GasId;
        public GasFuelProperties Fuel;
        public float AvailableMoles;
        public float DesiredOxygen;
        public float DissociationFactor;
        public float ActualFuelBurned;
    }

    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var currentTemp = mixture.Temperature;
        var dynamicIgnitionThreshold = GasReactionHelpers.GetDynamicIgnitionThreshold(mixture, atmosphereSystem);

        if (holder is TileAtmosphere tile && tile.Hotspot.Valid && currentTemp > Atmospherics.FireMinimumTemperatureToExist)
        {
            dynamicIgnitionThreshold = Atmospherics.FireMinimumTemperatureToExist;
        }

        if (currentTemp < dynamicIgnitionThreshold)
            return ReactionResult.NoReaction;

        var initialOxygen = mixture.GetMoles(Gas.Oxygen);
        if (initialOxygen <= 0f)
            return ReactionResult.NoReaction;

        // Fuel data gathering
        var activeFuels = new FuelBurnData[Atmospherics.TotalNumberOfGases];
        var fuelCount = 0;
        var totalOxygenDemanded = 0f;

        foreach (var gasId in Enum.GetValues<Gas>())
        {
            var moles = mixture.GetMoles(gasId);
            if (moles <= 0f)
                continue;

            var gasProto = atmosphereSystem.GetGas((int)gasId);
            if (!gasProto.IsFuel || gasProto.FuelProperties == null)
                continue;

            var fuel = gasProto.FuelProperties.Value;

            // Compute dissociation factor based on current mixture temperature
            var dissociationFactor = 0f;
            if (currentTemp > fuel.DissociationOnsetTemperature && fuel.MaxFlameTemperature > fuel.DissociationOnsetTemperature)
            {
                var range = fuel.MaxFlameTemperature - fuel.DissociationOnsetTemperature;
                var progress = currentTemp - fuel.DissociationOnsetTemperature;
                dissociationFactor = MathF.Min(1.0f, progress / range);
            }

            if (dissociationFactor >= 1.0f)
                continue; // Completely dissociated; cannot burn further

            // Max potential burn based on tick rate limits
            var maxFuelBurn = moles / fuel.BurnRateDelta;
            if (maxFuelBurn <= Atmospherics.MinimumHeatCapacity)
                continue;

            var fuelBurnedTarget = MathF.Min(maxFuelBurn, moles);
            var oxygenDemanded = fuelBurnedTarget * fuel.OxygenRatio;

            activeFuels[fuelCount++] = new FuelBurnData
            {
                GasId = gasId,
                Fuel = fuel,
                AvailableMoles = moles,
                DesiredOxygen = oxygenDemanded,
                DissociationFactor = dissociationFactor,
                ActualFuelBurned = fuelBurnedTarget
            };

            totalOxygenDemanded += oxygenDemanded;
        }

        if (fuelCount == 0)
            return ReactionResult.NoReaction;

        // Proportional oxygen scaling (Handles limited oxygen realistic parallel competition)
        var oxygenScaling = 1.0f;
        if (totalOxygenDemanded > initialOxygen)
        {
            oxygenScaling = initialOxygen / totalOxygenDemanded;
        }

        // Execute unified burn step
        var totalEnergyReleased = 0f;
        var totalGasReacted = 0f;
        var totalOxygenBurned = 0f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        for (var i = 0; i < fuelCount; i++)
        {
            ref var data = ref activeFuels[i];

            // Apply oxygen limit scaling to actual burned quantity
            var finalFuelBurned = data.ActualFuelBurned * oxygenScaling;
            var finalOxygenBurned = finalFuelBurned * data.Fuel.OxygenRatio;

            if (finalFuelBurned <= 0f)
                continue;

            // Deduct reactants
            mixture.AdjustMoles(data.GasId, -finalFuelBurned);
            totalOxygenBurned += finalOxygenBurned;

            // Generate products
            var effFactor = 1.0f - data.DissociationFactor;

            // Check for plasma supersaturation
            if (data.GasId == Gas.Plasma && (initialOxygen / data.AvailableMoles) >= Atmospherics.SuperSaturationThreshold)
            {
                // Supersaturated plasma converts directly into hydrogen
                // currently magically transmogrifies oxygen into hydrogen
                var hydrogenProduced = finalFuelBurned * 3.0f;
                mixture.AdjustMoles(Gas.Hydrogen, hydrogenProduced);
            }
            else
            {
                // Standard fuel burn using prototype data
                foreach (var (productGas, ratio) in data.Fuel.StandardProducts)
                {
                    var amount = finalFuelBurned * ratio * effFactor;
                    if (amount > 0f)
                        mixture.AdjustMoles(productGas, amount);
                }

                foreach (var (productGas, ratio) in data.Fuel.DissociationProducts)
                {
                    var amount = finalFuelBurned * ratio * data.DissociationFactor;
                    if (amount > 0f)
                        mixture.AdjustMoles(productGas, amount);
                }
            }

            // Calculate energy
            totalEnergyReleased += finalFuelBurned * data.Fuel.EnergyReleased * effFactor;
            totalGasReacted += (finalFuelBurned + finalOxygenBurned);
        }

        // Deduct combined Oxygen pool
        mixture.AdjustMoles(Gas.Oxygen, -totalOxygenBurned);
        mixture.ReactionResults[(byte)GasReaction.Fire] = totalGasReacted;

        // Apply energy and update temperature
        totalEnergyReleased /= heatScale;
        if (totalEnergyReleased > 0f && oldHeatCapacity > Atmospherics.MinimumHeatCapacity)
        {
            var totalThermalEnergy = (currentTemp * oldHeatCapacity) + totalEnergyReleased;
            var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

            if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
                mixture.Temperature = totalThermalEnergy / newHeatCapacity;
        }

        if (holder is TileAtmosphere location && mixture.Temperature > Atmospherics.FireMinimumTemperatureToExist)
        {
            atmosphereSystem.HotspotExpose(location, mixture.Temperature, mixture.Volume);
        }

        return totalGasReacted > 0f ? ReactionResult.Reacting : ReactionResult.NoReaction;
    }
}

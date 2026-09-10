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
        public float DesiredOxygenEq;
        public float DissociationFactor;
        public float ActualFuelBurned;
    }

    private struct OxidizerData
    {
        public Gas GasId;
        public GasOxidizerProperties Oxidizer;
        public float AvailableMoles;
        public float OxygenEquivalentMoles;
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

        // Gather all active oxidizers and calculate total available oxygen equivalents
        var activeOxidizers = new OxidizerData[Atmospherics.TotalNumberOfGases];
        var oxidizerCount = 0;
        var totalOxygenEqAvailable = 0f;

        foreach (var gasId in Enum.GetValues<Gas>())
        {
            var moles = mixture.GetMoles(gasId);
            if (moles <= 0f)
                continue;

            var gasProto = atmosphereSystem.GetGas((int)gasId);
            if (!gasProto.IsOxidizer || gasProto.OxidizerProperties == null)
                continue;

            var oxidizer = gasProto.OxidizerProperties.Value;
            var oxEq = moles * oxidizer.Power;

            if (oxEq <= 0f)
                continue;

            activeOxidizers[oxidizerCount++] = new OxidizerData
            {
                GasId = gasId,
                Oxidizer = oxidizer,
                AvailableMoles = moles,
                OxygenEquivalentMoles = oxEq
            };

            totalOxygenEqAvailable += oxEq;
        }

        if (totalOxygenEqAvailable <= 0f)
            return ReactionResult.NoReaction;

        // Gather active fuels and calculate total oxygen equivalents demanded
        var activeFuels = new FuelBurnData[Atmospherics.TotalNumberOfGases];
        var fuelCount = 0;
        var totalOxygenEqDemanded = 0f;

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

            var maxFuelBurn = moles / fuel.BurnRateDelta;
            if (maxFuelBurn <= Atmospherics.MinimumHeatCapacity)
                continue;

            var fuelBurnedTarget = MathF.Min(maxFuelBurn, moles);
            var oxygenEqDemanded = fuelBurnedTarget * fuel.OxygenRatio;

            activeFuels[fuelCount++] = new FuelBurnData
            {
                GasId = gasId,
                Fuel = fuel,
                AvailableMoles = moles,
                DesiredOxygenEq = oxygenEqDemanded,
                DissociationFactor = dissociationFactor,
                ActualFuelBurned = fuelBurnedTarget
            };

            totalOxygenEqDemanded += oxygenEqDemanded;
        }

        if (fuelCount == 0)
            return ReactionResult.NoReaction;

        // Proportional burn scaling based on fuel vs oxidizer capacity limits
        var capacityScaling = 1.0f;
        if (totalOxygenEqDemanded > totalOxygenEqAvailable)
        {
            capacityScaling = totalOxygenEqAvailable / totalOxygenEqDemanded;
        }

        // Calculate the proportion of total oxidizers consumed during this step
        var totalOxygenEqBurned = totalOxygenEqDemanded * capacityScaling;
        var oxidizerConsumptionRatio = totalOxygenEqAvailable > 0f ? (totalOxygenEqBurned / totalOxygenEqAvailable) : 0f;

        // Execute unified burn step
        var totalEnergyReleased = 0f;
        var totalGasReacted = 0f;
        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        // Burn the fuels and generate products
        for (var i = 0; i < fuelCount; i++)
        {
            ref var data = ref activeFuels[i];

            // Apply scaling to actual burned quantity
            var finalFuelBurned = data.ActualFuelBurned * capacityScaling;
            if (finalFuelBurned <= 0f)
                continue;

            // Deduct reactants
            mixture.AdjustMoles(data.GasId, -finalFuelBurned);
            totalGasReacted += finalFuelBurned;

            // Generate products
            var effFactor = 1.0f - data.DissociationFactor;

            // Check for plasma supersaturation
            if (data.GasId == Gas.Plasma && (totalOxygenEqAvailable / data.AvailableMoles) >= Atmospherics.SuperSaturationThreshold)
            {
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
        }

        // Burn the oxidizers and generate byproducts
        for (var i = 0; i < oxidizerCount; i++)
        {
            ref var data = ref activeOxidizers[i];

            // Consumed moles map to the shared pool's total consumption percentage
            var finalOxidizerBurned = data.AvailableMoles * oxidizerConsumptionRatio;
            if (finalOxidizerBurned <= 0f)
                continue;

            mixture.AdjustMoles(data.GasId, -finalOxidizerBurned);
            totalGasReacted += finalOxidizerBurned;

            // Generate unique side products or byproducts assigned to this specific oxidizer
            foreach (var (productGas, ratio) in data.Oxidizer.Byproducts)
            {
                var amount = finalOxidizerBurned * ratio;
                if (amount > 0f)
                    mixture.AdjustMoles(productGas, amount);
            }
        }

        // Apply energy modifications and update temperature
        mixture.ReactionResults[(byte)GasReaction.Fire] = totalGasReacted;
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

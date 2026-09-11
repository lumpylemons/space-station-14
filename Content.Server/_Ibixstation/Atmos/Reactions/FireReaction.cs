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
    private struct StagedFuelData
    {
        public int GasIndex;
        public float TargetBurn;
        public float DissociationFactor;
        public float AvailableMoles;
        public GasFuelProperties FuelProps;
    }

    private struct StagedOxidizerData
    {
        public int GasIndex;
        public float AvailableMoles;
        public GasOxidizerProperties OxProps;
    }

    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var currentTemp = mixture.Temperature;
        var dynamicIgnitionThreshold = GasReactionHelpers.GetDynamicIgnitionThreshold(mixture, atmosphereSystem);

        // Already lit fires on tiles sustain themselves at 100C regardless of the fuel type
        var tile = holder as TileAtmosphere;
        if (tile != null && tile.Hotspot.Valid && currentTemp > Atmospherics.FireMinimumTemperatureToExist)
        {
            dynamicIgnitionThreshold = Atmospherics.FireMinimumTemperatureToExist;
        }

        if (currentTemp < dynamicIgnitionThreshold)
            return ReactionResult.NoReaction;

        var activeOxidizers = new StagedOxidizerData[Atmospherics.TotalNumberOfGases];
        var activeFuels = new StagedFuelData[Atmospherics.TotalNumberOfGases];

        var oxidizerCount = 0;
        var fuelCount = 0;

        var totalOxygenEqAvailable = 0f;
        var totalOxygenEqDemanded = 0f;

        // Process fuels and oxidizers
        for (var i = 0; i < Atmospherics.TotalNumberOfGases; i++)
        {
            var moles = mixture.GetMoles(i);
            if (moles < Atmospherics.GasMinMoles)
                continue;

            var gasProto = atmosphereSystem.GetGas(i);

            // Process oxidizers
            if (gasProto.IsOxidizer && gasProto.OxidizerProperties != null)
            {
                var oxidizer = gasProto.OxidizerProperties.Value;
                var oxEq = moles * oxidizer.Power;

                if (oxEq > 0f)
                {
                    activeOxidizers[oxidizerCount++] = new StagedOxidizerData
                    {
                        GasIndex = i,
                        AvailableMoles = moles,
                        OxProps = oxidizer
                    };
                    totalOxygenEqAvailable += oxEq;
                }
            }

            // Process fuels
            if (gasProto.IsFuel && gasProto.FuelProperties != null)
            {
                var fuel = gasProto.FuelProperties.Value;

                var dissociationFactor = 0f;
                if (currentTemp > fuel.DissociationOnsetTemperature && fuel.MaxFlameTemperature > fuel.DissociationOnsetTemperature)
                {
                    var range = fuel.MaxFlameTemperature - fuel.DissociationOnsetTemperature;
                    var progress = currentTemp - fuel.DissociationOnsetTemperature;
                    dissociationFactor = MathF.Min(1.0f, progress / range);
                }

                if (dissociationFactor >= 1.0f)
                    continue;

                var maxFuelBurn = moles / fuel.BurnRateDelta;

                var fuelBurnedTarget = MathF.Min(maxFuelBurn, moles);
                var oxygenEqDemanded = fuelBurnedTarget * fuel.OxygenRatio;

                activeFuels[fuelCount++] = new StagedFuelData
                {
                    GasIndex = i,
                    TargetBurn = fuelBurnedTarget,
                    DissociationFactor = dissociationFactor,
                    AvailableMoles = moles,
                    FuelProps = fuel
                };
                totalOxygenEqDemanded += oxygenEqDemanded;
            }
        }

        // Calculate global capacity limits based on mutual resource constraints
        var capacityScaling = 1.0f;
        if (totalOxygenEqDemanded > totalOxygenEqAvailable)
        {
            capacityScaling = totalOxygenEqAvailable / totalOxygenEqDemanded;
        }

        var totalOxygenEqBurned = totalOxygenEqDemanded * capacityScaling;
        var oxidizerConsumptionRatio = totalOxygenEqAvailable > 0f ? (totalOxygenEqBurned / totalOxygenEqAvailable) : 0f;

        var totalEnergyReleased = 0f;
        var totalGasReacted = 0f;
        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        // Burn staged fuels
        for (var i = 0; i < fuelCount; i++)
        {
            ref var data = ref activeFuels[i];
            var finalFuelBurned = data.TargetBurn * capacityScaling;
            if (finalFuelBurned <= 0f)
                continue;

            var gasId = (Gas)data.GasIndex;
            mixture.AdjustMoles(gasId, -finalFuelBurned);
            totalGasReacted += finalFuelBurned;

            var fuelProps = data.FuelProps;
            var effFactor = 1.0f - data.DissociationFactor;

            // Plasma supersaturation logic check
            if (gasId == Gas.Plasma && (totalOxygenEqAvailable / data.AvailableMoles) >= Atmospherics.SuperSaturationThreshold)
            {
                var oxygenReturned = finalFuelBurned * 2.0f;
                mixture.AdjustMoles(Gas.Hydrogen, finalFuelBurned);
                mixture.AdjustMoles(Gas.Oxygen, oxygenReturned);
            }
            else
            {
                foreach (var (productGas, ratio) in fuelProps.StandardProducts)
                {
                    var amount = finalFuelBurned * ratio * effFactor;
                    if (amount > 0f)
                        mixture.AdjustMoles(productGas, amount);
                }

                foreach (var (productGas, ratio) in fuelProps.DissociationProducts)
                {
                    var amount = finalFuelBurned * ratio * data.DissociationFactor;
                    if (amount > 0f)
                        mixture.AdjustMoles(productGas, amount);
                }
            }

            totalEnergyReleased += finalFuelBurned * fuelProps.EnergyReleased * effFactor;
        }

        // Burn staged oxidizers
        for (var i = 0; i < oxidizerCount; i++)
        {
            ref var data = ref activeOxidizers[i];
            var finalOxidizerBurned = data.AvailableMoles * oxidizerConsumptionRatio;
            if (finalOxidizerBurned <= 0f)
                continue;

            var gasId = (Gas)data.GasIndex;
            mixture.AdjustMoles(gasId, -finalOxidizerBurned);
            totalGasReacted += finalOxidizerBurned;

            var oxProps = data.OxProps;

            foreach (var (productGas, ratio) in oxProps.Byproducts)
            {
                var amount = finalOxidizerBurned * ratio;
                if (amount > 0f)
                    mixture.AdjustMoles(productGas, amount);
            }
        }

        // Add thermal energy
        mixture.ReactionResults[(byte)GasReaction.Fire] = totalGasReacted;
        totalEnergyReleased /= heatScale;

        if (totalEnergyReleased > 0f && oldHeatCapacity > Atmospherics.MinimumHeatCapacity)
        {
            var totalThermalEnergy = (currentTemp * oldHeatCapacity) + totalEnergyReleased;
            var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

            if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
                mixture.Temperature = totalThermalEnergy / newHeatCapacity;
        }

        if (tile != null && mixture.Temperature > Atmospherics.FireMinimumTemperatureToExist)
        {
            atmosphereSystem.HotspotExpose(tile, mixture.Temperature, mixture.Volume);
        }

        return ReactionResult.Reacting;
    }
}

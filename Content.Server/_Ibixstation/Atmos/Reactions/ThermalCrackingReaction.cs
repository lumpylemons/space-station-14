using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class ThermalCrackingReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var currentTemp = mixture.Temperature;
        var totalGasReacted = 0f;
        var totalEnergyAbsorbed = 0f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        for (var i = 0; i < IbixAtmospherics.CrackableGases.Length; i++)
        {
            var gasId = IbixAtmospherics.CrackableGases[i];
            var gasIndex = (int)gasId;
            var moles = mixture.GetMoles(gasIndex);
            if (moles < Atmospherics.GasMinMoles)
                continue;

            var gasProto = atmosphereSystem.GetGas(gasIndex);
            if (gasProto.CrackingProperties == null)
                continue;

            var crackProps = gasProto.CrackingProperties.Value;
            if (currentTemp < crackProps.OnsetTemperature)
                continue;

            // Determine cracking speed
            var maxCrackingAmount = moles / crackProps.CrackingRateDelta;

            var finalMolesCracked = MathF.Min(maxCrackingAmount, moles);
            if (finalMolesCracked <= 0f)
                continue;

            // Deduct the reactant gas
            mixture.AdjustMoles(gasId, -finalMolesCracked);
            totalGasReacted += finalMolesCracked;

            // Synthesize product gases
            foreach (var (productGas, ratio) in crackProps.Products)
            {
                var amount = finalMolesCracked * ratio;
                if (amount > 0f)
                    mixture.AdjustMoles(productGas, amount);
            }

            // Accumulate total energy absorbed
            totalEnergyAbsorbed += finalMolesCracked * crackProps.EnergyAbsorbed;
        }

        totalEnergyAbsorbed /= heatScale;

        if (totalEnergyAbsorbed > 0f && oldHeatCapacity > Atmospherics.MinimumHeatCapacity)
        {
            // Subtract absorbed energy
            var totalThermalEnergy = (currentTemp * oldHeatCapacity) - totalEnergyAbsorbed;
            var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

            if (newHeatCapacity > Atmospherics.MinimumHeatCapacity && totalThermalEnergy > 0f)
            {
                mixture.Temperature = totalThermalEnergy / newHeatCapacity;
            }
            else
            {
                mixture.Temperature = Atmospherics.TCMB;
            }
        }

        return ReactionResult.Reacting;
    }
}

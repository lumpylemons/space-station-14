using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class CarbonDioxideDecompositionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var temperature = mixture.Temperature;
        var initialCO2 = mixture.GetMoles(Gas.CarbonDioxide);

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        // Arrhenius exponential activation rate
        // Scales decomposition rapidly as temperature approaches high limits
        var activationFactor = MathF.Exp(-IbixAtmospherics.CO2DecompositionActivationEnergy / temperature);
        var decompRate = initialCO2 * activationFactor;
        var co2Decomposed = MathF.Min(decompRate, initialCO2);

        mixture.AdjustMoles(Gas.CarbonDioxide, -co2Decomposed);
        mixture.AdjustMoles(Gas.CarbonMonoxide, co2Decomposed);
        mixture.AdjustMoles(Gas.Oxygen, co2Decomposed * 0.5f);

        var energyAbsorbed = co2Decomposed * IbixAtmospherics.CO2DecompositionEnergyAbsorbed;
        energyAbsorbed /= heatScale;

        var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
        if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
        {
            mixture.Temperature = MathF.Max(
                Atmospherics.TCMB,
                (temperature * oldHeatCapacity - energyAbsorbed) / newHeatCapacity
            );
        }

        return ReactionResult.Reacting;
    }
}

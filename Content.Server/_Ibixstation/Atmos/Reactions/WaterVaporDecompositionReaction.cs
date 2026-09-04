using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

/// <summary>
///     Endothermic decomposition of Water Vapor back into Tritium and Oxygen.
///     Scales exponentially with temperature to create a natural thermal lock.
/// </summary>
[UsedImplicitly]
public sealed partial class WaterVaporDecompositionReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var temperature = mixture.Temperature;

        // Skip calculations below activation threshold
        if (temperature < Atmospherics.WaterDecompositionMinTemp)
            return ReactionResult.NoReaction;

        var initialWater = mixture.GetMoles(Gas.WaterVapor);
        if (initialWater <= Atmospherics.MinimumHeatCapacity)
            return ReactionResult.NoReaction;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        // Arrhenius-style exponential activation rate
        // Scales decomposition rapidly as temperature approaches high limits
        var activationFactor = MathF.Exp(-Atmospherics.WaterDecompositionActivationEnergy / temperature);
        var decompRate = initialWater * activationFactor;
        var waterDecomposed = MathF.Min(decompRate, initialWater);

        if (waterDecomposed <= Atmospherics.MinimumHeatCapacity)
            return ReactionResult.NoReaction;

        mixture.AdjustMoles(Gas.WaterVapor, -waterDecomposed);
        mixture.AdjustMoles(Gas.Tritium, waterDecomposed);
        mixture.AdjustMoles(Gas.Oxygen, waterDecomposed * 0.5f);

        // Energy absorbed per mole decomposed (Negative energyDelta)
        var energyAbsorbed = waterDecomposed * Atmospherics.WaterDecompositionEnergyAbsorbed;
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

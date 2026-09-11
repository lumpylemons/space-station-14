using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Radiation.Components;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class HydrogenTritiumRadiationReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var entityManager = IoCManager.Resolve<IEntityManager>();

        if (holder is Component component && entityManager.TryGetComponent<RadiationReceiverComponent>(component.Owner, out var receiver))
        {
            var rads = receiver.CurrentRadiation;

            var hydrogenMoles = mixture.GetMoles(Gas.Hydrogen);
            var conversionRate = (hydrogenMoles * rads) / IbixAtmospherics.HydrogenTritiumRadiationReactionRate;

            var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

            mixture.AdjustMoles(Gas.Hydrogen, -conversionRate);
            mixture.AdjustMoles(Gas.Tritium, conversionRate);

            var energyReleased = conversionRate * IbixAtmospherics.HydrogenTritiumRadiationEnergyReleased;
            energyReleased /= heatScale;

            if (energyReleased > 0f)
            {
                var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
                if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
                {
                    mixture.Temperature = (mixture.Temperature * oldHeatCapacity + energyReleased) / newHeatCapacity;
                }
            }

            return ReactionResult.Reacting;
        }

        return ReactionResult.NoReaction;
    }
}

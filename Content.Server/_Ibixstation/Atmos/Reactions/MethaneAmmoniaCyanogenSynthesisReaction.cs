using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class MethaneAmmoniaCyanogenSynthesisReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        if (mixture.Pressure > IbixAtmospherics.MethaneAmmoniaCyanogenSynthesisMaximumPressure)
            return ReactionResult.NoReaction;

        var nMethane = mixture.GetMoles(Gas.Methane);
        var nAmmonia = mixture.GetMoles(Gas.Ammonia);
        var nTotal = mixture.TotalMoles;

        var maxMethaneByAmmonia = nAmmonia;
        var reactingMethane = MathF.Min(nMethane, maxMethaneByAmmonia);

        var fMethane = nMethane / nTotal;
        var fAmmonia = nAmmonia / nTotal;

        var collisionRate = fMethane * fAmmonia;

        collisionRate /= MathF.Max(1.0f, mixture.Pressure / Atmospherics.OneAtmosphere);

        var deltaMethane = (reactingMethane / IbixAtmospherics.MethaneAmmoniaCyanogenSynthesisReactionRate) * collisionRate;

        var deltaAmmonia = deltaMethane;
        var deltaCyanogen = deltaMethane / 2f;
        var deltaHydrogen = deltaMethane * 3f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.Methane, -deltaMethane);
        mixture.AdjustMoles(Gas.Ammonia, -deltaAmmonia);
        mixture.AdjustMoles(Gas.Cyanogen, deltaCyanogen);
        mixture.AdjustMoles(Gas.Hydrogen, deltaHydrogen);

        var energyAbsorbed = deltaCyanogen * IbixAtmospherics.MethaneAmmoniaCyanogenSynthesisEnergyAbsorbed;
        energyAbsorbed /= heatScale;

        if (energyAbsorbed > 0f)
        {
            var newHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);
            if (newHeatCapacity > Atmospherics.MinimumHeatCapacity)
            {
                mixture.Temperature = (mixture.Temperature * oldHeatCapacity - energyAbsorbed) / newHeatCapacity;
            }
        }

        return ReactionResult.Reacting;
    }
}

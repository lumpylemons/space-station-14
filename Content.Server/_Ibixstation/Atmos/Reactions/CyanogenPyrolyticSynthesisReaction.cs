using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class CyanogenPyrolyticSynthesisReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var temperature = mixture.Temperature;
        var nMethane = mixture.GetMoles(Gas.Methane);
        var nAmmonia = mixture.GetMoles(Gas.Ammonia);
        var nTotal = mixture.TotalMoles;

        var maxByMethane = nMethane / 2f;
        var maxByAmmonia = nAmmonia / 2f;
        var reactingUnits = MathF.Min(maxByMethane, maxByAmmonia);

        var fMethane = nMethane / nTotal;
        var fAmmonia = nAmmonia / nTotal;

        var activationFactor = MathF.Exp(-IbixAtmospherics.CyanogenPyrolysisActivationEnergy / temperature);
        var rate = fMethane * fAmmonia * activationFactor;

        var deltaCyanogen = reactingUnits / IbixAtmospherics.CyanogenPyrolysisReactionRate * rate;

        var deltaMethane = deltaCyanogen * 2f;
        var deltaAmmonia = deltaCyanogen * 2f;
        var deltaHydrogen = deltaCyanogen * 7f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.Methane, -deltaMethane);
        mixture.AdjustMoles(Gas.Ammonia, -deltaAmmonia);
        mixture.AdjustMoles(Gas.Cyanogen, deltaCyanogen);
        mixture.AdjustMoles(Gas.Hydrogen, deltaHydrogen);

        var energyAbsorbed = deltaCyanogen * IbixAtmospherics.CyanogenPyrolysisEnergyAbsorbed;
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

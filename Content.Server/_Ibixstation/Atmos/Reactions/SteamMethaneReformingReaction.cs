using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class SteamMethaneReformingReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var temperature = mixture.Temperature;
        var nMethane = mixture.GetMoles(Gas.Methane);
        var nWater = mixture.GetMoles(Gas.WaterVapor);
        var nTotal = mixture.TotalMoles;

        var reactingUnits = MathF.Min(nMethane, nWater);

        var fMethane = nMethane / nTotal;
        var fWater = nWater / nTotal;

        var limitingFraction = MathF.Min(fMethane, fWater);
        var activationFactor = MathF.Exp(-IbixAtmospherics.SteamMethaneReformingActivationEnergy / temperature);

        var rate = limitingFraction * activationFactor;
        var deltaMethane = reactingUnits / IbixAtmospherics.SteamMethaneReformingReactionRate * rate;

        var deltaWater = deltaMethane;
        var deltaCO = deltaMethane;
        var deltaHydrogen = deltaMethane * 3f;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.Methane, -deltaMethane);
        mixture.AdjustMoles(Gas.WaterVapor, -deltaWater);
        mixture.AdjustMoles(Gas.CarbonMonoxide, deltaCO);
        mixture.AdjustMoles(Gas.Hydrogen, deltaHydrogen);

        var energyAbsorbed = deltaMethane * IbixAtmospherics.SteamMethaneReformingEnergyAbsorbed;
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

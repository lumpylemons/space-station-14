using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Shared._Ibixstation.Atmos;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Reactions;
using JetBrains.Annotations;

namespace Content.Server._Ibixstation.Atmos.Reactions;

[UsedImplicitly]
public sealed partial class ReverseWaterGasShiftReaction : IGasReactionEffect
{
    public ReactionResult React(GasMixture mixture, IGasMixtureHolder? holder, AtmosphereSystem atmosphereSystem, float heatScale)
    {
        var temperature = mixture.Temperature;
        var nCO2 = mixture.GetMoles(Gas.CarbonDioxide);
        var nHydrogen = mixture.GetMoles(Gas.Hydrogen);
        var nTotal = mixture.TotalMoles;

        var reactingUnits = MathF.Min(nCO2, nHydrogen);

        var fCO2 = nCO2 / nTotal;
        var fHydrogen = nHydrogen / nTotal;

        var limitingFraction = MathF.Min(fCO2, fHydrogen);
        var activationFactor = MathF.Exp(-IbixAtmospherics.ReverseWaterGasShiftActivationEnergy / temperature);

        var rate = limitingFraction * activationFactor;
        var deltaCO2 = reactingUnits / IbixAtmospherics.ReverseWaterGasShiftReactionRate * rate;

        var deltaHydrogen = deltaCO2;
        var deltaCO = deltaCO2;
        var deltaWater = deltaCO2;

        var oldHeatCapacity = atmosphereSystem.GetHeatCapacity(mixture, true);

        mixture.AdjustMoles(Gas.CarbonDioxide, -deltaCO2);
        mixture.AdjustMoles(Gas.Hydrogen, -deltaHydrogen);
        mixture.AdjustMoles(Gas.CarbonMonoxide, deltaCO);
        mixture.AdjustMoles(Gas.WaterVapor, deltaWater);

        var energyAbsorbed = deltaCO2 * IbixAtmospherics.ReverseWaterGasShiftEnergyAbsorbed;
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
